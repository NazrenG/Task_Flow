namespace Task_Flow.WebAPI.Dtos
{
    public class CreateProjectDto:ProjectDto
    {
        public string GitHubRepositoryUrl { get; set; }
        public string GitHubRepositoryName { get; set; }
    }
}
