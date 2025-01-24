using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace pharmacy_pos_system.module.user.model
{

    public class UserConfig : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {

                builder.ToTable("User");


                builder.HasKey(p => p.Id);


                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                builder.Property(p => p.Role)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(p => p.Password)
                    .IsRequired();

                builder.Property(p => p.Email)
                    .IsRequired();


                builder.Property(p => p.Phone)
                    .IsRequired();

                builder.Property(p =>p.CreatedAt)
                    .IsRequired();


               
            }
        }
    }

