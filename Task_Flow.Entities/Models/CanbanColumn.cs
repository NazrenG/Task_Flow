
using Task_Flow.Core.Abstract;

namespace Task_Flow.Entities.Models
{
    public class CanbanColumn : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public string StatusKey { get; set; } = null!;
        // "to-do", "in-progress", "review"

        public int Order { get; set; }

        public bool IsFixed { get; set; }

        public Project Project { get; set; }
        public List<Work> TaskForUsers { get; set; }
    }
}
