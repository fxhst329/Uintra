﻿using System;
using System.Linq;
using System.Web;
using Compent.Extensions;
using UBaseline.Core.RequestContext;
using Uintra20.Core.Activity.Models.Headers;
using Uintra20.Core.Controls.LightboxGallery;
using Uintra20.Core.Member.Entities;
using Uintra20.Core.Member.Services;
using Uintra20.Core.UbaselineModels.RestrictedNode;
using Uintra20.Features.Groups.Helpers;
using Uintra20.Features.Links;
using Uintra20.Features.Media;
using Uintra20.Features.Media.Helpers;
using Uintra20.Features.Media.Strategies.Preset;
using Uintra20.Features.Permissions;
using Uintra20.Features.Permissions.Interfaces;
using Uintra20.Features.Social.Models;
using Uintra20.Features.Tagging.UserTags.Services;
using Uintra20.Infrastructure.Extensions;

namespace Uintra20.Features.Social.Converters
{
    public class SocialDetailsPageViewModelConverter :
        UintraRestrictedNodeViewModelConverter<SocialDetailsPageModel, SocialDetailsPageViewModel>
    {
        private readonly IFeedLinkService _feedLinkService;
        private readonly IUserTagService _userTagService;
        private readonly ISocialService<Entities.Social> _socialService;
        private readonly IIntranetMemberService<IntranetMember> _memberService;
        private readonly ILightboxHelper _lightboxHelper;
        private readonly IPermissionsService _permissionsService;
        private readonly IGroupHelper _groupHelper;
        private readonly IUBaselineRequestContext _context;
        public SocialDetailsPageViewModelConverter(
            IFeedLinkService feedLinkService,
            IIntranetMemberService<IntranetMember> memberService,
            IUserTagService userTagService,
            ISocialService<Entities.Social> socialsService,
            ILightboxHelper lightboxHelper,
            IPermissionsService permissionsService,
            IGroupHelper groupHelper,
            IErrorLinksService errorLinksService, 
            IUBaselineRequestContext context)
            : base(errorLinksService)
        {
            _feedLinkService = feedLinkService;
            _userTagService = userTagService;
            _socialService = socialsService;
            _memberService = memberService;
            _lightboxHelper = lightboxHelper;
            _permissionsService = permissionsService;
            _groupHelper = groupHelper;
            _context = context;
        }

        public override ConverterResponseModel MapViewModel(SocialDetailsPageModel node, SocialDetailsPageViewModel viewModel)
        {
            var id = _context.ParseQueryString("id").TryParseGuid();

            if (!id.HasValue) return NotFoundResult();

            var social = _socialService.Get(id.Value);

            if (social == null) return NotFoundResult();

            if (!_permissionsService.Check(PermissionResourceTypeEnum.Social, PermissionActionEnum.View))
            {
                return ForbiddenResult();
            }

            var member = _memberService.GetCurrentMember();
            
            viewModel.Details = GetViewModel(social);
            viewModel.Tags = _userTagService.Get(id.Value);
            viewModel.CanEdit = _socialService.CanEdit(id.Value);
            viewModel.IsGroupMember = !social.GroupId.HasValue || member.GroupIds.Contains(social.GroupId.Value);

            var groupIdStr = HttpContext.Current.Request["groupId"];
            if (Guid.TryParse(groupIdStr, out var groupId) && social.GroupId == groupId)
            {
                viewModel.GroupHeader = _groupHelper.GetHeader(groupId);
            }

            return OkResult();
        }

        protected SocialExtendedViewModel GetViewModel(Entities.Social social)
        {
            var viewModel = social.Map<SocialViewModel>();

            viewModel.Media = MediaHelper.GetMediaUrls(social.MediaIds);

            viewModel.LightboxPreviewModel = _lightboxHelper.GetGalleryPreviewModel(social.MediaIds, PresetStrategies.ForActivityDetails);
            viewModel.CanEdit = _socialService.CanEdit(social);
            viewModel.Links = _feedLinkService.GetLinks(social.Id);
            viewModel.IsReadOnly = false;
            viewModel.HeaderInfo = social.Map<IntranetActivityDetailsHeaderViewModel>();
            viewModel.HeaderInfo.Dates = social.PublishDate.ToDateTimeFormat().ToEnumerable();
            viewModel.HeaderInfo.Owner = _memberService.Get(social).ToViewModel();
            viewModel.HeaderInfo.Links = _feedLinkService.GetLinks(social.Id);

            var extendedModel = viewModel.Map<SocialExtendedViewModel>();

            return extendedModel;
        }
    }
}