using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Comments
{
    public class CommentCommandService : ICommentCommandService
    {
        private readonly ICommentService _commentService;

        public CommentCommandService(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<ServiceResult<Comment>> AddAsync(CommentDto value)
        {
            var comment = value.ToComment();
            await _commentService.Add(comment);

            return ServiceResult<Comment>.Success(comment);
        }

        public async Task<ServiceResult<Empty>> ChangeContextAsync(int id, string context)
        {
            var comment = await _commentService.GetCommentById(id);
            if (comment == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            comment.Context = context;
            await _commentService.Update(comment);

            return ServiceResult<Empty>.Success(Empty.Value);
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            var comment = await _commentService.GetCommentById(id);
            if (comment == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            await _commentService.Delete(comment);

            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
