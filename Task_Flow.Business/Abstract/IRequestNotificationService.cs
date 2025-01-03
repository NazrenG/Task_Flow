﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Flow.Entities.Models;

namespace Task_Flow.Business.Abstract
{
  public  interface IRequestNotificationService
    {
        Task<List<RequestNotification>> GetRequestNotifications(string receiverId);
        Task<RequestNotification> GetRequestNotificationById(int id);
        Task Add(RequestNotification requestNotification);
        Task<List<RequestNotification>> GetNotificationsBySenderId(string senderId);
        Task<List<RequestNotification>> GetNotificationsByProjectName(string projectName);

        Task Update(RequestNotification requestNotification);
        Task<List<RequestNotification>> GetAll();
        Task Delete(RequestNotification requestNotification);
    }
}
