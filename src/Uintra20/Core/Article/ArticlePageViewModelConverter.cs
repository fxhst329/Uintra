﻿using Compent.Extensions;
using UBaseline.Core.Node;
using UBaseline.Core.RequestContext;
using Uintra20.Features.Breadcrumbs.Services.Contracts;
using Uintra20.Features.Navigation.Services;
using Uintra20.Infrastructure.Extensions;

namespace Uintra20.Core.Article
{
    public class ArticlePageViewModelConverter : INodeViewModelConverter<ArticlePageModel, ArticlePageViewModel>
    {
        private readonly IUBaselineRequestContext _context;
        private readonly IBreadcrumbService _breadcrumbService;
        private readonly ISubNavigationModelBuilder _subNavigationModelBuilder;


        public ArticlePageViewModelConverter(
            IUBaselineRequestContext context,
            IBreadcrumbService breadcrumbService,
            ISubNavigationModelBuilder subNavigationModelBuilder )
        {
            _context = context;
            _breadcrumbService = breadcrumbService;
            _subNavigationModelBuilder = subNavigationModelBuilder;
        }

        public void Map(ArticlePageModel node, ArticlePageViewModel viewModel)
        {
            var groupId = _context.ParseQueryString("groupId").TryParseGuid();
            viewModel.Breadcrumbs = _breadcrumbService.GetBreadcrumbs();
            viewModel.GroupId = groupId;
            viewModel.SubNavigation = _subNavigationModelBuilder.GetMenu();
        }
    }
}