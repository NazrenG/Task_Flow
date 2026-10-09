namespace Task_Flow.WebAPI.Services.Notifications
{
    public static class RealtimeGuard
    {
        // SignalR xətaları əsas əməliyyatı dayandırmamalıdır
        public static async Task RunSafelyAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR error: {ex.Message}");
            }
        }
    }
}
