﻿// <auto-generated />
using System;
using FarmCentral.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FarmCentral.Migrations
{
    [DbContext(typeof(FarmCentralDbContext))]
    [Migration("20230528131634_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FarmCentral.Models.ModelsDB.Employee", b =>
                {
                    b.Property<Guid>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmployeeEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployeePassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("FarmCentral.Models.ModelsDB.Farmer", b =>
                {
                    b.Property<Guid>("FarmerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FarmerEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FarmerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FarmerPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FarmerId");

                    b.ToTable("Farmers");
                });

            modelBuilder.Entity("FarmCentral.Models.ModelsDB.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FarmerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProductDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ProductListDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("ProductPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.HasIndex("FarmerId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FarmCentral.Models.ModelsDB.Product", b =>
                {
                    b.HasOne("FarmCentral.Models.ModelsDB.Farmer", "Farmer")
                        .WithMany("FarmerProducts")
                        .HasForeignKey("FarmerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Farmer");
                });

            modelBuilder.Entity("FarmCentral.Models.ModelsDB.Farmer", b =>
                {
                    b.Navigation("FarmerProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
