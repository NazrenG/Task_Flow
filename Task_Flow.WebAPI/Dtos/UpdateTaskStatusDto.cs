namespace Task_Flow.WebAPI.Dtos
{
    public class UpdateTaskStatusDto
    {
        public int NewCanbanColumnId { get; set; }
        public string NewStatus { get; set; }
    }
}