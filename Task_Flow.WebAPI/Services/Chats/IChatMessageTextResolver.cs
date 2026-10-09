using Task_Flow.Entities.Models;

namespace Task_Flow.WebAPI.Services.Chats
{
    /// <summary>
    /// Mesajın istifadəçiyə göstəriləcək mətnini müəyyən edir (silinmiş, şifrələnmiş və ya köhnə mesaj).
    /// </summary>
    public interface IChatMessageTextResolver
    {
        string Resolve(ChatMessage message);
    }
}
