using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.DBContext;

public partial class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FriendList> FriendLists { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<GroupList> GroupLists { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FriendList>(entity =>
        {
            entity.HasKey(e => e.FriendId).HasName("PK__FriendLi__A2CF658352747910");

            entity.ToTable("FriendList");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.FriendUser).WithMany(p => p.FriendListFriendUsers)
                .HasForeignKey(d => d.FriendUserId)
                .HasConstraintName("fk_FriendList_FriendUserId_Users_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.FriendListUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_FriendList_UserId_Users_UserId");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__FriendRe__33A8517A592C8949");

            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.FromUser).WithMany(p => p.FriendRequestFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("fk_FriendRequests_FromUserId_Users_UserId");

            entity.HasOne(d => d.ToUser).WithMany(p => p.FriendRequestToUsers)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("fk_FriendRequests_ToUserId_Users_UserId");
        });

        modelBuilder.Entity<GroupList>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36A4BD4710A");

            entity.ToTable("GroupList");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GroupLists)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_Groups_CreatedBy_Users_UserId");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GroupMem__3214EC07993526C2");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_GroupMembers_GroupId_Groups_GroupId");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_GroupMembers_UserId_Users_UserId");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9C7584A369");

            entity.Property(e => e.FileUrl).HasMaxLength(300);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.FromUser).WithMany(p => p.MessageFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("fk_Messages_FromUserId_Users_UserId");

            entity.HasOne(d => d.Group).WithMany(p => p.Messages)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("fk_Messages_GroupId_Groups_GroupId");

            entity.HasOne(d => d.ToUser).WithMany(p => p.MessageToUsers)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("fk_Messages_ToUserId_Users_UserId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.ResetTokenExpiry).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
