using Common;
using Common.Extensions;
using Common.Security;
using Dodo;
using Dodo.Models;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

/// <summary>
/// Handles all user related services
/// </summary>
public class UserService : ResourceServiceBase<User, UserSchema>
{
	public const string RootURL = "auth";
	public const string LOGIN = "login";
	public const string LOGOUT = "logout";
	public const string REGISTER = "register";
	public const string RESET_PASSWORD = "resetpassword";
	public const string REDEEM_PASSWORD_TOKEN = "passtokenredeem";
	public const string SUBMIT_PASSWORD_RESET = "submitpassreset";
	public const string CHANGE_PASSWORD = "changepassword";
	public const string PARAM_TOKEN = "token";
	public const string VERIFY_EMAIL = "verifyemail";

	public UserService(AccessContext context, HttpContext httpContext, AuthorizationService<User, UserSchema> auth)
		: base(context, httpContext, auth)
	{
	}

	public async Task<IRequestResult> Login(LoginModel login)
	{
		var logstr = $"Login request for '{login.Username}'" +
			(string.IsNullOrEmpty(login.Redirect) ? "" : $" (redirect: {login.Redirect})" + ": ");
		if (Context.User != null)
		{
			// User is already logged in
			Logger.Debug($"{logstr} User was already logged in under guid {Context.User.Guid}");
			return ResourceRequestError.BadRequest($"User is already signed in as {Context.User.Slug}");
		}

		var user = ResourceManager.GetSingle(x => x.Slug == login.Username);
		if (user == null)
		{
			Logger.Debug($"{logstr} User was not found with that username.");
			return ResourceRequestError.UnauthorizedRequest("User was not found with that username.");
		}
		if (!user.AuthData.ChallengePassword(login.Password, out var passphrase))
		{
			Logger.Debug($"{logstr} User provided incorrect password.");
			return ResourceRequestError.UnauthorizedRequest("User provided incorrect password.");
		}

		// Generate an encryption key that we will include in the cookie and throw away on our end
		var key = new Passphrase(KeyGenerator.GetUniqueKey(SessionToken.KEYSIZE));

		// Create the session token

		var sessionTimeout = login.RememberMe ? SessionToken.ShortSessionExpiryTime : SessionToken.LongSessionExpiryTime;
		var token = new SessionToken(user, passphrase, key, HttpContext.Connection.RemoteIpAddress, DateTime.UtcNow + sessionTimeout);
		using (var rscLock = new ResourceLock(user))
		{
			user = rscLock.Value as User;
			user.TokenCollection.AddOrUpdate(user, token);
			UserManager.Update(user, rscLock);
		}

		// Create the claims ID
		var id = new ClaimsIdentity(AuthConstants.AUTHSCHEME);
		id.AddClaim(new Claim(AuthConstants.SUBJECT, token.UserKey));
		id.AddClaim(new Claim(AuthConstants.KEY, key.Value));
		var principal = new ClaimsPrincipal(id);
		var props = new AuthenticationProperties
		{
			IsPersistent = true,
			ExpiresUtc = DateTimeOffset.UtcNow.Add(sessionTimeout),
		};
		// issue authentication cookie with subject ID and username
		await HttpContext.SignInAsync(AuthConstants.AUTHSCHEME, principal, props);
		Logger.Debug($"{logstr} Request was successful, created new session token {token.Guid} (expires {token.ExpiryDate}) for {HttpContext.Connection.RemoteIpAddress}");
		return new OkRequestResult(user.GenerateJsonView(EPermissionLevel.ADMIN, user, new Passphrase(user.AuthData.PassPhrase.GetValue(login.Password))));
	}

	public async Task<IRequestResult> Logout()
	{
		if (Context.User == null)
		{
			return ResourceRequestError.ForbidRequest();
		}
		await HttpContext.SignOutAsync(AuthConstants.AUTHSCHEME);
		using var rscLock = new ResourceLock(Context.User);
		var user = rscLock.Value as User;
		var session = user.TokenCollection.GetAllTokens<SessionToken>(Context, EPermissionLevel.ADMIN, user)
				.SingleOrDefault(t => t.UserKey == Context.UserToken);
		if (session == null)
		{
			return ResourceRequestError.BadRequest();
		}
		if (!user.TokenCollection.Remove<SessionToken>(Context, EPermissionLevel.ADMIN, session, user))
		{
			Logger.Error($"Failed to log user {user} out - could not remove session token");
		}
		UserManager.Update(user, rscLock);
		Logger.Debug($"Logged out user {user.Slug}");
		return new RedirectRequestResult(Dodo.DodoApp.NetConfig.FullURI);
	}

	public async Task<IRequestResult> RequestPasswordReset(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return ResourceRequestError.BadRequest("Email can't be empty.");
		}
		if (Context.User != null && Context.User.PersonalData.Email != email)
		{
			return ResourceRequestError.BadRequest("User mismatch.");
		}
		var targetUser = UserManager.GetSingle(u => u.PersonalData.Email == email);
		if (targetUser != null)
		{
			using (var rscLock = new ResourceLock(targetUser))
			{
				targetUser = rscLock.Value as User;
				targetUser.TokenCollection.RemoveAllOfType<ResetPasswordToken>(Context, EPermissionLevel.ADMIN, targetUser);
				var resetToken = new ResetPasswordToken(targetUser);
				targetUser.TokenCollection.AddOrUpdate(targetUser, resetToken);
				UserManager.Update(targetUser, rscLock);

				var url = $"{Dodo.DodoApp.NetConfig.FullURI}/{REDEEM_PASSWORD_TOKEN}?token={resetToken.Key}";
				EmailUtility.SendEmail(
					new EmailAddress
					{
						Email = targetUser.PersonalData.Email,
						Name = targetUser.Name
					},
					$"Reset Your Password on {Dodo.DodoApp.PRODUCT_NAME}",
					"Callback",
					new Dictionary<string, string>
					{
						{ "MESSAGE", $"You've asked to reset your password on {Dodo.DodoApp.PRODUCT_NAME}. To set a new password, please follow the link below. " +
						"Please note that resetting your password will remove you as an administrator on any groups that you currently administrate." },
						{ "CALLBACK_MESSAGE", "Reset Password" },
						{ "CALLBACK_URL", url }
					});
			}
		}
		return new OkRequestResult("If an account with that email exists, you will receive a one-time link to reset your password.");
	}

	public async Task<IRequestResult> ResetPassword(string token, string password)
	{
		if (string.IsNullOrEmpty(token) ||
			!ValidationExtensions.IsStrongPassword(password, out _))
		{
			return ResourceRequestError.BadRequest();
		}
		var user = UserManager.GetSingle(u =>
			u.TokenCollection.GetSingleToken<ResetPasswordToken>(Context, EPermissionLevel.ADMIN, null)?.Key == token);
		if (user == null)
		{
			return ResourceRequestError.BadRequest("Token not found - possibly you've already changed your password?");
		}
		using (var rscLock = new ResourceLock(user))
		{
			user = rscLock.Value as User;
			// This will wipe the private key and passphrase, so the user needs to start fresh
			user.AuthData = new AuthorizationData(password);
			// Get rid of the password reset token immediately, as opposed to just waiting for it to get cleaned up
			user.TokenCollection.RemoveAllOfType<ResetPasswordToken>(Context, EPermissionLevel.ADMIN, user);
			// User won't be able to decrypt any decrypted tokens
			user.TokenCollection.Remove<IToken>(Context, EPermissionLevel.ADMIN, t => t.Encrypted, user);
			UserManager.Update(user, rscLock);
		}
		await Logout();
		return new RedirectRequestResult("~/");
	}

	public async Task<IRequestResult> ChangePassword(ChangePasswordModel model)
	{
		if (Context.User == null)
		{
			return ResourceRequestError.ForbidRequest();
		}
		if (!Context.User.AuthData.ChallengePassword(model.CurrentPassword, out _))
		{
			return ResourceRequestError.UnauthorizedRequest();
		}
		using (var rscLock = new ResourceLock(Context.User))
		{
			var user = rscLock.Value as User;
			user.AuthData.ChangePassword(new Passphrase(model.CurrentPassword), new Passphrase(model.NewPassword));
			UserManager.Update(user, rscLock);
		}
		await Logout();
		return new OkRequestResult();
	}

	public async Task<IRequestResult> VerifyEmail(string token)
	{
		if (Context.User == null)
		{
			return ResourceRequestError.ForbidRequest();
		}
		if (Context.User.PersonalData.EmailConfirmed)
		{
			return new OkRequestResult("You've already verified your email address");
		}
		var verifyToken = Context.User.TokenCollection.GetSingleToken<VerifyEmailToken>(Context, EPermissionLevel.ADMIN, Context.User);
		if (string.IsNullOrEmpty(token))
		{
			if (verifyToken.ConfirmationEmailRequestCount < VerifyEmailToken.MAX_REQUEST_COUNT)
			{
				// user is requesting new email
				SendEmailVerification(Context);
				return new OkRequestResult($"A new email verification link has been sent to {Context.User.PersonalData.Email}");
			}
		}

		if (verifyToken == null)
		{
			return ResourceRequestError.BadRequest();
		}
		if (verifyToken.Token != token)
		{
			Logger.Debug($"Expected token {verifyToken.Token} but got {token}");
			return ResourceRequestError.BadRequest("Token mismatch");
		}
		using var rscLock = new ResourceLock(Context.User);
		var user = rscLock.Value as User;
		user.PersonalData.EmailConfirmed = true;
		UserManager.Update(user, rscLock);
		return new OkRequestResult($"Successfully verified email {Context.User.PersonalData.Email}");
	}

	public async Task<IRequestResult> Register(UserSchema schema, string token = null)
	{
		if (!string.IsNullOrEmpty(token))
		{
			return await RegisterWithToken(token, schema);
		}
		var request = VerifyRequest(schema);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceCreationRequest)request;
		var user = ResourceManager.GetSingle(x => x.Slug == schema.Username);
		if (user != null)
		{
			return ResourceRequestError.UnauthorizedRequest("A user already exists with that username");
		}
		user = ResourceManager.GetSingle(x => x.PersonalData.Email == schema.Email);
		if (user != null)
		{
			return ResourceRequestError.UnauthorizedRequest("A user already exists with that email address");
		}
		EmailUtility.ClearEmailUnsubscription(schema.Email); // Clear any unsubscription on this email
		var factory = ResourceUtility.GetFactory<User>();
		user = factory.CreateTypedObject(new ResourceCreationRequest(default, schema));
		var passphrase = new Passphrase(user.AuthData.PassPhrase.GetValue(schema.Password));
		SendEmailVerification(new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)));
		await Login(new LoginModel { Username = schema.Username, Password = schema.Password });
		return new ResourceCreationRequest(
			new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)),
			schema)
		{
			Result = user
		};
	}

	public async Task<IRequestResult> RegisterWithToken(string token, UserSchema schema)
	{
		var request = VerifyRequest(schema);
		if (request is ResourceRequestError error)
		{
			return error;
		}
		var req = (ResourceCreationRequest)request;
		var user = ResourceManager.GetSingle(x => x.PersonalData.Email == schema.Email);
		if (user == null)
		{
			return ResourceRequestError.BadRequest();
		}
		// Check if this is a temporary user (e.g. a new admin activating an email invite to administrate a resource)
		var tempToken = user.TokenCollection.GetSingleToken<TemporaryUserToken>(Context, EPermissionLevel.ADMIN, Context.User);
		if (tempToken == null || !PasswordHasher.VerifyHashedPassword(tempToken.TokenChallenge, token))
		{
			Logger.Warning($"Registering user tried to redeem invalid token {token}");
			return ResourceRequestError.Conflict();
		}
		if (UserManager.GetSingle(u => u.Slug == schema.Username) != null)
		{
			return ResourceRequestError.Conflict();
		}
		using (var rscLock = new ResourceLock(user.Guid))
		{
			if (!user.AuthData.ChangePassword(new Passphrase(tempToken.Password), new Passphrase(schema.Password)))
			{
				return ResourceRequestError.BadRequest();
			}
			user.Slug = schema.Username;
			user.Name = schema.Name;
			if (!user.Verify(out var verificationError))
			{
				return ResourceRequestError.BadRequest(verificationError);
			}
			tempToken.Redeem(default);
			UserManager.Update(user, rscLock);
		}
		SendEmailVerification(new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)));
		req.Result = user;
		return req;
	}

	public static IRequestResult SendEmailVerification(AccessContext context)
	{
		if (context.User.PersonalData.EmailConfirmed)
		{
			return new OkRequestResult("User has already confirmed email");
		}
		using var rscLock = new ResourceLock(context.User);
		var user = rscLock.Value as User;
		var token = user.TokenCollection.GetSingleToken<VerifyEmailToken>(context, EPermissionLevel.ADMIN, context.User);
		if (token == null)
		{
			token = user.TokenCollection.AddOrUpdate(context.User, new VerifyEmailToken()) as VerifyEmailToken;
		}
		if (token.ConfirmationEmailRequestCount >= VerifyEmailToken.MAX_REQUEST_COUNT)
		{
			return ResourceRequestError.BadRequest("User has requested maximum number of email verifications");
		}
		var url = $"{Dodo.DodoApp.NetConfig.FullURI}/{RootURL}/{VERIFY_EMAIL}?token={token.Token}";
		EmailUtility.SendEmail(
			new EmailAddress
			{
				Email = context.User.PersonalData.Email,
				Name = context.User.Name
			},
			$"Verify Your Email With {Dodo.DodoApp.PRODUCT_NAME}",
			"Callback",
			new Dictionary<string, string>
			{
				{ "MESSAGE", $"You just registered at {Dodo.DodoApp.PRODUCT_NAME}. To verify your email address, please click the button below." },
				{ "CALLBACK_MESSAGE", "Verify Your Email Address" },
				{ "CALLBACK_URL", $"{Dodo.DodoApp.NetConfig.FullURI}/{UserService.RootURL}/{UserService.VERIFY_EMAIL}?token={token.Token}" }
			});
		token.ConfirmationEmailRequestCount++;
		user.TokenCollection.AddOrUpdate(user, token);
		ResourceManager.Update(user, rscLock);
		return new OkRequestResult($"Sent email confirmation to {context.User.PersonalData.Email}");
	}

	public static User CreateTemporaryUser(string email)
	{
		var temporaryPassword = new Passphrase(ValidationExtensions.GenerateStrongPassword());
		var schema = new UserSchema($"tempuser_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16)}",
			temporaryPassword.Value, email);
		var factory = ResourceUtility.GetFactory<User>();
		var newUser = factory.CreateTypedObject(new ResourceCreationRequest(default, schema));
		// TODO make this a bit more crytpsecure
		var token = KeyGenerator.GetUniqueKey(32).ToLowerInvariant();
		var tokenChallenge = PasswordHasher.HashPassword(token);
#if DEBUG
		if (!PasswordHasher.VerifyHashedPassword(tokenChallenge, token))
		{
			throw new Exception("Failed to generate challenge");
		}
		Logger.Debug($"Temp user token generated: {token}");
#endif
		using (var rscLock = new ResourceLock(newUser))
		{
			newUser.TokenCollection.AddOrUpdate(newUser, new TemporaryUserToken(temporaryPassword, tokenChallenge));
			ResourceUtility.GetManager<User>().Update(newUser, rscLock);
		}

		var url = $"{Dodo.DodoApp.NetConfig.FullURI}/{REGISTER}?token={token}";
		EmailUtility.SendEmail(
				new EmailAddress
				{
					Email = newUser.PersonalData.Email,
					Name = ""
				},
				$"You've been invited to create an account on {Dodo.DodoApp.PRODUCT_NAME}",
				"Callback",
				new Dictionary<string, string>
				{
					{ "MESSAGE", $"You've been invited to create an account on {Dodo.DodoApp.PRODUCT_NAME}. To create your account, please follow the link below." },
					{ "CALLBACK_MESSAGE", "Register Your Account" },
					{ "CALLBACK_URL", url }
				});
		return newUser;
	}
}
