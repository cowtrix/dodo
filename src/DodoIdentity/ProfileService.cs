using Dodo.Users;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DodoIdentity
{
	public class ProfileService : IProfileService
	{
		protected UserManager<User> _userManager;

		public ProfileService(UserManager<User> userManager)
		{
			_userManager = userManager;
		}

		public async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			//>Processing
			var user = await _userManager.GetUserAsync(context.Subject);

			var claims = new List<Claim>
			{
				new Claim("Username", user.AuthData.Username),
			};

			context.IssuedClaims.AddRange(claims);
		}

		public async Task IsActiveAsync(IsActiveContext context)
		{
			//>Processing
			var user = await _userManager.GetUserAsync(context.Subject);

			context.IsActive = (user != null);
		}
	}
}