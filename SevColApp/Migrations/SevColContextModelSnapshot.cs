﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SevColApp.Context;

namespace SevColApp.Migrations
{
    [DbContext(typeof(SevColContext))]
    partial class SevColContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SevColApp.Models.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abbreviation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ColonyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ColonyId");

                    b.ToTable("Banks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Abbreviation = "EFG",
                            ColonyId = 1,
                            Name = "Earth Financial Group"
                        },
                        new
                        {
                            Id = 2,
                            Abbreviation = "MOU",
                            ColonyId = 2,
                            Name = "Monetary Optimisation Unit"
                        },
                        new
                        {
                            Id = 3,
                            Abbreviation = "BOM",
                            ColonyId = 3,
                            Name = "Bank of Mars"
                        },
                        new
                        {
                            Id = 4,
                            Abbreviation = "EFF",
                            ColonyId = 4,
                            Name = "Endeavour for Financing"
                        },
                        new
                        {
                            Id = 5,
                            Abbreviation = "TEB",
                            ColonyId = 4,
                            Name = "Technically Evol Bank"
                        },
                        new
                        {
                            Id = 6,
                            Abbreviation = "GRF",
                            ColonyId = 4,
                            Name = "Green Foundation"
                        },
                        new
                        {
                            Id = 7,
                            Abbreviation = "WEM",
                            ColonyId = 4,
                            Name = "Wells-Morgan"
                        },
                        new
                        {
                            Id = 8,
                            Abbreviation = "UFB",
                            ColonyId = 4,
                            Name = "Union Faction Bank"
                        },
                        new
                        {
                            Id = 9,
                            Abbreviation = "SAS",
                            ColonyId = 5,
                            Name = "Saturnians Abroad Support"
                        },
                        new
                        {
                            Id = 10,
                            Abbreviation = "RSF",
                            ColonyId = 6,
                            Name = "Rock Steady Finance"
                        },
                        new
                        {
                            Id = 11,
                            Abbreviation = "SDB",
                            ColonyId = 6,
                            Name = "Sock Drawer Bank"
                        },
                        new
                        {
                            Id = 12,
                            Abbreviation = "TEB",
                            ColonyId = 7,
                            Name = "The Enlightened Bank"
                        },
                        new
                        {
                            Id = 13,
                            Abbreviation = "SCB",
                            ColonyId = 8,
                            Name = "SevCol Bank"
                        });
                });

            modelBuilder.Entity("SevColApp.Models.BankAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BankId")
                        .HasColumnType("int");

                    b.Property<long>("Credit")
                        .HasColumnType("bigint");

                    b.Property<long>("ExpectedIncome")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.HasIndex("userId");

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("SevColApp.Models.Colony", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Colonies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Earth"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Luna"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Mars"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Jupiter"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Saturn"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Eden and Kordoss"
                        },
                        new
                        {
                            Id = 7,
                            Name = "The Worlds of Light"
                        },
                        new
                        {
                            Id = 8,
                            Name = "SevCol"
                        });
                });

            modelBuilder.Entity("SevColApp.Models.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayingAccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceivingAccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("SevColApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AllowanceStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Prefixes")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 7777777,
                            FirstName = "SevCol",
                            LastName = "Master",
                            LoginName = "GameMaster",
                            PasswordHash = new byte[] { 92, 108, 153, 233, 172, 89, 109, 109, 73, 34, 120, 114, 10, 78, 6, 6, 23, 244, 108, 223, 240, 91, 44, 24, 224, 247, 90, 97, 186, 156, 70, 239, 103, 78, 98, 107, 120, 0, 73, 97, 179, 112, 148, 154, 9, 251, 230, 73, 252, 109, 40, 116, 98, 159, 54, 98, 8, 32, 41, 41, 228, 151, 201, 183 },
                            Prefixes = "Game"
                        });
                });

            modelBuilder.Entity("SevColApp.Models.Bank", b =>
                {
                    b.HasOne("SevColApp.Models.Colony", "Colony")
                        .WithMany()
                        .HasForeignKey("ColonyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Colony");
                });

            modelBuilder.Entity("SevColApp.Models.BankAccount", b =>
                {
                    b.HasOne("SevColApp.Models.Bank", "Bank")
                        .WithMany()
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SevColApp.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");

                    b.Navigation("user");
                });
#pragma warning restore 612, 618
        }
    }
}
