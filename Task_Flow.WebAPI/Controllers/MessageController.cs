using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;
        private readonly IChatService chatService;
        private readonly IChatMessageService chatMessageService;
        public MessageController(IMessageService messageService, IChatService chatService, IChatMessageService chatMessageService, IUserService userService)
        {
            this.messageService = messageService;
            this.chatService = chatService;
            this.chatMessageService = chatMessageService;
            this.userService = userService;
        }

        //GET: api/<MessageController>
        [Authorize]
        [HttpGet("UserMessage")]
        public async Task<IActionResult> Get()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }


            var list = await messageService.GetMessages();
            if (list == null) return NotFound();

            var items = list.Where(i => i.ReceiverId == userId).Select(c =>
            {
                return new
                {
                    ReceiverName = c.Receiver?.UserName,
                    SenderName = c.Sender?.UserName,
                    Path = c.Sender?.Image,
                    Text = c.Text,
                    SentDate = c.SentDate,
                };

            });
            return Ok(items);
        }
        ///
        [Authorize]
        [HttpGet("TwoMessage")]
        public async Task<IActionResult> TakeTwoMessage()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest("User not authenticated.");
            }
            var chats = await chatService.GetAllChatByUserId(userId);
         var list =new List<ChatMessage>();
            foreach (var item in chats)
            {
                var recentmessage = await chatMessageService.GetLatestMessageByChatIdAsync(item.Id);
               
               if(recentmessage!=null) list.Add(recentmessage);
            }

            var dtos = new List<MessageListDisplayer>();
            foreach (var item in list)
            {
                
                var sender = await userService.GetUserById(item.SenderId);

               dtos.Add(new MessageListDisplayer
                {
                   Sender=sender.Firstname+" "+sender.Lastname,
                   Content=item.Content,
                   SentDate=item.SentDate.ToShortDateString(),
                   Image=sender.Image,

                });
            }

          


            return Ok(new {Dtos=dtos});
        }




        [Authorize]
        //userin bildirim sayi
        [HttpGet("UserMessageCount")]
        public async Task<IActionResult> GetCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var chats =await chatService.GetAllChatByUserId(userId);
            var count = 0;
            foreach (var item in chats)
            {
                var recentmessage =await chatMessageService.GetLatestMessageByChatIdAsync(item.Id);
                if(recentmessage!=null)count++;

            }

            //var list = await messageService.GetMessages();

            return Ok(new {MessCount=count});
        }



        // POST api/<MessageController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageDto value)
        {
          
            var item = new Message
            {
                ReceiverId = value.ReceiverId,
                SenderId = value.SenderId,
                Text = value.Text,
               
                SentDate = DateTime.Now,
            };
            await messageService.Add(item);
            return Ok(item);
        }



        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await messageService.GetMessageById(id);
            if (item == null)
            {
                return NotFound();
            }
            await messageService.Delete(item);
            return Ok();
        }
    }
}
