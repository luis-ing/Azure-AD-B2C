using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace App_Azure_OpenId.Models;

public partial class NotesDbContext : DbContext
{
    public NotesDbContext()
    {
    }

    public NotesDbContext(DbContextOptions<NotesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=notes_db;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.27-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_general_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notes");

            entity.HasIndex(e => e.UserUpdated, "fk_notes_user1_idx");

            entity.HasIndex(e => e.UserCreated, "fk_notes_user_idx");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("datetime")
                .HasColumnName("dateUpdated");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("isActive");
            entity.Property(e => e.Title)
                .HasMaxLength(60)
                .HasColumnName("title");
            entity.Property(e => e.UserCreated)
                .HasColumnType("int(11)")
                .HasColumnName("userCreated");
            entity.Property(e => e.UserUpdated)
                .HasColumnType("int(11)")
                .HasColumnName("userUpdated");

            entity.HasOne(d => d.UserCreatedNavigation).WithMany(p => p.NoteUserCreatedNavigations)
                .HasForeignKey(d => d.UserCreated)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notes_user");

            entity.HasOne(d => d.UserUpdatedNavigation).WithMany(p => p.NoteUserUpdatedNavigations)
                .HasForeignKey(d => d.UserUpdated)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notes_user1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("datetime")
                .HasColumnName("dateCreated");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("isActive");
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .HasColumnName("mail");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Pass)
                .HasMaxLength(150)
                .HasColumnName("pass");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
