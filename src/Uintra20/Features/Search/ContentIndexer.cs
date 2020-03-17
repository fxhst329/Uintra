﻿using System.Collections.Generic;
using System.Linq;
using Compent.Shared.Extensions.Bcl;
using UBaseline.Core.Node;
using UBaseline.Shared.Node;
using UBaseline.Shared.PanelContainer;
using Uintra20.Features.Search.Entities;
using Uintra20.Features.Search.Indexes;
using Uintra20.Features.Search.Web;
using Uintra20.Infrastructure.Constants;
using Uintra20.Infrastructure.Extensions;
using Uintra20.Infrastructure.Grid;
using Uintra20.Infrastructure.Providers;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Uintra20.Features.Search
{
    public class ContentIndexer : IIndexer, IContentIndexer
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ISearchUmbracoHelper _searchUmbracoHelper;
        private readonly IElasticContentIndex _contentIndex;
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;
        private readonly INodeModelService _nodeModelService;
        private readonly ISearchContentPanelConverterProvider _searchContentPanelConverterProvider;

        public ContentIndexer(
            UmbracoHelper umbracoHelper,
            ISearchUmbracoHelper searchUmbracoHelper,
            IElasticContentIndex contentIndex,
            IDocumentTypeAliasProvider documentTypeAliasProvider, 
            INodeModelService nodeModelService,
            ISearchContentPanelConverterProvider searchContentPanelConverterProvider)
        {
            _umbracoHelper = umbracoHelper;
            _searchUmbracoHelper = searchUmbracoHelper;
            _contentIndex = contentIndex;
            _documentTypeAliasProvider = documentTypeAliasProvider;
            _nodeModelService = nodeModelService;
            _searchContentPanelConverterProvider = searchContentPanelConverterProvider;
        }

        public void FillIndex()
        {
            var homePage = _umbracoHelper.ContentAtRoot().First(pc => pc.ContentType.Alias.Equals(_documentTypeAliasProvider.GetHomePage()));
            var contentPages = homePage.DescendantsOfType(_documentTypeAliasProvider.GetArticlePage());

            var searchableContents = contentPages
                .Where(pc => _searchUmbracoHelper.IsSearchable(pc))
                .Select(GetContent);

            _contentIndex.Index(searchableContents);
        }

        public void FillIndex(int id)
        {
            var publishedContent = _umbracoHelper.Content(id);
            if (publishedContent == null) return;

            var isSearchable = _searchUmbracoHelper.IsSearchable(publishedContent);
            if (isSearchable)
            {
                _contentIndex.Delete(publishedContent.Id);
                _contentIndex.Index(GetContent(publishedContent));
            }
            else
            {
                _contentIndex.Delete(publishedContent.Id);
            }
        }

        public void DeleteFromIndex(int id)
        {
            _contentIndex.Delete(id);
        }

        private SearchableContent GetContent(IPublishedContent publishedContent)
        {
            var node = _nodeModelService.Get<NodeModel>(publishedContent.Id) as IPanelsComposition;
            var panels = _searchContentPanelConverterProvider.Convert(node);


             return new SearchableContent
             {
                 Id = publishedContent.Id,
                 Type = SearchableTypeEnum.Content.ToInt(),
                 Url = publishedContent.Url.ToLinkModel(),
                 Title = publishedContent.Name,
                 Panels = panels
             };
        }
    }
}
