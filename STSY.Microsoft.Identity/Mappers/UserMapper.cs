using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Models;
using System;
using System.Linq;

namespace STSY.Microsoft.Identity.Mappers
{
    public static class UserMapper
    {

        public static UserData ToUserData(this MicrosoftIdentityUser user)
        {
            if (user == null) return null;
            return new UserData
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ImageReference = user.ImageReference,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed,
            };
        }

        public static UserData UpdateUserData(this MicrosoftIdentityUser user, UserData target)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (target == null) throw new ArgumentNullException(nameof(target));
            target.Id = user.Id;
            target.UserName = user.UserName;
            target.Email = user.Email;
            target.CreatedAt = user.CreatedAt;
            target.FirstName = user.FirstName;
            target.LastName = user.LastName;
            target.PhoneNumber = user.PhoneNumber;
            target.DateOfBirth = user.DateOfBirth;
            target.ImageReference = user.ImageReference;
            target.IsEmailConfirmed = user.EmailConfirmed;
            target.IsPhoneNumberConfirmed = user.PhoneNumberConfirmed;
            return target;
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
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                ImageReference = user.ImageReference,
                IsEmailConfirmed = user.EmailConfirmed,
                IsPhoneNumberConfirmed = user.PhoneNumberConfirmed
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
