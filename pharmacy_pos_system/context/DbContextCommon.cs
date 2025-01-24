using Microsoft.Extensions.Hosting;
using pharmacy_pos_system.module.user.model;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using pharmacy_pos_system.module.medicine.model;

namespace pharmacy_pos_system.context
{
   
        public class DbContextCommon : DbContext
        {
            public DbSet<User> Users { get; set; }

            //public DbSet<Medicine> Medicines { get; set; }   


       
        public DbContextCommon(DbContextOptions<DbContextCommon> options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.HasDefaultSchema("pharmacy");
             
                modelBuilder.ApplyConfiguration(new UserConfig());

               // modelBuilder.ApplyConfiguration(new MedicineConfig());

                base.OnModelCreating(modelBuilder);
            }
        }
    
}
