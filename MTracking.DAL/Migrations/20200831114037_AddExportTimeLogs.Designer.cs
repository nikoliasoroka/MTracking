// <auto-generated />
using System;
using MTracking.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MTracking.DAL.Migrations
{
    [DbContext(typeof(MTrackingDbContext))]
    [Migration("20200831114037_AddExportTimeLogs")]
    partial class AddExportTimeLogs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MTracking.Core.Entities.Description", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCustom")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Descriptions");
                });

            modelBuilder.Entity("MTracking.Core.Entities.ExportTimeLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Records")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ExportTimeLogs");
                });

            modelBuilder.Entity("MTracking.Core.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CaseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ClosingOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EnglishCaseName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsChargedCase")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("OpeningOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("PortfolioNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PortfolioStatus")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioNumber")
                        .IsUnique();

                    b.ToTable("Files");
                });

            modelBuilder.Entity("MTracking.Core.Entities.TimeLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BillingDateCreation")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CommitRecordId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DescriptionId")
                        .HasColumnType("int");

                    b.Property<int?>("FileId")
                        .HasColumnType("int");

                    b.Property<bool>("ForExport")
                        .HasColumnType("bit");

                    b.Property<int?>("TopicId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WorkTime")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DescriptionId");

                    b.HasIndex("FileId");

                    b.HasIndex("TopicId");

                    b.HasIndex("UserId");

                    b.ToTable("TimeLogs");
                });

            modelBuilder.Entity("MTracking.Core.Entities.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCustom")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("MTracking.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommitId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmployeeRunCommit")
                        .HasColumnType("bit");

                    b.Property<bool>("EmployeeSoftwareInvention")
                        .HasColumnType("bit");

                    b.Property<string>("EnglishName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HebrewName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPasswordChanged")
                        .HasColumnType("bit");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommitId")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MTracking.Core.Entities.Description", b =>
                {
                    b.HasOne("MTracking.Core.Entities.User", "User")
                        .WithMany("Descriptions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MTracking.Core.Entities.TimeLog", b =>
                {
                    b.HasOne("MTracking.Core.Entities.Description", "Description")
                        .WithMany("TimeLogs")
                        .HasForeignKey("DescriptionId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MTracking.Core.Entities.File", "File")
                        .WithMany("TimeLogs")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MTracking.Core.Entities.Topic", "Topic")
                        .WithMany("TimeLogs")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MTracking.Core.Entities.User", "User")
                        .WithMany("TimeLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MTracking.Core.Entities.Topic", b =>
                {
                    b.HasOne("MTracking.Core.Entities.User", "User")
                        .WithMany("Topics")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
