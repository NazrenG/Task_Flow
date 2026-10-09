using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class WorkMappingExtensions
    {
        public static WorkDto ToWorkDto(this Work work, string userId)
        {
            return new WorkDto
            {
                Id = work.Id,
                CreatedById = userId,
                Description = work.Description,
                Deadline = work.Deadline,
                Priority = work.Priority,
                Status = work.Status,
                Title = work.Title,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                StartDate = work.StartTime,
                Color = work.Color,
                CanbanColumnId = work.CanbanColumnId
            };
        }

        // Profil səhifəsi üçün: Id və Color göndərilmir
        public static WorkDto ToProfileWorkDto(this Work work, string userId)
        {
            return new WorkDto
            {
                CreatedById = userId,
                Description = work.Description,
                Deadline = work.Deadline,
                Priority = work.Priority,
                Status = work.Status,
                Title = work.Title,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                StartDate = work.StartTime,
                CanbanColumnId = work.CanbanColumnId
            };
        }

        // Tək task üçün: Id göndərilmir
        public static WorkDto ToSingleWorkDto(this Work work, string userId)
        {
            var dto = work.ToWorkDto(userId);
            dto.Id = default;
            return dto;
        }

        public static WorkDetailsDto ToWorkDetailsDto(this Work work)
        {
            return new WorkDetailsDto
            {
                TaskId = work.Id,
                ProjectId = work.ProjectId,
                ProjectName = work.Project?.Title,
                MemberName = $"{work.CreatedBy?.Firstname} {work.CreatedBy?.Lastname}",
                MemberImage = work.CreatedBy?.Image,
                MemberMail = work.CreatedBy?.Email,
                TaskTitle = work.Title,
                StartTime = work.StartTime ?? DateTime.Now,
                Deadline = work.Deadline,
                Status = work.Status,
                Priority = work.Priority
            };
        }

        public static object ToFullWorkDetail(this Work work, Project project, CustomUser createdBy)
        {
            return new
            {
                Description = work.Description,
                Deadline = work.Deadline,
                Priority = work.Priority,
                Status = work.Status,
                Title = work.Title,
                StartDate = work.StartTime,
                Color = work.Color,
                CanbanColumnId = work.CanbanColumnId,
                gitHubBranchName = work.GitHubBranchName,
                project = new
                {
                    id = work.ProjectId,
                    title = project.Title,
                    gitHubRepositoryUrl = project.GitHubRepositoryUrl,
                    gitHubRepositoryName = project.GitHubRepositoryName
                },
                createdBy = new
                {
                    id = createdBy.Id,
                    username = createdBy.UserName,
                    firstname = createdBy.Firstname,
                    lastname = createdBy.Lastname
                }
            };
        }

        public static Work ToNewWork(this WorkDto dto, string branchName)
        {
            return new Work
            {
                CreatedById = dto.CreatedById,
                Description = dto.Description,
                Deadline = dto.Deadline,
                Priority = dto.Priority,
                Status = dto.Status,
                Title = dto.Title,
                Color = dto.Color,
                ProjectId = dto.ProjectId,
                CanbanColumnId = dto.CanbanColumnId,
                GitHubBranchName = branchName,
                SprintId = dto.SprintId
            };
        }

        public static void ApplyPmEdit(this Work work, WorkDto dto)
        {
            work.Title = dto.Title;
            work.Description = dto.Description;
            work.Deadline = dto.Deadline;
            work.Color = dto.Color;
            work.CreatedById = dto.CreatedById;
            work.StartTime = dto.StartDate;
            work.Priority = dto.Priority;
            work.Status = dto.Status;
            work.CanbanColumnId = dto.CanbanColumnId;
        }
    }
}
