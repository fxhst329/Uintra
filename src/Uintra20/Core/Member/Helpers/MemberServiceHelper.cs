﻿using System.Collections.Generic;
using System.Linq;
using Uintra20.Core.Member.Abstractions;
using Uintra20.Core.Member.Models;
using Uintra20.Features.Media;
using Uintra20.Features.Media.Strategies.ImageResize;
using Uintra20.Infrastructure.Extensions;
using Umbraco.Core.Models;
using Umbraco.Core.Services;


namespace Uintra20.Core.Member.Helpers
{
    public class MemberServiceHelper : IMemberServiceHelper
    {
        private readonly IMemberService _memberService;
        private readonly IMemberTypeService _memberTypeService;
        private readonly IImageHelper _imageHelper;

        public MemberServiceHelper(IMemberService memberService, IMemberTypeService memberTypeService)
        {
            _memberService = memberService;
            _memberTypeService = memberTypeService;
        }

        private const string RelatedUserPropertyName = "relatedUser";
        private const string FirstLoginPerformedPropertyName = "firstLoginPerformed";
        public Dictionary<IMember, int?> GetRelatedUserIdsForMembers(IEnumerable<IMember> users)
        {
            return users.ToDictionary(x => x, u => u.GetValue<int?>(RelatedUserPropertyName));
        }

        public bool IsFirstLoginPerformed (IMember member) => member.GetValue<bool>(FirstLoginPerformedPropertyName);

        public void SetFirstLoginPerformed(IMember member)
        {
            member.SetValue(FirstLoginPerformedPropertyName, value: true);
            _memberService.Save(member, raiseEvents: false);
        }

        private const string MemberTypeAlias = "Member";
        private const string MemberProfileTabName = "Profile";
        public IEnumerable<PropertyType> GetAvailableProfileProperties()
        {
            var mts = _memberTypeService.Get(MemberTypeAlias);
            var profileTab = mts.PropertyGroups.FirstOrDefault(i => i.Name.Equals(MemberProfileTabName));
            var properties = profileTab.PropertyTypes
                .Where(i => !i.Alias.Equals(RelatedUserPropertyName));
            return properties;
        }

        public MemberViewModel ToViewModel(IIntranetMember member)
        {
            var result = member.Map<MemberViewModel>();
            result.Photo = _imageHelper.GetImageWithResize(member.Photo, RenderStrategies.ForMemberProfile.Thumbnail);

            return result;
        }
    }
}
