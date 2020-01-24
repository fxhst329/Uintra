﻿using System.Collections.Generic;
using System.Linq;
using Uintra20.Core.Member.Entities;
using Uintra20.Core.Member.Services;
using Compent.Extensions;
using UBaseline.Core.Navigation;
using UBaseline.Core.Node;
using Uintra20.Features.Navigation.Models;
using Uintra20.Infrastructure;

namespace Uintra20.Features.Navigation
{
    public class NavigationModelsBuilder : INavigationModelsBuilder
    {
        private readonly IUintraInformationService _uintraInformationService;
        private readonly INodeModelService _nodeModelService;
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;
        private readonly INodeDirectAccessValidator _nodeDirectAccessValidator;
        private readonly INavigationBuilder _navigationBuilder;

        public NavigationModelsBuilder(
            IUintraInformationService uintraInformationService,
            INodeModelService nodeModelService,
            INodeDirectAccessValidator nodeDirectAccessValidator,
            IIntranetMemberService<IntranetMember> intranetMemberService,
            INavigationBuilder navigationBuilder)
        {
            _uintraInformationService = uintraInformationService;
            _nodeModelService = nodeModelService;
            _intranetMemberService = intranetMemberService;
            _nodeDirectAccessValidator = nodeDirectAccessValidator;
            _navigationBuilder = navigationBuilder;
        }

        public virtual IEnumerable<TreeNavigationItemModel> GetLeftSideNavigation()
        {
            var navigationNodes = _nodeModelService.AsEnumerable()
                .Where(i => i.Level >= 1 && _nodeDirectAccessValidator.HasAccess(i))
                .OfType<IUintraNavigationComposition>()
                .OrderBy(i => i.SortOrder)
                .Where(i => i.Navigation.ShowInMenu.Value && i.Url.HasValue());

            var items = _navigationBuilder.GetTreeNavigation(navigationNodes);

            return items;
        }

        public virtual TopNavigationModel GetTopNavigationModel()
        {
            var menuItems = new List<TopNavigationItem>();
            var currentMember = _intranetMemberService.GetCurrentMember();

            if (currentMember.RelatedUser != null)
            {
                menuItems.Add(new TopNavigationItem()
                {
                    Name = "Login To Umbraco",
                    Type = TopNavigationItemTypes.LoginToUmbraco,
                    Url = "/api/auth/login/umbraco"
                });
            }
            menuItems.Add(new TopNavigationItem()
            {
                Name = "Edit Profile",
                Type = TopNavigationItemTypes.EditProfile,
                Url = "/profile-edit" //todo return not stabbed link to edit profile
            });

            menuItems.Add(new TopNavigationItem()
            {
                Name = $"Uintra Help v{_uintraInformationService.Version}",
                Type = TopNavigationItemTypes.UintraHelp,
                Url = _uintraInformationService.DocumentationLink.ToString()
            });

            menuItems.Add(new TopNavigationItem()
            {
                Name = "Logout",
                Type = TopNavigationItemTypes.Logout,
                Url = "/api/auth/logout"
            });

            var model = new TopNavigationModel()
            {
                CurrentMember = _intranetMemberService.GetCurrentMember(),
                Items = menuItems
            };

            return model;
        }
    }
}