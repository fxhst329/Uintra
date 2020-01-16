﻿namespace Uintra20.Features.Media.Strategies.ImageResize
{
    public interface IRenderStrategy
    {
        string Thumbnail { get; }
        string Preview { get; }
        string PreviewTwo { get; }
        int MediaFilesToDisplay { get; }
    }
}