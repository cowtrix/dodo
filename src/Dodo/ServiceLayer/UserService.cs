using Common;
using Common.Extensions;
using Common.Security;
using Dodo;
using Dodo.Models;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Security;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

public class UserService : ResourceServiceBase<User, UserSchema>
{
	public const string RootURL = "auth";
	public const string LOGIN = "login";
	public const string LOGOUT = "logout";
	public const string REGISTER = "register";
	public const string RESET_PASSWORD = "resetpassword";
	public const string CHANGE_PASSWORD = "changepassword";
	public const string PARAM_TOKEN = "token";
	public const string VERIFY_EMAIL = "verifyemail";

	public UserService(AccessContext context, HttpContext httpContext, AuthorizationService<User, UserSchema> auth)
		: base(context, httpContext, auth)
	{
	}

	public async Task<IRequestResult> Login(LoginModel login)
	{
		var logstr = $"Login request for {login.Username}" +
			(string.IsNullOrEmpty(login.Redirect) ? "" : $" (redirect: {login.Redirect}).");
		if (Context.User != null)
		{
			// User is already logged in
			Logger.Debug($"{logstr} User was already logged in under guid {Context.User.Guid}");
			return new OkRequestResult();
		}

		var user = ResourceManager.GetSingle(x => x.AuthData.Username == login.Username);
		if (user == null)
		{
			Logger.Debug($"{logstr} User was not found with that username.");
			return ResourceRequestError.NotFoundRequest();
		}
		if (!user.AuthData.ChallengePassword(login.Password, out var passphrase))
		{
			Logger.Debug($"{logstr} User provided incorrect username.");
			return ResourceRequestError.BadRequest();
		}

		// Generate an encryption key that we will include in the cookie and throw away on our end
		var key = new Passphrase(KeyGenerator.GetUniqueKey(SessionToken.KEYSIZE));

		// Create the session token
		var token = new SessionToken(user, passphrase, key, HttpContext.Connection.RemoteIpAddress);
		using (var rscLock = new ResourceLock(user))
		{
			user = rscLock.Value as User;
			user.TokenCollection.Add(user, token);
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
			ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(1))
		};
		// issue authentication cookie with subject ID and username
		await HttpContext.SignInAsync(AuthConstants.AUTHSCHEME, principal, props);
		Logger.Debug($"{logstr} Request was successful, created new session token {token.Guid} (expires {token.ExpiryDate})");
		return new OkRequestResult(user.GenerateJsonView(EPermissionLevel.OWNER, user, new Passphrase(user.AuthData.PassPhrase.GetValue(login.Password))));
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
		var session = user.TokenCollection.GetAllTokens<SessionToken>(Context, EPermissionLevel.OWNER)
				.SingleOrDefault(t => t.UserKey == Context.UserToken);
		if (session == null)
		{
			return ResourceRequestError.BadRequest();
		}
		if (!user.TokenCollection.Remove(Context, session))
		{
			Logger.Error($"Failed to log user {user} out - could not remove session token");
		}
		UserManager.Update(user, rscLock);
		Logger.Debug($"Logged out user {user.AuthData.Username}");
		return new OkRequestResult();
	}

	public async Task<IRequestResult> RequestPasswordReset(string email)
	{
		if (Context.User != null && Context.User.PersonalData.Email != email)
		{
			return ResourceRequestError.BadRequest("Mismatching emails");
		}
		var targetUser = UserManager.GetSingle(u => u.PersonalData.Email == email);
		if (targetUser != null)
		{
			using (var rscLock = new ResourceLock(targetUser))
			{
				targetUser = rscLock.Value as User;
				var resetToken = new ResetPasswordToken(targetUser);
				targetUser.TokenCollection.Add(targetUser, resetToken);
				UserManager.Update(targetUser, rscLock);
				EmailHelper.SendPasswordResetEmail(targetUser.PersonalData.Email, targetUser.Name,
					$"{Dodo.DodoApp.NetConfig.FullURI}/{RootURL}/{RESET_PASSWORD}?token={resetToken.Key}");
			}
		}
		return new OkRequestResult();
	}

	public async Task<IRequestResult> ResetPassword(string token, string password)
	{
		if (string.IsNullOrEmpty(token) ||
			!ValidationExtensions.IsStrongPassword(password, out _))
		{
			return ResourceRequestError.BadRequest();
		}
		var user = UserManager.GetSingle(u =>
			u.TokenCollection.GetSingleToken<ResetPasswordToken>(Context)?.Key == token);
		if (user == null)
		{
			return ResourceRequestError.BadRequest();
		}
		using (var rscLock = new ResourceLock(user))
		{
			user = rscLock.Value as User;
			user.TokenCollection.RemoveAll<ResetPasswordToken>(Context);
			user.AuthData = new AuthorizationData(user.AuthData.Username, password);
			UserManager.Update(user, rscLock);
		}
		await Logout();
		return new OkRequestResult();
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
			return new OkRequestResult();
		}
		var verifyToken = Context.User.TokenCollection.GetSingleToken<VerifyEmailToken>(Context);
		if (verifyToken == null)
		{
			throw new Exception($"Verify token was null for user {Context.User.Guid}");
		}
		if (verifyToken.Token != token)
		{
			return ResourceRequestError.ForbidRequest("Token mismatch");
		}
		using var rscLock = new ResourceLock(Context.User);
		var user = rscLock.Value as User;
		user.PersonalData.EmailConfirmed = true;
		UserManager.Update(user, rscLock);
		return new OkRequestResult();
	}

	public async Task<IRequestResult> Register(UserSchema schema, string token = null)
	{
		if (!string.IsNullOrEmpty(token))
		{
			return await CreateWithToken(token, schema);
		}
		var request = VerifyRequest(schema);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceCreationRequest)request;
		var user = ResourceManager.GetSingle(x => x.AuthData.Username == schema.Username || x.PersonalData.Email == schema.Email);
		if (user != null)
		{
			return ResourceRequestError.Conflict();
		}
		var factory = ResourceUtility.GetFactory<User>();
		user = factory.CreateTypedObject(new ResourceCreationRequest(default, schema));
		var passphrase = new Passphrase(user.AuthData.PassPhrase.GetValue(schema.Password));
		SendEmailVerification(new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)));
		return new ResourceCreationRequest(
			new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)),
			schema)
			{ 
				Result = user 
			};
	}

	public async Task<IRequestResult> CreateWithToken(string token, UserSchema schema)
	{
		var request = VerifyRequest(schema);
		if (request is ResourceRequestError error)
		{
			return error;
		}
		var req = (ResourceActionRequest)request;
		var user = ResourceManager.GetSingle(x => x.AuthData.Username == schema.Username || x.PersonalData.Email == schema.Email);
		if (user == null)
		{
			return ResourceRequestError.BadRequest();
		}
		// Check if this is a temporary user (e.g. a new admin activating an email invite to administrate a resource)
		var tempToken = user.TokenCollection.GetSingleToken<TemporaryUserToken>(Context);
		if (tempToken == null || !PasswordHasher.VerifyHashedPassword(tempToken.TokenChallenge, token))
		{
			return ResourceRequestError.Conflict();
		}
		if (UserManager.GetSingle(u => u.AuthData.Username == schema.Username) != null)
		{
			return ResourceRequestError.Conflict();
		}
		using var rscLock = new ResourceLock(user.Guid);
		user.AuthData.ChangePassword(new Passphrase(tempToken.Password), new Passphrase(schema.Password));
		user.AuthData.Username = schema.Username;
		user.Name = schema.Name;
		if (!user.Verify(out var verificationError))
		{
			return ResourceRequestError.BadRequest(verificationError);
		}
		tempToken.Redeem(default);
		UserManager.Update(user, rscLock);
		SendEmailVerification(new AccessContext(user, user.AuthData.PassPhrase.GetValue(schema.Password)));
		req.Result = user;
		return req;
	}

	private static void SendEmailVerification(AccessContext context)
	{
		var token = context.User.TokenCollection.GetSingleToken<VerifyEmailToken>(context);
		if (token == null)
		{
			Logger.Error($"Couldn't send email verification to {context.User} - user was already verified.");
			return;
		}
		if (token.IsRedeemed)
		{
			Logger.Error($"Couldn't send email verification to {context.User} - user was already verified.");
			return;
		}
		EmailHelper.SendEmailVerificationEmail(context.User.PersonalData.Email, context.User.Name,
			$"{Dodo.DodoApp.NetConfig.FullURI}/{RootURL}/{VERIFY_EMAIL}?token={token.Token}");
	}

	public static User CreateTemporaryUser(string email)
	{
		var temporaryPassword = new Passphrase(ValidationExtensions.GenerateStrongPassword());
		var schema = new UserSchema("TEMPORARY", Guid.NewGuid().ToString().Replace("-", ""),
			temporaryPassword.Value, email);
		var factory = ResourceUtility.GetFactory<User>();
		var newUser = factory.CreateTypedObject(new ResourceCreationRequest(default, schema));
		var token = KeyGenerator.GetUniqueKey(32);
		var tokenChallenge = PasswordHasher.HashPassword(token);
		using (var rscLock = new ResourceLock(newUser))
		{
			newUser.TokenCollection.Add(newUser, new TemporaryUserToken(temporaryPassword, tokenChallenge));
			ResourceUtility.GetManager<User>().Update(newUser, rscLock);
		}
		EmailHelper.SendEmail(email, "New Rebel",
			$"You've been invited to create an account on {Dodo.DodoApp.PRODUCT_NAME}",
			$"To create your account, please following the following link:\n\n{Dodo.DodoApp.NetConfig.FullURI}/{RootURL}/{REGISTER}?token={token}");
		return newUser;
	}
}
