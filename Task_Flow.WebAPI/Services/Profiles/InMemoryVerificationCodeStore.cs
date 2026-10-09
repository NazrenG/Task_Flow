using System.Collections.Concurrent;

namespace Task_Flow.WebAPI.Services.Profiles
{
    /// <summary>
    /// Kodları proses yaddaşında saxlayır (Singleton kimi qeyd olunur).
    /// Əvvəlki static Dictionary-nin thread-safe əvəzidir.
    /// </summary>
    public class InMemoryVerificationCodeStore : IVerificationCodeStore
    {
        private readonly ConcurrentDictionary<string, int> _codes = new();

        public void Save(string email, int code) => _codes[email] = code;

        public bool Contains(string email) => _codes.ContainsKey(email);

        public void Remove(string email) => _codes.TryRemove(email, out _);
    }
}
