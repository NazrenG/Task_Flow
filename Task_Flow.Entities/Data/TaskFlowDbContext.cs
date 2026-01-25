using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task_Flow.Entities.Models;

namespace Task_Flow.Entities.Data
{
    public class TaskFlowDbContext : IdentityDbContext<CustomUser>
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //avtomatik hemise silinmemisleri getirsin
            modelBuilder.Entity<UserTask>().HasQueryFilter(f=>!f.IsDeleted);
            modelBuilder.Entity<Work>().HasQueryFilter(f=>!f.IsDeleted);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.UserFriend)
                .WithMany(u => u.FriendsOf)
                .HasForeignKey(f => f.UserFriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestNotification>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.RequestNotificationsReceiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RequestNotification>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.RequestNotificationsSender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.MessagesReceiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Work>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.TaskForUsers)
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskAssigne>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.TaskAssignees)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskAssigne>()
                .HasOne(ta => ta.TaskForUser)
                .WithMany(t => t.TaskAssignees)
                .HasForeignKey(ta => ta.TaskForUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Project)
                .WithMany(p => p.TeamMembers)
                .HasForeignKey(tm => tm.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecentActivity>()
                .HasOne(ra => ra.User)
                .WithMany(u => u.RecentActivities)
                .HasForeignKey(ra => ra.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GroupChatMembers>().HasOne(gcm => gcm.GroupChat).WithMany(g => g.Members).HasForeignKey(k => k.GroupId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GroupChatMessage>().HasOne(c => c.GroupChat).WithMany(c=>c.Messages).HasForeignKey(k=>k.GroupChatId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GroupChatMembers>()
                .HasOne(gcm => gcm.CustomUser)
                .WithMany()
                .HasForeignKey(gcm => gcm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupChat>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(g => g.IsDeleted)
                      .HasDefaultValue(false);

                entity.HasMany(g => g.Members)
                      .WithOne(m => m.GroupChat)
                      .HasForeignKey(m => m.GroupId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(g => g.Messages)
                      .WithOne(m => m.GroupChat)
                      .HasForeignKey(m => m.GroupChatId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GroupChatMembers>(entity =>
            {
                entity.HasOne(gcm => gcm.CustomUser)
                      .WithMany()
                      .HasForeignKey(gcm => gcm.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });




            base.OnModelCreating(modelBuilder);
        }
        

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Work> Works { get; set; }
        public virtual DbSet<TeamMember> TeamMembers { get; set; }
        public virtual DbSet<Friend> Friends { get; set; }
        public virtual DbSet<TaskAssigne> TaskAssignes { get; set; }
        public virtual DbSet<TaskCustomize> TaskCustomizes { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatMessage> ChatMessages { get; set; }
        public virtual DbSet<RequestNotification> RequestNotifications { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationSetting> NotificationSettings { get; set; }
        public virtual DbSet<RecentActivity> RecentActivities { get; set; }
        public virtual DbSet<ProjectActivity>ProjectActivities { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }
        public virtual DbSet<GroupChat> GroupChats { get; set; }
        public virtual DbSet<GroupChatMessage> GroupChatMessages { get; set; }
        public  virtual DbSet<GroupChatMembers> GroupChatMembers { get; set; }
        public virtual DbSet<CanbanColumn> CanbanColumns { get; set; }
        public virtual DbSet<Sprint> Sprints { get; set; }


    }
}
