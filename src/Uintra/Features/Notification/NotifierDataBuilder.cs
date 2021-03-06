﻿//using EmailWorker.Data.Extensions;
using Compent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uintra.Core.Activity.Entities;
using Uintra.Core.Member.Abstractions;
using Uintra.Core.Member.Entities;
using Uintra.Core.Member.Services;
using Uintra.Features.Comments.Models;
using Uintra.Features.Notification.Entities.Base;
using Uintra.Features.Subscribe;
using Uintra.Infrastructure.Extensions;
using Uintra.Infrastructure.Helpers;
using static Uintra.Features.Notification.Configuration.NotificationTypeEnum;

namespace Uintra.Features.Notification
{
    public class NotifierDataBuilder : INotifierDataBuilder
    {
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;
        private readonly INotifierDataHelper _notifierDataHelper;


        public NotifierDataBuilder(
            IIntranetMemberService<IntranetMember> intranetMemberService,
            INotifierDataHelper notifierDataHelper)
        {
            _intranetMemberService = intranetMemberService;
            _notifierDataHelper = notifierDataHelper;
        }

        public NotifierData GetNotifierData<TActivity>(TActivity activity, Enum notificationType)
            where TActivity : IIntranetActivity, IHaveOwner
        {
            var currentMemberId = new Lazy<Guid>(()=> _intranetMemberService.GetCurrentMemberId());
            var data = new NotifierData
            {
                NotificationType = notificationType,
                ActivityType = activity.Type,
            };

            switch (notificationType)
            {
                case ActivityLikeAdded:
                    data.Value = _notifierDataHelper.GetLikesNotifierDataModel(activity, notificationType, currentMemberId.Value);
                    data.ReceiverIds = ReceiverIds(activity, notificationType).Except(new[] {currentMemberId.Value});
                    break;

                case BeforeStart:
                    data.Value = _notifierDataHelper.GetActivityReminderDataModel(activity, notificationType);
                    data.ReceiverIds = ReceiverIds(activity, notificationType);
                    break;

                case EventHidden:
                case EventUpdated:
                    data.Value = _notifierDataHelper.GetActivityNotifierDataModel(activity, notificationType, currentMemberId.Value);
                    data.ReceiverIds = ReceiverIds(activity, notificationType).Except(new[] {currentMemberId.Value});
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return data;
        }

        public NotifierData GetNotifierData<TActivity>(CommentModel comment, TActivity activity, Enum notificationType)
            where TActivity : IIntranetActivity, IHaveOwner
        {
            var currentMember = _intranetMemberService.GetCurrentMember();
            var currentMemberId = currentMember.Id;
            var data = new NotifierData
            {
                NotificationType = notificationType,
                ActivityType = activity.Type,
                ReceiverIds = ReceiverIds(comment, activity, notificationType, currentMember).Except(new [] { currentMemberId }/*.ToEnumerableOfOne()*/),
                Value = _notifierDataHelper.GetCommentNotifierDataModel(activity, comment, notificationType, currentMemberId)
            };

            return data;
        }

        public async Task<NotifierData> GetNotifierDataAsync<TEntity>(TEntity activity, Enum notificationType) where TEntity : IIntranetActivity, IHaveOwner
        {
            var currentMemberId = _intranetMemberService.GetCurrentMemberId();
            var data = new NotifierData
            {
                NotificationType = notificationType,
                ActivityType = activity.Type,
                ReceiverIds = ReceiverIds(activity, notificationType).Except(new [] { currentMemberId })
            };

            switch (notificationType)
            {
                case ActivityLikeAdded:
                    data.Value = await _notifierDataHelper.GetLikesNotifierDataModelAsync(activity, notificationType, currentMemberId);
                    break;

                case BeforeStart:
                    data.Value = await _notifierDataHelper.GetActivityReminderDataModelAsync(activity, notificationType);
                    break;

                case EventHidden:
                case EventUpdated:
                    data.Value = await _notifierDataHelper.GetActivityNotifierDataModelAsync(activity, notificationType, currentMemberId);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return data;
        }

        public async Task<NotifierData> GetNotifierDataAsync<TEntity>(CommentModel comment, TEntity activity, Enum notificationType) where TEntity : IIntranetActivity, IHaveOwner
        {
            //var currentMember = await _intranetMemberService.GetCurrentMemberAsync();
            var currentMember = _intranetMemberService.GetCurrentMember();
            var data = new NotifierData
            {
                NotificationType = notificationType,
                ActivityType = activity.Type,
                ReceiverIds = ReceiverIds(comment, activity, notificationType, currentMember).Except(new []{currentMember.Id}/*.ToEnumerableOfOne()*/),
                Value = await _notifierDataHelper.GetCommentNotifierDataModelAsync(activity, comment, notificationType, currentMember.Id)
            };

            return data;
        }

        private static IEnumerable<Guid> ReceiverIds<TActivity>(TActivity activity, Enum notificationType)
            where TActivity : IIntranetActivity, IHaveOwner
        {
            switch (notificationType)
            {
                case Enum type when type.In(BeforeStart, EventHidden, EventUpdated) && activity is ISubscribable subscribable:
                    return GetNotifiedSubscribers(subscribable);

                case ActivityLikeAdded:
                    return OwnerId(activity);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Guid> ReceiverIds<TActivity>(
            CommentModel comment,
            TActivity activity,
            Enum notificationType,
            IIntranetMember currentMember)
            where TActivity : IIntranetActivity, IHaveOwner
        {
            switch (notificationType)
            {
                case CommentAdded when activity is ISubscribable subscribable:
                    return GetNotifiedSubscribers(subscribable).Concat(OwnerId(activity)).Distinct();

                case CommentAdded:
                    return OwnerId(activity);

                case CommentEdited:
                    return OwnerId(activity);

                case CommentLikeAdded:
                    return currentMember.Id == comment.UserId ? new List<Guid>() : OwnerId(comment);

                case CommentReplied:
                    return OwnerId(comment);

                default:
                    throw new InvalidOperationException();
            }
        }

        private static IEnumerable<Guid> OwnerId(IHaveOwner haveOwner)
        {
            yield return haveOwner.OwnerId;
        }

        private static IEnumerable<Guid> OwnerId(CommentModel comment)
        {
            yield return comment.UserId;
        }

        private static IEnumerable<Guid> GetNotifiedSubscribers(ISubscribable subscribable) =>
            subscribable.Subscribers
                .Where(s => !s.IsNotificationDisabled)
                .Select(s => s.UserId);
    }
}