using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class ProfileMappingExtensions
    {
        public static object ToOwnProfile(this CustomUser user, string userId)
        {
            return new
            {
                userId = userId,
                userName = user.UserName,
                email = user.Email,
                firstname = user.Firstname,
                lastname = user.Lastname,
                image = user.Image,
                gitHubAccessToken = user.GitHubAccessToken,
                gitHubUsername = user.GitHubUsername
            };
        }

        public static object ToPublicProfile(this CustomUser user)
        {
            return new
            {
                Username = user.UserName,
                Firstname = user.Firstname,
                Fullname = user.Firstname + " " + user.Lastname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Gender = user.Gender,
                Country = user.Country,
                Birthday = user.Birthday,
                Email = user.Email,
                Path = user.Image,
                Occupation = user.Occupation
            };
        }

        public static object ToBasicProfileInfo(this CustomUser user)
        {
            return new
            {
                Username = user.UserName,
                Firstname = user.Firstname,
                Fullname = user.Firstname + " " + user.Lastname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Gender = user.Gender,
                Country = user.Country,
                Birthday = user.Birthday,
                Email = user.Email,
                Path = user.Image,
                Occupation = user.Occupation,
                RegisterDate = user.RegisterDate,
                IsOnline = user.IsOnline
            };
        }

        public static object ToCurrentUserData(this CustomUser user)
        {
            return new
            {
                Username = user.UserName,
                Firstname = user.Firstname,
                Fullname = user.Firstname + " " + user.Lastname,
                Lastname = user.Lastname,
                Phone = user.PhoneNumber,
                Gender = user.Gender,
                Country = user.Country,
                Birthday = user.Birthday,
                Email = user.Email,
                Image = user.Image,
                PlanType = user.PlanType,
                Occupation = user.Occupation
            };
        }

        public static void ApplyProfileEdit(this CustomUser user, UserDto dto)
        {
            // "Ad Soyad" formatındakı Fullname ad və soyada bölünür
            var nameParts = dto.Fullname?.Split(" ");
            user.Firstname = nameParts != null && nameParts.Length > 0 ? nameParts[0] : user.Firstname;
            user.Lastname = nameParts != null && nameParts.Length > 1 ? nameParts[1] : user.Lastname;

            user.Birthday = dto.Birthday;
            user.Email = dto.Email;
            user.Country = dto.Country;
            user.PhoneNumber = dto.Phone;
            user.Occupation = dto.Occupation;
            user.Gender = dto.Gender;
        }
    }
}
