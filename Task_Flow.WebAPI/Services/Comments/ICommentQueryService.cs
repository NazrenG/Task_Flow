using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Comments
{
    /// <summary>
    /// Task şərhlərini oxuyan əməliyyatlar.
    /// </summary>
    public interface ICommentQueryService
    {
        Task<ServiceResult<List<CommentDto>>> GetAllAsync();
        Task<ServiceResult<int>> GetCountAsync();
        Task<ServiceResult<CommentDto>> GetByIdAsync(int id);
    }
}
