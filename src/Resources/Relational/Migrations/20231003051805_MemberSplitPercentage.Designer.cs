﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spenses.Resources.Relational;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231003051805_MemberSplitPercentage")]
    partial class MemberSplitPercentage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Credit", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<Guid>("HomeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("PaidByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("HomeId");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("PaidByMemberId");

                    b.ToTable("Credit");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HomeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IncurredByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("HomeId");

                    b.HasIndex("IncurredByMemberId");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Expense");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Home", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Home");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("HomeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedById")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SplitPercentage")
                        .HasColumnType("float");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("HomeId");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("UserId");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.UserIdentity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserIdentity");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Credit", b =>
                {
                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "CreatedBy")
                        .WithMany("CreatedCredits")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.Home", "Home")
                        .WithMany("Credits")
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "ModifiedBy")
                        .WithMany("ModifiedCredits")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.Member", "PaidByMember")
                        .WithMany("Credits")
                        .HasForeignKey("PaidByMemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Home");

                    b.Navigation("ModifiedBy");

                    b.Navigation("PaidByMember");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Expense", b =>
                {
                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "CreatedBy")
                        .WithMany("CreatedExpenses")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.Home", "Home")
                        .WithMany("Expenses")
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.Member", "IncurredByMember")
                        .WithMany("Expenses")
                        .HasForeignKey("IncurredByMemberId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "ModifiedBy")
                        .WithMany("ModifiedExpenses")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Home");

                    b.Navigation("IncurredByMember");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Home", b =>
                {
                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "CreatedBy")
                        .WithMany("CreatedHomes")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "ModifiedBy")
                        .WithMany("ModifiedHomes")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "CreatedBy")
                        .WithMany("CreatedMembers")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.Home", "Home")
                        .WithMany("Members")
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "ModifiedBy")
                        .WithMany("ModifiedMembers")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Spenses.Resources.Relational.Models.UserIdentity", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("CreatedBy");

                    b.Navigation("Home");

                    b.Navigation("ModifiedBy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Home", b =>
                {
                    b.Navigation("Credits");

                    b.Navigation("Expenses");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.Navigation("Credits");

                    b.Navigation("Expenses");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.UserIdentity", b =>
                {
                    b.Navigation("CreatedCredits");

                    b.Navigation("CreatedExpenses");

                    b.Navigation("CreatedHomes");

                    b.Navigation("CreatedMembers");

                    b.Navigation("ModifiedCredits");

                    b.Navigation("ModifiedExpenses");

                    b.Navigation("ModifiedHomes");

                    b.Navigation("ModifiedMembers");
                });
#pragma warning restore 612, 618
        }
    }
}
