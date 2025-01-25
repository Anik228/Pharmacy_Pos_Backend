using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace pharmacy_pos_system.module.medicine.model
{

    public class MedicineConfig : IEntityTypeConfiguration<Medicine>
        {
            public void Configure(EntityTypeBuilder<Medicine> builder)
            {

                builder.ToTable("Medicine");


                builder.HasKey(p => p.Id);


                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                builder.Property(p => p.Brand)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(p => p.Image)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(p => p.Description)
                    .IsRequired()
                    .HasMaxLength(100);


            builder.Property(p => p.Price)
                    .IsRequired();

                builder.Property(p => p.Quantity)
                    .IsRequired();


                builder.Property(p => p.CreatedAt)
                    .IsRequired();

                builder.Property(p =>p.Expiry_date)
                    .IsRequired();
             
            }
        }
    }

