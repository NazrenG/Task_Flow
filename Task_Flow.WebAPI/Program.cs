using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using Task_Flow.Business.Abstract;
using Task_Flow.Business.Cocrete;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.DataAccess.Concrete;
using Task_Flow.Entities.Data;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Hubs;
using Task_Flow.WebAPI.Services.Auth;
using Task_Flow.WebAPI.Services.Chats;
using Task_Flow.WebAPI.Services.Companies;
using Task_Flow.WebAPI.Services.Friends;
using Task_Flow.WebAPI.Services.GroupChats;
using Task_Flow.WebAPI.Services;
using Task_Flow.WebAPI.Services.NotificationCenter;
using Task_Flow.WebAPI.Services.NotificationCenter.RequestAcceptance;
using Task_Flow.WebAPI.Services.Notifications;
using Task_Flow.WebAPI.Services.Profiles;
using Task_Flow.WebAPI.Services.Quizzes;
using Task_Flow.WebAPI.Services.TeamMembers;
using Task_Flow.WebAPI.Services.UserTasks;
using Task_Flow.WebAPI.Services.Projects;
using Task_Flow.WebAPI.Services.Works;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<GitHubSettings>(
    builder.Configuration.GetSection("GitHub")
    );


//stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSignalR();
// Database connection
builder.Services.AddDbContext<TaskFlowDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("SMTP.json", optional: false, reloadOnChange: true);


builder.Services.AddTransient<MailService>();
builder.Services.AddHostedService<ReminderService>();

builder.Services.AddScoped<IUserDal, UserDal>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentDal, CommentDal>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IQuizDal, QuizDal>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IProjectDal, ProjectDal>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskDal, TaskDal>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskCustomizeDal, TaskCustomizeDal>();
builder.Services.AddScoped<ITaskCustomizeService, TaskCustomizeService>();
builder.Services.AddScoped<ITeamMemberDal, TeamMemberDal>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<IMessageDal, MessageDal>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFriendDal, FriendDal>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IUserTaskDal, UserTaskDal>();
builder.Services.AddScoped<IUserTaskService, UserTaskService>();
builder.Services.AddScoped<INotificationDal, NotificationDal>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationSettingsDal,NotificationSettingDal>();  
builder.Services.AddScoped<INotificationSettingService, NotificationSettingService>();
builder.Services.AddScoped<IRecentActivityDal,RecentActivityDal>(); 
builder.Services.AddScoped<IRecentActivityService, RecentActivityService>();
builder.Services.AddScoped<IFileService, Task_Flow.Business.Cocrete.FileService>();
builder.Services.AddScoped<IRequestNotificationDal, RequestNotificationDal>();
builder.Services.AddScoped<IRequestNotificationService, RequestNotificationService>();
builder.Services.AddScoped<IProjectActivityDal, ProjectActivityDal>();
builder.Services.AddScoped<IProjectActivityService, ProjectActivityService>();
builder.Services.AddScoped<IChatDal, ChatDal>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatMessageDal,ChatMessageDal>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IGroupChatDal, GroupChatDal>();
builder.Services.AddScoped<IGroupChatMessageDal , GroupChatMessageDal>();   
builder.Services.AddScoped<IGroupChatMembersDal , GroupChatMembersDal>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddSingleton<MessageEncryptionService>();
builder.Services.AddScoped<IPremiumUserService, PremiumUserService>();
builder.Services.AddScoped<ICanbanColumnDal, CanbanColumnDal>();
builder.Services.AddScoped<ISprintDal, SprintDal>();
builder.Services.AddScoped<ISprintService, SprintService>();
builder.Services.AddScoped<ICanbanColumnService, CanbanColumnService>();
builder.Services.AddScoped<ICompanyDal, CompanyDal>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyWorkerDal, CompanyWorkerDal>();
builder.Services.AddScoped<ICompanyWorkerService, CompanyWorkerService>();
builder.Services.AddScoped<IWorkDal, WorkDal>();
builder.Services.AddHttpClient<IGitHubService,GitHubService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IWorkRealtimeNotifier, WorkRealtimeNotifier>();
builder.Services.AddScoped<IWorkQueryService, WorkQueryService>();
builder.Services.AddScoped<IWorkCommandService, WorkCommandService>();
builder.Services.AddScoped<IProjectRealtimeNotifier, ProjectRealtimeNotifier>();
builder.Services.AddScoped<IProjectQueryService, ProjectQueryService>();
builder.Services.AddScoped<IProjectCommandService, ProjectCommandService>();
builder.Services.AddScoped<INotificationRealtimeNotifier, NotificationRealtimeNotifier>();
builder.Services.AddScoped<ICalendarNotificationAppService, CalendarNotificationAppService>();
builder.Services.AddScoped<INotificationSettingAppService, NotificationSettingAppService>();
builder.Services.AddScoped<IRecentActivityAppService, RecentActivityAppService>();
builder.Services.AddScoped<IRequestNotificationAppService, RequestNotificationAppService>();
builder.Services.AddScoped<IRequestAcceptHandler, ProjectRequestAcceptHandler>();
builder.Services.AddScoped<IRequestAcceptHandler, FriendRequestAcceptHandler>();
builder.Services.AddScoped<IRequestAcceptHandler, CompanyWorkerRequestAcceptHandler>();
builder.Services.AddSingleton<IVerificationCodeStore, InMemoryVerificationCodeStore>();
builder.Services.AddScoped<IProfileRealtimeNotifier, ProfileRealtimeNotifier>();
builder.Services.AddScoped<IProfileAppService, ProfileAppService>();
builder.Services.AddScoped<IPasswordAppService, PasswordAppService>();
builder.Services.AddScoped<IUserTaskRealtimeNotifier, UserTaskRealtimeNotifier>();
builder.Services.AddScoped<IUserTaskQueryService, UserTaskQueryService>();
builder.Services.AddScoped<IUserTaskCommandService, UserTaskCommandService>();
builder.Services.AddScoped<ITeamMemberQueryService, TeamMemberQueryService>();
builder.Services.AddScoped<ITeamMemberCommandService, TeamMemberCommandService>();
builder.Services.AddScoped<ITeamInvitationService, TeamInvitationService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthAppService, AuthAppService>();
builder.Services.AddScoped<IUserLookupService, UserLookupService>();
builder.Services.AddScoped<IGroupChatRealtimeNotifier, GroupChatRealtimeNotifier>();
builder.Services.AddScoped<IGroupChatQueryService, GroupChatQueryService>();
builder.Services.AddScoped<IGroupChatCommandService, GroupChatCommandService>();
builder.Services.AddScoped<IFriendQueryService, FriendQueryService>();
builder.Services.AddScoped<IFriendCommandService, FriendCommandService>();
builder.Services.AddScoped<IOccupationStatisticsService, OccupationStatisticsService>();
builder.Services.AddScoped<IQuizAppService, QuizAppService>();
builder.Services.AddScoped<IChatRealtimeNotifier, ChatRealtimeNotifier>();
builder.Services.AddScoped<IChatMessageTextResolver, ChatMessageTextResolver>();
builder.Services.AddScoped<IChatMessageQueryService, ChatMessageQueryService>();
builder.Services.AddScoped<IChatMessageCommandService, ChatMessageCommandService>();
builder.Services.AddScoped<IChatQueryService, ChatQueryService>();
builder.Services.AddScoped<ICompanyRealtimeNotifier, CompanyRealtimeNotifier>();
builder.Services.AddScoped<ICompanyQueryService, CompanyQueryService>();
builder.Services.AddScoped<ICompanyCommandService, CompanyCommandService>();


// Identity configuration (only user management, no roles)
builder.Services.AddIdentity<CustomUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskFlowDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/connect"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// App configuration
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors(x =>
{
    x.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials();
});


app.UseHttpsRedirection();
app.UseRouting(); 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); 
app.MapHub<ConnectionHub>("/connect");

app.Run();
