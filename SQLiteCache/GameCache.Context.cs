﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CleanEmulatorFrontend.SqLiteCache
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GamesCacheEntities : DbContext
    {
        public GamesCacheEntities()
            : base("name=GamesCacheEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<base_path> base_path { get; set; }
        public virtual DbSet<emulated_system> emulated_system { get; set; }
        public virtual DbSet<game> games { get; set; }
        public virtual DbSet<game_base_path> game_base_path { get; set; }
        public virtual DbSet<last_played> last_played { get; set; }
    }
}
