using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    // Project ve User class-inin coxun coxa relation-undan emele gelen 3-cu class-dir.

    public class TeamMember:IEntity
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        public string? UserId { get; set; }
        public string? Role { get; set; }
        public bool GitHubAccessGranted { get; set; }

        ///
        public virtual CustomUser? User { get; set; }
        public virtual Project? Project { get; set; }
    }
}
