﻿namespace Uintra.Features.Media.Extensions
{
    public static class MediaExtensions
    {
        public static string ClearExtension(this string src) => 
            src?.ToLower()?.TrimStart('.');
    }
}