using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class TeamMemberMappingExtensions
    {
        public static TeamMemberDto ToTeamMemberDto(this TeamMember member)
        {
            return new TeamMemberDto
            {
                ProjectId = member.ProjectId,
                UserId = member.UserId
            };
        }

        public static TeamUserDto ToTeamUserDto(this TeamMember member)
        {
            var user = member.User!;
            return new TeamUserDto
            {
                Id = member.UserId!,
                Name = $"{user.Firstname} {user.Lastname}",
                Phone = user.PhoneNumber!,
                Occupation = user.Occupation!,
                Email = user.Email!,
                Photo = user.Image!,
                IsOnline = user.IsOnline
            };
        }

        // isRequest: dəvət hələ göndərilib (true) və ya artıq komanda üzvüdür (false)
        public static ExtendedTeamMemberDto ToExtendedTeamMemberDto(this CustomUser user, bool isRequest, bool isAccepted)
        {
            return new ExtendedTeamMemberDto
            {
                Username = user.UserName!,
                Firstname = user.Firstname!,
                Lastname = user.Lastname!,
                ImgPath = user.Image!,
                IsRequest = isRequest,
                IsAccepted = isAccepted
            };
        }
    }
}
