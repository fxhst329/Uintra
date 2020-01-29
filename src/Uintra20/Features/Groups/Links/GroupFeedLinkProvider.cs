﻿using Uintra20.Core.Activity.Helpers;
using Uintra20.Core.Feed;
using Uintra20.Features.Links;
using Uintra20.Features.Links.Models;
using Uintra20.Infrastructure.Extensions;

namespace Uintra20.Features.Groups.Links
{
    public class GroupFeedLinkProvider : FeedLinkProvider, IGroupFeedLinkProvider
    {
        public GroupFeedLinkProvider(
            IActivityPageHelper activityPageHelper,
            IProfileLinkProvider profileLinkProvider)
            : base(activityPageHelper, profileLinkProvider)
        {
        }

        public IActivityLinks GetLinks(GroupActivityTransferModel activity)
        {
            return new ActivityLinks
            {
                Feed = _activityPageHelper.GetFeedUrl()?.AddGroupId(activity.GroupId),
                Overview = null,//helper.GetOverviewPageUrl().AddGroupId(activity.GroupId),//TODO: Research overview pages
                Create = _activityPageHelper.GetCreatePageUrl(activity.Type)?.AddGroupId(activity.GroupId),
                Details = _activityPageHelper.GetDetailsPageUrl(activity.Type, activity.Id).AddGroupId(activity.GroupId),
                Edit = _activityPageHelper.GetEditPageUrl(activity.Type, activity.Id).AddGroupId(activity.GroupId),
                Owner = GetProfileLink(activity.OwnerId),
                DetailsNoId = _activityPageHelper.GetDetailsPageUrl(activity.Type).AddGroupId(activity.GroupId)
            };
        }

        public IActivityCreateLinks GetCreateLinks(GroupActivityTransferCreateModel model)
        {

            return new ActivityCreateLinks
            {
                Feed = _activityPageHelper.GetFeedUrl()?.AddGroupId(model.GroupId),
                Overview = null,//helper.GetOverviewPageUrl().AddGroupId(model.GroupId),//TODO: Research overview pages
                Create = _activityPageHelper.GetCreatePageUrl(model.Type)?.AddGroupId(model.GroupId),
                Owner = GetProfileLink(model.OwnerId),
                DetailsNoId = _activityPageHelper.GetDetailsPageUrl(model.Type).AddGroupId(model.GroupId)
            };
        }
    }
}