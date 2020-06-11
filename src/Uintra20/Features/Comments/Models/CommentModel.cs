﻿using System;
using Uintra20.Features.LinkPreview.Models;

namespace Uintra20.Features.Comments.Models
{
    public class CommentModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ActivityId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public string Text { get; set; }

        public Guid? ParentId { get; set; }

        public LinkPreviewModel LinkPreview { get; set; }
    }
}