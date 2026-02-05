namespace Task_Flow.WebAPI.Dtos
{
    public class UpdateTaskColumnDto
    {
        public int TaskId { get; set; }
        public int NewCanbanColumnId { get; set; }
        public string Status {  get; set; }
    }
}
