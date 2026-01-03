using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatMessageController : ControllerBase
    {
        private readonly IHubContext<ConnectionHub> _hub;
        private readonly IChatService _chatService;
        private readonly IChatMessageService _chatMessageService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly MessageEncryptionService encryptionService;

        public ChatMessageController(MessageEncryptionService encryptionService, IHubContext<ConnectionHub> hub, IChatService chatService, IChatMessageService chatMessageService, IUserService userService, UserManager<CustomUser> userManager)
        {
            _hub = hub;
            _chatService = chatService;
            _chatMessageService = chatMessageService;
            _userService = userService;
            _userManager = userManager;
            this.encryptionService = encryptionService;
        }

        [Authorize]
        [HttpPost("NewMessage")]
        public async Task<IActionResult> Post([FromBody] ChatMessageDto dto)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var encrypted = encryptionService.Encrypt(dto.Text);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token or user not found.");
            }
            var friend = await _userManager.FindByEmailAsync(dto.FriendEmail);
            var chat = await _chatService.GetByRecieverAndSenderId(friend.Id, userId);
            //var messageList = new List<ChatMessage>();
            //else
            //{
            //    messageList = await _chatMessageService.GetAllByChatId(chat.Id);
            //}

            var message = new ChatMessage { Content = encrypted.CipherText, IV = encrypted.IV, SenderId = userId, SentDate = DateTime.UtcNow, ChatId = chat.Id ,IsImage=dto.IsImage};

            await _chatMessageService.AddAsync(message);
            await _hub.Clients.User(userId).SendAsync("ReceiveMessages2",friend.Email);

            return Ok(new {SenderId=userId,FriendId=friend.Id});
        }

        //[Authorize]
        //[HttpPost("NewEmoji")]
        //public async Task<IActionResult> AddEmoji([FromBody] ChatMessageDto dto)
        //{

        //    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized("Invalid token or user not found.");
        //    }
        //    var friend = await _userManager.FindByEmailAsync(dto.FriendEmail);
        //    var chat = await _chatService.GetByRecieverAndSenderId(friend.Id, userId);
        //    //var messageList = new List<ChatMessage>();
        //    //else
        //    //{
        //    //    messageList = await _chatMessageService.GetAllByChatId(chat.Id);
        //    //}

        //    var message = new ChatMessage { Content = dto.Text, SenderId = userId, SentDate = DateTime.UtcNow, ChatId = chat.Id };

        //    await _chatMessageService.AddAsync(message);
        //    await _hub.Clients.User(friend.Id).SendAsync("UpdateChat");

        //    return Ok();
        //}

        [Authorize]
        [HttpGet("AllMessages/{friendMail}")]
        public async Task<IActionResult> Get(string friendMail)
        {

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token or user not found.");
            }
            if (string.IsNullOrEmpty(friendMail)) { return Ok(new {List=new List<UserMessageDto>()}); }
            var friend = await _userManager.FindByEmailAsync(friendMail);
            var chat = await _chatService.GetByRecieverAndSenderId(friend.Id, userId);
            if (chat == null) { chat = new Chat { SenderId = userId, ReceiverId = friend.Id, Messages = new List<ChatMessage>() }; await _chatService.AddAsync(chat); }
            var allMessages = await _chatMessageService.GetAllByChatId(chat.Id);
            var dtoList = new List<UserMessageDto>();
            string decryptedMessage = "";


            foreach (var message in allMessages)
            {
                var sender = await _userService.GetUserById(message.SenderId);
                if (message.Status == "Deleted")
    {
        decryptedMessage = "This message was deleted!";
    }
    else if (!string.IsNullOrEmpty(message.IV))
    {
        // encrypted message
        decryptedMessage = encryptionService.Decrypt(
            message.Content,
            message.IV
        );
    }
    else
    {
        // old messages (before encryption)
        decryptedMessage = message.Content;
    }
                dtoList.Add(new UserMessageDto
                {
                    IsOnline = sender.IsOnline,
                    IsSender = sender.Id == userId,
                    Fullname = sender.Firstname + " " + sender.Lastname,
                    Message = decryptedMessage,
                    Photo = sender.Image,
                    Status=message.Status,
                    SentDate = message.SentDate,
                    MessageId = message.Id,
                });
            }
            return Ok(new { List = dtoList });

        }


        //[Authorize]
        [HttpDelete("RemoveMessage/{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var message = await _chatMessageService.GetAsync(id);
            // message = new ChatMessage
            // {
            //     Id=message.Id,
            //     Content="",
            //Status = "Deleted",
            //HasSeen = false,
            //IsImage=false,
            //SentDate=message.SentDate,
            //SenderId = message.SenderId,
            //ChatId = message.ChatId,

            // };
            message.Status = "Deleted";
            await _chatMessageService.UpdateAsync(message);
            return Ok();
        
        }
    }
}
