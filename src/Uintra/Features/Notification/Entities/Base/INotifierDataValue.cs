﻿using Uintra.Features.Links.Models;

namespace Uintra.Features.Notification.Entities.Base
{
    public interface INotifierDataValue
    {
        UintraLinkModel Url { get; set; }
        bool IsPinned { get; set; }
        bool IsPinActual { get; set; }
    }
}
