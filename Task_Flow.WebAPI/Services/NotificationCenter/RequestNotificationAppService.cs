using Microsoft.AspNetCore.Identity;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
using Task_Flow.WebAPI.Services.Mapping;
using Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.NotificationCenter
{
    public class RequestNotificationAppService : IRequestNotificationAppService
    {
        private const int LatestCount = 2;
        private const string CompanyWorkerRequestText = "Company Worker Request";

        private readonly IRequestNotificationService _requestNotificationService;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IPremiumUserService _premiumUserService;
        private readonly MailService _mailService;
        private readonly INotificationRealtimeNotifier _notifier;
        private readonly IRecentActivityAppService _recentActivityAppService;
        private readonly Dictionary<string, IRequestAcceptHandler> _acceptHandlers;

        public RequestNotificationAppService(
            IRequestNotificationService requestNotificationService,
            IUserService userService,
            UserManager<CustomUser> userManager,
            IPremiumUserService premiumUserService,
            MailService mailService,
            INotificationRealtimeNotifier notifier,
            IRecentActivityAppService recentActivityAppService,
            IEnumerable<IRequestAcceptHandler> acceptHandlers)
        {
            _requestNotificationService = requestNotificationService;
            _userService = userService;
            _userManager = userManager;
            _premiumUserService = premiumUserService;
            _mailService = mailService;
            _notifier = notifier;
            _recentActivityAppService = recentActivityAppService;
            _acceptHandlers = acceptHandlers.ToDictionary(h => h.NotificationType);
        }

        public async Task<ServiceResult<List<object>>> GetPendingSummariesAsync(string userId)
        {
            var pending = await GetPendingAsync(userId);
            return ServiceResult<List<object>>.Success(pending.Select(r => r.ToSenderSummary()).ToList());
        }

        public async Task<ServiceResult<List<object>>> GetLatestPendingSummariesAsync(string userId)
        {
            var pending = await GetPendingAsync(userId);
            var latest = pending
                .OrderByDescending(r => r.Id)
                .Take(LatestCount)
                .Select(r => r.ToSenderSummary())
                .ToList();

            return ServiceResult<List<object>>.Success(latest);
        }

        public async Task<ServiceResult<List<object>>> GetPendingRequestsAsync(string userId)
        {
            var pending = await GetPendingAsync(userId);
            return ServiceResult<List<object>>.Success(pending.Select(r => r.ToRequestItem()).ToList());
        }

        public async Task<ServiceResult<int>> GetPendingCountAsync(string? userId)
        {
            var pending = await GetPendingAsync(userId);
            return ServiceResult<int>.Success(pending.Count);
        }

        public async Task<ServiceResult<object>> SendRequestAsync(string userId, RequestNotificationDto dto)
        {
            var isFriendRequest = dto.NotificationType == RequestNotificationTypes.FriendRequest;

            if (isFriendRequest)
            {
                var permission = await _premiumUserService.IsUserAllowedToSendRequestAsync(userId);
                if (!permission.Allowed)
                {
                    return ServiceResult<object>.Success(new { message = permission.Message, allowed = false });
                }
            }

            var sender = await _userService.GetUserById(userId);
            var receiver = await _userManager.FindByEmailAsync(dto.ReceiverEmail!);
            if (receiver == null)
            {
                return ServiceResult<object>.BadRequest(new { message = "Receiver not found" });
            }

            var request = new RequestNotification
            {
                Text = dto.Text,
                SenderId = userId,
                ReceiverId = receiver.Id,
                IsAccepted = dto.IsAccepted,
                NotificationType = dto.NotificationType!
            };
            await _requestNotificationService.Add(request);

            // notification list
            await _notifier.NotifyRequestListsAsync(receiver.Id);
            await _notifier.NotifyFollowRequestSentAsync(sender.Id, receiver.Id);

            var mailText = isFriendRequest
                ? $"You have new friendship request to {sender.Firstname} {sender.Lastname} "
                : $"You have a new project proposal from {sender.Firstname} {sender.Lastname}";
            _mailService.SendEmail(receiver.Email!, mailText);

            return ServiceResult<object>.Success(new
            {
                message = "Activity added successfully",
                data = new RequestNotificationDto
                {
                    Text = request.Text,
                    ReceiverEmail = receiver.Email
                },
                allowed = true
            });
        }

        public async Task<ServiceResult<object>> SendCompanyWorkerRequestsAsync(string userId, CompanyRequestDto dto)
        {
            var sender = await _userService.GetUserById(userId);

            foreach (var receiverId in dto.UserIds)
            {
                var receiver = await _userManager.FindByIdAsync(receiverId);
                if (receiver == null)
                {
                    return ServiceResult<object>.BadRequest(new { message = "Receiver not found" });
                }

                await _requestNotificationService.Add(new RequestNotification
                {
                    Text = CompanyWorkerRequestText,
                    SenderId = userId,
                    ReceiverId = receiverId,
                    IsAccepted = false,
                    NotificationType = RequestNotificationTypes.CompanyWorkerRequest
                });

                // notification list
                await _notifier.NotifyRequestListsAsync(receiverId);
                _mailService.SendEmail(receiver.Email!,
                    $"You have new Company Worker request from {sender.Firstname} {sender.Lastname} ");
            }

            return ServiceResult<object>.Success(new { message = "Request sent successfully!" });
        }

        public async Task<ServiceResult<object>> DeleteRequestAsync(int requestId, string userId)
        {
            var request = await _requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null)
            {
                return ServiceResult<object>.BadRequest(new { message = "request not found" });
            }

            await _requestNotificationService.Delete(request);
            await _notifier.NotifyRequestListsAsync(userId);
            await _recentActivityAppService.LogNotificationActivityAsync(userId, "Delete request");

            return ServiceResult<object>.Success(new { message = "delete request notification succesfully" });
        }

        public async Task<ServiceResult<object>> AcceptRequestAsync(int requestId, string userId)
        {
            var request = await _requestNotificationService.GetRequestNotificationById(requestId);
            if (request == null)
            {
                return ServiceResult<object>.BadRequest(new { message = "request not found" });
            }

            request.IsAccepted = true;
            await _requestNotificationService.Update(request);

            if (request.NotificationType != null &&
                _acceptHandlers.TryGetValue(request.NotificationType, out var handler))
            {
                await handler.HandleAsync(request, userId);
            }

            await _notifier.NotifyUserActivityAsync(request.SenderId!);
            await _notifier.NotifyRequestListsAsync(userId);
            await _recentActivityAppService.LogNotificationActivityAsync(userId, "Accept request");

            return ServiceResult<object>.Success(new
            {
                message = "accept request succesfuly",
                notificationType = request.NotificationType
            });
        }

        private async Task<List<RequestNotification>> GetPendingAsync(string? userId)
        {
            var requests = await _requestNotificationService.GetRequestNotifications(userId!);
            return requests.Where(r => !r.IsAccepted).ToList();
        }
    }
}
