﻿using System.Linq;
using System.Web.Mvc;
using uIntra.Core.Extentions;
using uIntra.Core.Grid;
using uIntra.Core.UmbracoEventServices;
using uIntra.Search;
using uIntra.Users;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;
using static uIntra.Core.Constants.GridEditorConstants;

namespace Compent.uIntra.Core
{
    public static class UmbracoEventsModule
    {
        public static void RegisterEvents()
        {
            ContentService.Published += ContentServiceOnPublished;
            ContentService.UnPublished += ContentServiceOnUnPublished;

            MemberService.Deleting += MemberServiceOnDeleting;

            MediaService.Saved += MediaServiceOnSaved;
            MediaService.Trashed += MediaServiceOnTrashed;
        }

        private static void MediaServiceOnSaved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            var services = DependencyResolver.Current.GetServices<IUmbracoMediaEventService>();
            foreach (var service in services)
                service.ProcessMediaSaved(sender, e);
        }

        private static void MediaServiceOnTrashed(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            var services = DependencyResolver.Current.GetServices<IUmbracoMediaEventService>();
            foreach (var service in services)
                service.ProcessMediaTrashed(sender, e);
        }

        private static void ContentServiceOnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
        {
            var contentIndexer = DependencyResolver.Current.GetService<IContentIndexer>();
            var umbracoHelper = DependencyResolver.Current.GetService<UmbracoHelper>();
            var gridHelper = DependencyResolver.Current.GetService<IGridHelper>();

            foreach (var entity in publishEventArgs.PublishedEntities)
            {
                contentIndexer.FillIndex(entity.Id);
                umbracoHelper.TypedContentAtRoot()
                    .SelectMany(c => c.DescendantsOrSelf())
                    .Where(c => ContainsGlobalPanel(c, entity))
                    .Select(c => c.Id)
                    .ToList()
                    .ForEach(contentIndexer.FillIndex);
            }
        }
        static IGridHelper gridHelper = DependencyResolver.Current.GetService<IGridHelper>();
        private static bool ContainsGlobalPanel(IPublishedContent content, IContent globalPanel)
        {
            return gridHelper
                    .GetValues(content, GlobalPanelPickerAlias)
                    .Any(t => t.value.id == globalPanel.Id);
        }

        private static void ContentServiceOnUnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
        {
            var contentIndexer = DependencyResolver.Current.GetService<IContentIndexer>();
            foreach (var entity in publishEventArgs.PublishedEntities)
                contentIndexer.DeleteFromIndex(entity.Id);
        }

        private static void MemberServiceOnDeleting(IMemberService sender, DeleteEventArgs<IMember> e)
        {
            var cacheableUserService = DependencyResolver.Current.GetService<ICacheableIntranetUserService>();
            var memberService = DependencyResolver.Current.GetService<IMemberService>();

            foreach (var member in e.DeletedEntities)
            {
                member.IsLockedOut = true;
                memberService?.Save(member);
                cacheableUserService?.UpdateUserCache(member.Key);
            }

            if (e.CanCancel)
            {
                e.Cancel = true;                           
            }
        }
    }
}