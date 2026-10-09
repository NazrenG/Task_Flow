namespace Task_Flow.WebAPI.Services.Results
{
    /// <summary>
    /// Cavab gövdəsi olmayan uğurlu nəticə (HTTP 200, boş body).
    /// </summary>
    public sealed class Empty
    {
        public static readonly Empty Value = new();

        private Empty()
        {
        }
    }
}
