﻿// <auto-generated />
using System;
using DirectorPortalDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DirectorPortalDatabase.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210330014412_AddAdditionalFieldUtilities")]
    partial class AddAdditionalFieldUtilities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11");

            modelBuilder.Entity("DirectorPortalDatabase.Models.AdditionalFields", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FieldName")
                        .HasColumnType("TEXT");

                    b.Property<string>("TableName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AdditionalFields");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreetAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("ZipCode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ZipCodeExt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Business", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BusinessName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExtraFields")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExtraNotes")
                        .HasColumnType("TEXT");

                    b.Property<int>("MailingAddressId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MembershipLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PhysicalAddressId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Website")
                        .HasColumnType("TEXT");

                    b.Property<int>("YearEstablished")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MailingAddressId");

                    b.HasIndex("PhysicalAddressId");

                    b.ToTable("Businesses");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.BusinessRep", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactPersonId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("ContactPersonId");

                    b.ToTable("BusinessReps");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Categories", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.CategoryRef", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryRef");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.ContactPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ContactPeople");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Email", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactPersonId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ContactPersonId");

                    b.ToTable("Emails");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.EmailGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GroupName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EmailGroups");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.EmailGroupMember", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EmailId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmailId");

                    b.HasIndex("GroupId");

                    b.ToTable("EmailGroupMembers");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("GrossPay")
                        .HasColumnType("TEXT");

                    b.Property<string>("InvoiceNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("PayPalReferenceTxnId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PayPalTransactionId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ProcessingFees")
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.PaymentItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PaymentId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("PaymentId");

                    b.ToTable("PaymentItems");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.PhoneNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactPersonId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GEnumPhoneType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<string>("Number")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ContactPersonId");

                    b.ToTable("PhoneNumbers");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.ReportField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModelPropertyName")
                        .HasColumnType("TEXT");

                    b.Property<int>("TemplateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TemplateId");

                    b.ToTable("ReportFields");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.ReportTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModelName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReportTemplateName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ReportTemplates");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Todo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("MarkedAsDone")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TodoListItems");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.YearlyData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BallotNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BusinessId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Credit")
                        .HasColumnType("REAL");

                    b.Property<double>("DuesPaid")
                        .HasColumnType("REAL");

                    b.Property<string>("ExtraFields")
                        .HasColumnType("TEXT");

                    b.Property<int>("TermLength")
                        .HasColumnType("INTEGER");

                    b.Property<double>("TicketsReturned")
                        .HasColumnType("REAL");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("BusinessYearlyData");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Business", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Address", "MailingAddress")
                        .WithMany()
                        .HasForeignKey("MailingAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DirectorPortalDatabase.Models.Address", "PhysicalAddress")
                        .WithMany()
                        .HasForeignKey("PhysicalAddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.BusinessRep", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Business", "Business")
                        .WithMany("BusinessReps")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DirectorPortalDatabase.Models.ContactPerson", "ContactPerson")
                        .WithMany("BusinessReps")
                        .HasForeignKey("ContactPersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Categories", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Business", null)
                        .WithMany("Categories")
                        .HasForeignKey("BusinessId");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.CategoryRef", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DirectorPortalDatabase.Models.Categories", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Email", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.ContactPerson", "ContactPerson")
                        .WithMany("Emails")
                        .HasForeignKey("ContactPersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.EmailGroupMember", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Email", "Email")
                        .WithMany("EmailGroups")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DirectorPortalDatabase.Models.EmailGroup", "Group")
                        .WithMany("Emails")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.Payment", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.PaymentItem", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("DirectorPortalDatabase.Models.Payment", "Payment")
                        .WithMany("Items")
                        .HasForeignKey("PaymentId");
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.PhoneNumber", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.ContactPerson", "ContactPerson")
                        .WithMany("PhoneNumbers")
                        .HasForeignKey("ContactPersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.ReportField", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.ReportTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DirectorPortalDatabase.Models.YearlyData", b =>
                {
                    b.HasOne("DirectorPortalDatabase.Models.Business", "Business")
                        .WithMany("YearlyData")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
