﻿using Keas.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Keas.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamMember> TeamMembers { get; set; }
        public virtual DbSet<TeamMembership> TeamRoles { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
    }
}