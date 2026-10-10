using Task_Flow.DataAccess.Abstract;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Comments
{
    public class CommentQueryService : ICommentQueryService
    {
        private readonly ICommentService _commentService;

        public CommentQueryService(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<ServiceResult<List<CommentDto>>> GetAllAsync()
        {
            var comments = await _commentService.GetComments();
            return ServiceResult<List<CommentDto>>.Success(comments.Select(c => c.ToCommentDto()).ToList());
        }

        public async Task<ServiceResult<int>> GetCountAsync()
        {
            return ServiceResult<int>.Success(await _commentService.GetCount());
        }

        public async Task<ServiceResult<CommentDto>> GetByIdAsync(int id)
        {
            var comment = await _commentService.GetCommentById(id);
            if (comment == null)
            {
                return ServiceResult<CommentDto>.NotFound();
            }

            return ServiceResult<CommentDto>.Success(comment.ToCommentDto());
        }
    }
}
