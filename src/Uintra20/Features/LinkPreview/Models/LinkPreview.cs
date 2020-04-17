﻿using System;

namespace Uintra20.Features.LinkPreview.Models
{
    public class LinkPreview
    {
        public int Id { get; set; }

        public Uri Uri { get; set; }
        public string Url { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Uri ImageUri { get; set; }

        public Uri FaviconUri { get; set; }
    }
}