using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class CommentMappingExtensions
    {
        public static CommentDto ToCommentDto(this Comment comment)
        {
            return new CommentDto
            {
                Context = comment.Context,
                TaskForUserId = comment.TaskForUserId,
                UserId = comment.UserId
            };
        }

        public static Comment ToComment(this CommentDto dto)
        {
            return new Comment
            {
                Context = dto.Context,
                TaskForUserId = dto.TaskForUserId,
                UserId = dto.UserId
            };
        }
    }
}
