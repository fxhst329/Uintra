﻿using UBaseline.Shared.DocumentLibraryPanel;
using Uintra20.Core.Search.Converters.SearchDocumentPanelConverter;
using Uintra20.Core.Search.Entities;
using Umbraco.Core;

namespace Uintra20.Features.Search.Converters.Panel
{
    public class DocumentLibraryPanelSearchConverter : SearchDocumentPanelConverter<DocumentLibraryPanelViewModel>
    {
        protected override SearchablePanel OnConvert(DocumentLibraryPanelViewModel panel)
        {
            return new SearchablePanel
            {
                Title = panel.Title,
                Content = panel.RichTextEditor?.Value?.StripHtml()
            };
        }
    }
}