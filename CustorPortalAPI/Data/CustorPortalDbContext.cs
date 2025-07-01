using CustorPortalAPI.Models;
using Microsoft.EntityFrameworkCore;
using File = CustorPortalAPI.Models.File;
using Task = CustorPortalAPI.Models.Task;

namespace CustorPortalAPI.Data
{
    public class CustorPortalDbContext : DbContext
    {
        public CustorPortalDbContext(DbContextOptions<CustorPortalDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAssignee> TaskAssignees { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Project>().ToTable("Project");
            modelBuilder.Entity<Task>().ToTable("Tasks");
            modelBuilder.Entity<TaskAssignee>().ToTable("TaskAssignees");
            modelBuilder.Entity<File>().ToTable("Files"); // Ensure Files table is mapped
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
            modelBuilder.Entity<UserProject>().ToTable("UserProjects");

            modelBuilder.Entity<User>().HasKey(u => u.UserKey);
            modelBuilder.Entity<Role>().HasKey(r => r.RoleKey);
            modelBuilder.Entity<Project>().HasKey(p => p.ProjectKey);
            modelBuilder.Entity<Task>().HasKey(t => t.TaskKey);
            modelBuilder.Entity<File>().HasKey(f => f.FileKey); // Ensure File has a key
            modelBuilder.Entity<Comment>().HasKey(c => c.Id);
            modelBuilder.Entity<Notification>().HasKey(n => n.Id);

            modelBuilder.Entity<TaskAssignee>()
                .HasKey(ta => new { ta.Taskkey, ta.UserKey });

            modelBuilder.Entity<UserProject>()
                .HasKey(up => new { up.UserKey, up.ProjectKey });

            // Map Role properties
            modelBuilder.Entity<Role>()
                .Property(r => r.RoleKey).HasColumnName("RoleKey");
            modelBuilder.Entity<Role>()
                .Property(r => r.Role_Name).HasColumnName("Role_Name");
            modelBuilder.Entity<Role>()
                .Property(r => r.Description).HasColumnName("Description");

            // Map User properties
            modelBuilder.Entity<User>()
                .Property(u => u.UserKey).HasColumnName("UserKey");
            modelBuilder.Entity<User>()
                .Property(u => u.Email).HasColumnName("Email");
            modelBuilder.Entity<User>()
                .Property(u => u.Password_Hash).HasColumnName("Password_Hash");
            modelBuilder.Entity<User>()
                .Property(u => u.First_Name).HasColumnName("First_Name");
            modelBuilder.Entity<User>()
                .Property(u => u.Last_Name).HasColumnName("Last_Name");
            modelBuilder.Entity<User>()
                .Property(u => u.RoleKey).HasColumnName("RoleKey");
            modelBuilder.Entity<User>()
                .Property(u => u.Created_At).HasColumnName("Created_At");
            modelBuilder.Entity<User>()
                .Property(u => u.Updated_At).HasColumnName("Updated_At");
            modelBuilder.Entity<User>()
                .Property(u => u.Is_Active).HasColumnName("Is_Active");

            // Map Project properties
            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectKey).HasColumnName("projectKey");
            modelBuilder.Entity<Project>()
                .Property(p => p.Name).HasColumnName("name");
            modelBuilder.Entity<Project>()
                .Property(p => p.Description).HasColumnName("description");
            modelBuilder.Entity<Project>()
                .Property(p => p.Created_at).HasColumnName("Created_at");
            modelBuilder.Entity<Project>()
                .Property(p => p.Updated_at).HasColumnName("updated_at");
            modelBuilder.Entity<Project>()
                .Property(p => p.creatorKey).HasColumnName("creatorKey");

            // Map File properties
            modelBuilder.Entity<File>()
                .Property(f => f.FileKey).HasColumnName("FileKey");
            modelBuilder.Entity<File>()
                .Property(f => f.ProjectKey).HasColumnName("ProjectKey");
            modelBuilder.Entity<File>()
                .Property(f => f.FileName).HasColumnName("FileName");
            modelBuilder.Entity<File>()
                .Property(f => f.FileType).HasColumnName("FileType");
            modelBuilder.Entity<File>()
                .Property(f => f.Version).HasColumnName("Version");
            modelBuilder.Entity<File>()
                .Property(f => f.FilePath).HasColumnName("FilePath");
            modelBuilder.Entity<File>()
                .Property(f => f.Size).HasColumnName("Size");
            modelBuilder.Entity<File>()
                .Property(f => f.UploaderKey).HasColumnName("UploaderKey");
            modelBuilder.Entity<File>()
                .Property(f => f.UploadedAt).HasColumnName("UploadedAt");
            modelBuilder.Entity<File>()
                .Property(f => f.IsCurrent).HasColumnName("IsCurrent");

            // Relationships (keep existing ones)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleKey);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.creatorKey);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectKey);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorKey);

            modelBuilder.Entity<TaskAssignee>()
                .HasOne(ta => ta.Task)
                .WithMany(t => t.TaskAssignees)
                .HasForeignKey(ta => ta.Taskkey);

            modelBuilder.Entity<TaskAssignee>()
                .HasOne(ta => ta.User)
                .WithMany()
                .HasForeignKey(ta => ta.UserKey);

            modelBuilder.Entity<File>()
                .HasOne(f => f.Project)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.ProjectKey);

            modelBuilder.Entity<File>()
                .HasOne(f => f.Uploader)
                .WithMany()
                .HasForeignKey(f => f.UploaderKey);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .IsRequired(false);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Document)
                .WithMany(f => f.Comments)
                .HasForeignKey(c => c.DocumentId)
                .IsRequired(false);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserKey);

            modelBuilder.Entity<UserProject>()
                .HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.ProjectKey);

            modelBuilder.Entity<Task>()
                .Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("To Do");

            modelBuilder.Entity<Task>()
                .Property(t => t.Priority)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("Low");

            modelBuilder.Entity<File>()
                .Property(f => f.FileType)
                .HasConversion<string>()
                .HasMaxLength(100)
                .IsRequired();
           modelBuilder.Entity<Comment>()
                .Property(c => c.Mentions)
                .HasColumnName("Mentions");
            // Map UserProject properties
            modelBuilder.Entity<UserProject>()
                .Property(up => up.UserKey)
                .HasColumnName("UserKey");
            modelBuilder.Entity<UserProject>()
                .Property(up => up.ProjectKey)
                .HasColumnName("ProjectKey");
            modelBuilder.Entity<UserProject>()
                .Property(up => up.Role)
                .HasColumnName("role"); // Map to snake_case
            modelBuilder.Entity<UserProject>()
                .Property(up => up.Assigned_at)
                .HasColumnName("Assigned_at"); // Map to snake_case
        }
    }
}