﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaftanRailway.Domain.Concrete.DbContext.OBD
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class OBDEntities : DbContext
    {
        public OBDEntities()
            : base("name=OBDEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<v_akt> v_akts { get; set; }
        public DbSet<v_akt_vag> v_akt_vags { get; set; }
        public DbSet<v_kart> v_karts { get; set; }
        public DbSet<v_nach> v_nachs { get; set; }
        public DbSet<v_o_v> v_o_vs { get; set; }
        public DbSet<v_otpr> v_otprs { get; set; }
        public DbSet<v_pam> v_pams { get; set; }
        public DbSet<v_pam_vag> v_pam_vags { get; set; }
    }
}
