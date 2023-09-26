﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spenses.Resources.Relational;

#nullable disable

namespace Spenses.Resources.Relational.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HomeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IncurredByMemberId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("HomeId");

                    b.HasIndex("IncurredByMemberId");

                    b.ToTable("Expense");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Home", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Home");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("AnnualTakeHomeIncome")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid>("HomeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("HomeId");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserIdentity");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Expense", b =>
                {
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

                    b.Navigation("Home");

                    b.Navigation("IncurredByMember");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.HasOne("Spenses.Resources.Relational.Models.Home", "Home")
                        .WithMany("Members")
                        .HasForeignKey("HomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Home");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Home", b =>
                {
                    b.Navigation("Expenses");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("Spenses.Resources.Relational.Models.Member", b =>
                {
                    b.Navigation("Expenses");
                });
#pragma warning restore 612, 618
        }
    }
}
