using Task_Flow.Business.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Messages
{
    public class MessageAppService : IMessageAppService
    {
        private readonly IMessageService _messageService;

        public MessageAppService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<ServiceResult<List<object>>> GetReceivedMessagesAsync(string userId)
        {
            var messages = await _messageService.GetMessages();
            if (messages == null)
            {
                return ServiceResult<List<object>>.NotFound();
            }

            var received = messages
                .Where(m => m.ReceiverId == userId)
                .Select(m => (object)new
                {
                    ReceiverName = m.Receiver?.UserName,
                    SenderName = m.Sender?.UserName,
                    Path = m.Sender?.Image,
                    Text = m.Text,
                    SentDate = m.SentDate
                })
                .ToList();

            return ServiceResult<List<object>>.Success(received);
        }

        public async Task<ServiceResult<Message>> AddAsync(MessageDto value)
        {
            var message = new Message
            {
                ReceiverId = value.ReceiverId,
                SenderId = value.SenderId,
                Text = value.Text,
                SentDate = DateTime.Now
            };
            await _messageService.Add(message);

            return ServiceResult<Message>.Success(message);
        }

        public async Task<ServiceResult<Empty>> DeleteAsync(int id)
        {
            var message = await _messageService.GetMessageById(id);
            if (message == null)
            {
                return ServiceResult<Empty>.NotFound();
            }

            await _messageService.Delete(message);
            return ServiceResult<Empty>.Success(Empty.Value);
        }
    }
}
