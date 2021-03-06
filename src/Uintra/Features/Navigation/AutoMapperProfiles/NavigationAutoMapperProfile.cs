﻿using AutoMapper;
using UBaseline.Core.Navigation;
using Uintra.Features.Navigation.Models;
using Uintra.Features.Navigation.Models.MyLinks;

namespace Uintra.Features.Navigation.AutoMapperProfiles
{
    public class NavigationAutoMapperProfile : Profile
    {
        public  NavigationAutoMapperProfile()
        {
            //For new Uintra, need to check maybe it would be solved by automapper itself

            CreateMap<TopNavigationModel, TopNavigationViewModel>()
                .ForMember(dst => dst.CurrentMember, o => o.Ignore())
                .ForMember(dst => dst.Items, o => o.MapFrom(s=>s.Items));


            CreateMap<TreeNavigationItemModel, MenuItemViewModel>()
                .ForMember(dst => dst.IsClickable, o => o.Ignore())
                .ForMember(dst => dst.IsHeading, o => o.Ignore())
                .ForMember(dst => dst.IsHomePage, o => o.Ignore())
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Title));

            CreateMap<TopNavigationItem, TopNavigationItemViewModel>();

            CreateMap<MenuItemModel, MenuItemViewModel>();

            CreateMap<TreeNavigationItemModel, LeftNavigationViewModel>()
                .ForMember(dst => dst.MenuItems, o => o.MapFrom(src => src.Children))
                .ForMember(dst => dst.GroupItems, o => o.Ignore())
                .ForMember(dst => dst.SharedLinks, o => o.Ignore())
                .ForMember(dst => dst.MyLinks, o => o.Ignore());
            //CreateMap<TopNavigationModel, TopNavigationViewModel>();

            CreateMap<MyLinkItemModel, MyLinkItemViewModel>();

            //CreateMap<SystemLinksModel, SystemLinksViewModel>();
            //CreateMap<SystemLinkItemModel, SystemLinkItemViewModel>()
            //    .ForMember(dst => dst.Name, o => o.MapFrom(el => el.Caption))
            //    .ForMember(dst => dst.Url, o => o.MapFrom(el => el.Link))
            //    .ForMember(dst => dst.Target, o => o.MapFrom(el => el.Target));

            CreateMap<UserListLinkModel, UserListLinkViewModel>();

            //CreateMap<TopNavigationModel, TopMenuViewModel>()
            //    .ForMember(c => c.NotificationsUrl, o => o.Ignore())
            //    .ForMember(c => c.NotificationList, o => o.Ignore());
        }
    }
}