namespace Task_Flow.WebAPI.Dtos
{
    public class AddCompanyWorkerDto
    {
            public string WorkerId {  get; set; }
            public int CompanyId {  get; set; }
            public string Occupation {  get; set; }
            public int Role { get; set; }
    }
}
