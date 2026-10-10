using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Comments
{
    /// <summary>
    /// Task şərhlərini yaradan, dəyişən və silən əməliyyatlar.
    /// </summary>
    public interface ICommentCommandService
    {
        Task<ServiceResult<Comment>> AddAsync(CommentDto value);
        Task<ServiceResult<Empty>> ChangeContextAsync(int id, string context);
        Task<ServiceResult<Empty>> DeleteAsync(int id);
    }
}
