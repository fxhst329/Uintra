﻿using System;
using System.Collections.Generic;
using Uintra20.Core.Member.Abstractions;

namespace Uintra20.Features.Groups
{
    public interface IGroupMember : IIntranetMember
    {
        IEnumerable<Guid> GroupIds { get; set; }
    }
}
