﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Uintra.Features.CentralFeed.Providers;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Uintra.Infrastructure.Providers
{
    public class ContentPageContentProvider : ContentProviderBase, IContentPageContentProvider
    {
        private readonly IDocumentTypeAliasProvider _documentTypeAliasProvider;
        private readonly IEnumerable<string> _baseXPath;

        public ContentPageContentProvider(IDocumentTypeAliasProvider documentTypeAliasProvider)
        {
            _documentTypeAliasProvider = documentTypeAliasProvider;
            _baseXPath = new[] { _documentTypeAliasProvider.GetHomePage() };
        }

        public IEnumerable<IPublishedContent> GetAllContentPages()
        {
            var contentPages = GetDescendants(_baseXPath.Append(_documentTypeAliasProvider.GetArticlePage())).ToList();
            return contentPages;
        }

        public IPublishedContent GetFirstUserListContentPage()
        {
            return GetAllContentPages().FirstOrDefault(i =>
            {
                var obj = i.Value<JObject>("grid");
                var tokens = obj.SelectTokens("sections[0].rows[0].areas[0].controls[*].editor.alias");
                return tokens.Any(j =>
                    j is JValue temp && temp != null && temp.Value?.ToString() == "custom.UserList");
            });
        }

        public IPublishedContent GetUserListContentPageFromPicker()
        {
            var homePage = GetContent(_baseXPath);
            return homePage.Value<IEnumerable<IPublishedContent>>("userListPage")?.FirstOrDefault();
        }
    }
}