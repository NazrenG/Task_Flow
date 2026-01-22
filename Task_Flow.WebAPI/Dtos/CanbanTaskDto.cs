namespace Task_Flow.WebAPI.Dtos
{
    public class CanbanTaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTime Deadline { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }

        public string? Priority { get; set; }// Urgent, Primary, Simple
        public int ProjectId { get; set; }
        public string? Color { get; set; }
        public int TotalTask { get; set; }
        public int CompletedTask { get; set; }
        public string? ParticipantPath { get; set; }
        public string? ParticipantName{ get; set; }
        public string? ParticipantEmail { get; set; }
        public string? CreatedById { get; set; }//userId
        public int? CanbanColumnId { get; set; }
        public int? SprintId { get; set; }
        public string? ParticipantId { get; set; }

    }
}
