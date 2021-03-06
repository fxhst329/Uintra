﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uintra.Persistence;
using Uintra.Persistence.Sql;

namespace Uintra.Features.Comments.Sql
{
    [UintraTable("CommentToLinkPreview")]
    public class CommentToLinkPreviewEntity : SqlEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required, Index("UQ_CommentToLinkPreview_CommentId_LinkPreviewId", 1, IsUnique = true)]
        public Guid CommentId { get; set; }
        [Required, Index("UQ_CommentToLinkPreview_CommentId_LinkPreviewId", 2, IsUnique = true)]
        public int LinkPreviewId { get; set; }
    }
}