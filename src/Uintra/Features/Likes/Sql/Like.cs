﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uintra.Persistence;
using Uintra.Persistence.Sql;

namespace Uintra.Features.Likes.Sql
{
    [UintraTable("Like")]
    public class Like : SqlEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override Guid Id { get; set; }

        [Required]
        [Index("UQ_Like_UserId_EntityId", 1, IsUnique = true)]
        public Guid UserId { get; set; }

        [Required]
        [Index("UQ_Like_UserId_EntityId", 2, IsUnique = true)]
        public Guid EntityId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}