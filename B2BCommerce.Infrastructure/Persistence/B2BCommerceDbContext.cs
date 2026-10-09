using B2BCommerce.Application.Common.Interfaces;
using B2BCommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace B2BCommerce.Infrastructure.Persistence
{
    public class B2BCommerceDbContext : DbContext
    {
        private readonly ICurrentTenantService _currentTenantService;

        public B2BCommerceDbContext(
            DbContextOptions<B2BCommerceDbContext> options,
            ICurrentTenantService currentTenantService) : base(options)
        {
            _currentTenantService = currentTenantService;
        }
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Tenant (Only soft delete filter)
            modelBuilder.Entity<Tenant>()
                .HasQueryFilter(t => !t.IsDeleted);

            // 2. Multi-Tenant Scoped Entities (TenantId + Soft delete filter)
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => u.TenantId == _currentTenantService.TenantId && !u.IsDeleted);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => c.TenantId == _currentTenantService.TenantId && !c.IsDeleted);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => p.TenantId == _currentTenantService.TenantId && !p.IsDeleted);

            modelBuilder.Entity<InventoryItem>()
                .HasQueryFilter(i => i.TenantId == _currentTenantService.TenantId && !i.IsDeleted);

            modelBuilder.Entity<Order>()
                .HasQueryFilter(o => o.TenantId == _currentTenantService.TenantId && !o.IsDeleted);

            modelBuilder.Entity<OrderItem>()
                .HasQueryFilter(oi => oi.TenantId == _currentTenantService.TenantId && !oi.IsDeleted);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(B2BCommerceDbContext).Assembly);

        }


    }
}
