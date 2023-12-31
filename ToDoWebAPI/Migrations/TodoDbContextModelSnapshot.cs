﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoWebAPI.Models;

#nullable disable

namespace ToDoWebAPI.Migrations
{
    [DbContext(typeof(TodoDbContext))]
    partial class TodoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("ToDoWebAPI.Models.Todo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CompletedDate")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan?>("CompletedDuration")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Todos");
                });

            modelBuilder.Entity("ToDoWebAPI.Models.Todo", b =>
                {
                    b.OwnsOne("ToDoWebAPI.Models.TodoInfo", "Info", b1 =>
                        {
                            b1.Property<Guid>("TodoId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("BadgePath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("ReportPath")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("TodoId");

                            b1.ToTable("Todos");

                            b1.WithOwner()
                                .HasForeignKey("TodoId");
                        });

                    b.Navigation("Info")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
