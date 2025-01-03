﻿using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
    public interface IMessageDal:IEntityRepository<Message>
    {
        Task<List<Message>> GetMessages();
    }
}
