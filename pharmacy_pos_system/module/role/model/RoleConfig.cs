using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using pharmacy_pos_system.module.medicine.model;

namespace pharmacy_pos_system.module.role.model
{
 
        public class RoleConfig : IEntityTypeConfiguration<Role>
        {
            public void Configure(EntityTypeBuilder<Role> builder)
            {

                builder.ToTable("Role");


                builder.HasKey(p => p.Id);


                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

            }
        }
    
}
