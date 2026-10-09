using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Services.Mapping
{
    public static class ProjectMappingExtensions
    {
        private const string DoneStatus = "done";

        public static ExtendedProjectListDto ToExtendedProjectListDto(this Project project)
        {
            return new ExtendedProjectListDto
            {
                Id = project.Id,
                EndDate = project.EndDate,
                StartDate = project.StartDate,
                Title = project.Title,
                Deadline = project.EndDate,
                TotalTask = project.TaskForUsers!.Count,
                CompletedTask = project.TaskForUsers!.Count(t => t.Status == DoneStatus),
                ParticipantsPath = project.TeamMembers!.Select(tm => tm.User!.Image!).ToList(),
                Color = project.Color
            };
        }

        // Dashboard-dakı "iştirak etdiyim layihələr" üçün: tarixlər göndərilmir
        public static ExtendedProjectListDto ToInvolvedProjectDto(this Project project)
        {
            return new ExtendedProjectListDto
            {
                Id = project.Id,
                Title = project.Title,
                TotalTask = project.TaskForUsers!.Count,
                CompletedTask = project.TaskForUsers!.Count(t => t.Status == DoneStatus),
                ParticipantsPath = project.TeamMembers!.Select(tm => tm.User!.Image!).ToList(),
                Color = project.Color
            };
        }

        public static object ToOnGoingProjectItem(this Project project)
        {
            return new
            {
                Title = project.Title,
                EndDate = project.EndDate,
                StartDate = project.StartDate,
                MembersPath = project.TeamMembers!.Select(tm => tm.User?.Image).ToList(),
                Color = project.Color
            };
        }

        public static ProjectDto ToProjectDto(this Project project, bool isCompanyProject)
        {
            return new ProjectDto
            {
                Owner = project.CreatedBy?.UserName,
                OwnerMail = project.CreatedBy?.Email,
                IsCompleted = project.IsCompleted,
                Description = project.Description,
                Title = project.Title,
                Color = project.Color,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                IsCompanyProject = isCompanyProject
            };
        }

        public static Project ToNewProject(this CreateProjectDto dto, string userId)
        {
            return new Project
            {
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted,
                Title = dto.Title,
                Color = dto.Color,
                GitHubRepositoryName = dto.GitHubRepositoryName,
                GitHubRepositoryUrl = dto.GitHubRepositoryUrl
            };
        }

        public static void ApplyUpdate(this Project project, PutProjectDto dto)
        {
            project.Title = dto.Title;
            project.Color = dto.Color;
            project.Description = dto.Description;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
        }

        public static CanbanTaskDto ToCanbanTaskDto(this Work work)
        {
            return new CanbanTaskDto
            {
                Id = work.Id,
                CreatedById = work.CreatedById,
                Description = work.Description,
                Deadline = work.Deadline,
                Priority = work.Priority,
                Status = work.Status,
                Title = work.Title,
                StartDate = work.StartTime,
                Color = work.Color,
                CanbanColumnId = work.CanbanColumnId,
                SprintId = work.SprintId,
                ParticipantId = work.CreatedById,
                ParticipantPath = work.CreatedBy?.Image,
                ParticipantName = work.CreatedBy != null
                    ? $"{work.CreatedBy.Firstname} {work.CreatedBy.Lastname}"
                    : "Unknown Participant",
                ParticipantEmail = work.CreatedBy?.Email ?? "unknown@example.com"
            };
        }
    }
}
