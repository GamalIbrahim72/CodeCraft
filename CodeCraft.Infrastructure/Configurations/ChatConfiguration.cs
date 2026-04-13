using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Configurations;
public class ChatConfiguration:IEntityTypeConfiguration<TrackChatMessage>
{
    public void Configure(EntityTypeBuilder<TrackChatMessage> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content)
              .IsRequired()
              .HasMaxLength(1000);

        builder.Property(e => e.SentAt)
              .IsRequired();

        builder.HasOne(e => e.Sender)
              .WithMany(u => u.TrackChatMessages)
              .HasForeignKey(e => e.SenderId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Track)
              .WithMany(t => t.ChatMessages)
              .HasForeignKey(e => e.TrackId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
