// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using BlazorAppForms.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BlazorAppForms.Server.Data;

public partial class BlazorFormsContext : DbContext
{
    public BlazorFormsContext()
    {
    }

    public BlazorFormsContext(DbContextOptions<BlazorFormsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customer { get; set; }
    public virtual DbSet<CustomerLocation> CustomerLocation { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=BlazorForms;Integrated Security=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_100_CI_AI");

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdCustomer);

            entity.Property(e => e.Birth).HasColumnType("date");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CustomerLocation>(entity =>
        {
            entity.HasKey(e => e.IdCustomerLocation);

            entity.HasIndex(e => e.IdCustomer, "IX1_CustomerLocation_Customer");

            entity.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCustomerNavigation)
                .WithMany(p => p.CustomerLocation)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerLocation_Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}