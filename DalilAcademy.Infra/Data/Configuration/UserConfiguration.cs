using DalilAcademy.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Infra.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
          builder.HasIndex(u => u.Email).IsUnique();
          builder.Property(u => u.Email).HasMaxLength(256);
          builder.Property(u => u.Role).HasMaxLength(20);

        }
    }
}
