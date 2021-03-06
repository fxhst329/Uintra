﻿using UBaseline.Shared.Node;
using UBaseline.Shared.PageSettings;
using UBaseline.Shared.PanelContainer;
using UBaseline.Shared.Property;

namespace Uintra.Features.News.Models
{
    public class UintraNewsDetailsPageModel : 
        NodeModel, 
        IPanelsComposition
    {
        public PropertyModel<PanelContainerModel> Panels { get; set; }
        public PageSettingsCompositionModel PageSettings { get; set; }
    }
}