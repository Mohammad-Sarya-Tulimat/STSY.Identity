using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Models;
using System.Collections.Generic;
using System.Linq;

namespace STSY.Microsoft.Identity.Mappers
{
    public static class UserMapper
    {
        public static ExtendedUser ToExtendedUser(this MicrosoftIdentityUser user, List<string> roles)
        {
            return new ExtendedUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsEmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };
        }

        public static UserData ToUserData(this MicrosoftIdentityUser user)
        {
            return new UserData
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }
        public static IQueryable<UserData> AsUserData(this IQueryable<MicrosoftIdentityUser> users)
        {
            return users.Select(user => new UserData
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            });
        }

        public static UserPassKey ToUserPassKey(this UserPasskeyInfo passkeyInfo)
        {
            return new UserPassKey
            {
                Id = passkeyInfo.CredentialId,
                Name = passkeyInfo.Name,
                CreatedAt = passkeyInfo.CreatedAt,
            };
        }
    }
}
