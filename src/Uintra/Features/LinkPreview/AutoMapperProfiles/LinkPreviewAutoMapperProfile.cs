﻿using AutoMapper;
using Uintra.Features.LinkPreview.Sql;

namespace Uintra.Features.LinkPreview.AutoMapperProfiles
{
    public class LinkPreviewAutoMapperProfile : Profile
    {
        public LinkPreviewAutoMapperProfile()
        {
            CreateMap<Compent.LinkPreview.HttpClient.LinkPreview, LinkPreviewEntity>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OgDescription, o => o.Ignore())
                .ForMember(d => d.Uri, o => o.Ignore())
                .ForMember(d => d.MediaId, o => o.Ignore());
        }
    }
}