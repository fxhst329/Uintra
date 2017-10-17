using System;
using uIntra.Core.Extentions;

namespace uIntra.Groups
{
    public class GroupLinkProvider : IGroupLinkProvider
    {
        private readonly IGroupContentProvider _contentProvider;

        public GroupLinkProvider(IGroupContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }

        public string GetGroupLink(Guid groupId)
        {
           return _contentProvider.GetGroupRoomPage().Url.AddGroupId(groupId);
        }

        public string GetDeactivatedGroupLink(Guid groupId)
        {
            return _contentProvider.GetDeactivatedGroupPage().Url.AddGroupId(groupId);
        }

        public string GetGroupsOverviewLink()
        {
            return _contentProvider.GetOverviewPage().Url;
        }

        public string GetCreateGroupLink()
        {
            return _contentProvider.GetCreateGroupPage().Url;
        }
    }
}