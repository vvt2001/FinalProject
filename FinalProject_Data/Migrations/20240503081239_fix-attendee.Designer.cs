﻿// <auto-generated />
using System;
using FinalProject_Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinalProject_Data.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240503081239_fix-attendee")]
    partial class fixattendee
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.17")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FinalProject_Data.Model.Attendee", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("meeting_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("meetingform_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("meeting_id");

                    b.HasIndex("meetingform_id");

                    b.ToTable("attendees");
                });

            modelBuilder.Entity("FinalProject_Data.Model.Meeting", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("duration")
                        .HasColumnType("int");

                    b.Property<string>("location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("meeting_description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("meeting_title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("owner_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("platform")
                        .HasColumnType("int");

                    b.Property<DateTime>("starttime")
                        .HasColumnType("datetime2");

                    b.Property<int>("trangthai")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("owner_id");

                    b.ToTable("meetings");
                });

            modelBuilder.Entity("FinalProject_Data.Model.MeetingForm", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("duration")
                        .HasColumnType("int");

                    b.Property<string>("location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("meeting_description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("meeting_title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("owner_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("platform")
                        .HasColumnType("int");

                    b.Property<DateTime?>("starttime")
                        .HasColumnType("datetime2");

                    b.Property<int>("trangthai")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("owner_id");

                    b.ToTable("meetingforms");
                });

            modelBuilder.Entity("FinalProject_Data.Model.MeetingTime", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("duration")
                        .HasColumnType("int");

                    b.Property<string>("meetingform_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("time")
                        .HasColumnType("datetime2");

                    b.Property<int>("trangthai")
                        .HasColumnType("int");

                    b.Property<int>("vote_count")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("meetingform_id");

                    b.ToTable("meetingtimes");
                });

            modelBuilder.Entity("FinalProject_Data.Model.User", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("trangthai")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("ID");

                    b.HasIndex("email")
                        .IsUnique();

                    b.ToTable("users");
                });

            modelBuilder.Entity("FinalProject_Data.Model.Attendee", b =>
                {
                    b.HasOne("FinalProject_Data.Model.Meeting", "meeting")
                        .WithMany("attendee")
                        .HasForeignKey("meeting_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FinalProject_Data.Model.MeetingForm", "meetingform")
                        .WithMany("attendee")
                        .HasForeignKey("meetingform_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("meeting");

                    b.Navigation("meetingform");
                });

            modelBuilder.Entity("FinalProject_Data.Model.Meeting", b =>
                {
                    b.HasOne("FinalProject_Data.Model.User", "owner")
                        .WithMany()
                        .HasForeignKey("owner_id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("owner");
                });

            modelBuilder.Entity("FinalProject_Data.Model.MeetingForm", b =>
                {
                    b.HasOne("FinalProject_Data.Model.User", "owner")
                        .WithMany()
                        .HasForeignKey("owner_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("owner");
                });

            modelBuilder.Entity("FinalProject_Data.Model.MeetingTime", b =>
                {
                    b.HasOne("FinalProject_Data.Model.MeetingForm", "meetingform")
                        .WithMany("times")
                        .HasForeignKey("meetingform_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("meetingform");
                });

            modelBuilder.Entity("FinalProject_Data.Model.Meeting", b =>
                {
                    b.Navigation("attendee");
                });

            modelBuilder.Entity("FinalProject_Data.Model.MeetingForm", b =>
                {
                    b.Navigation("attendee");

                    b.Navigation("times");
                });
#pragma warning restore 612, 618
        }
    }
}
