﻿using System;
using System.Linq;
using Uintra.Core.Member.Abstractions;
using Uintra.Core.Member.Entities;
using Uintra.Core.Member.Services;
using Uintra.Features.Notification.Entities;
using Uintra.Features.Notification.Entities.Base;
using Uintra.Features.Notification.Models;
using Uintra.Features.Notification.Models.NotifierTemplates;
using Uintra.Features.Notification.Services;
using Uintra.Infrastructure.Extensions;
using static Uintra.Features.Notification.Constants.TokensConstants;

namespace Uintra.Features.Notification
{
    public class UiNotificationModelMapper : INotificationModelMapper<UiNotifierTemplate, UiNotificationMessage>
    {
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;

        public UiNotificationModelMapper(IIntranetMemberService<IntranetMember> intranetMemberService)
        {
            _intranetMemberService = intranetMemberService;
        }

        public UiNotificationMessage Map(INotifierDataValue notifierData, UiNotifierTemplate template, IIntranetMember receiver)
        {
            var message = new UiNotificationMessage
            {
                ReceiverId = receiver.Id,
                IsPinned = notifierData.IsPinned,
                IsPinActual = notifierData.IsPinActual,
                //IsDesktopNotificationEnabled = template.IsDesktopNotificationEnabled
            };
            (string, string)[] tokens;
            switch (notifierData)
            {
                case ActivityNotifierDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    message.NotifierId = model.NotifierId;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (ActivityType, model.ActivityType.ToString()),
                        (FullName, _intranetMemberService.Get(model.NotifierId).DisplayedName),
                        (NotifierFullName, receiver.DisplayedName),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;
                case ActivityReminderDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (ActivityType, model.ActivityType.ToString()),
                        (StartDate, model.StartDate.ToShortDateString()),
                        (FullName, receiver.DisplayedName),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;
                case CommentNotifierDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    message.NotifierId = model.NotifierId;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (FullName, _intranetMemberService.Get(model.NotifierId).DisplayedName),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;
                case LikesNotifierDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    message.NotifierId = model.NotifierId;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (ActivityType, model.ActivityType.ToString()),
                        (FullName, _intranetMemberService.Get(model.NotifierId).DisplayedName),
                        (CreatedDate, model.CreatedDate.ToShortDateString()),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;
                case UserMentionNotifierDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    message.NotifierId = model.NotifierId;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (FullName, _intranetMemberService.Get(model.ReceiverId).DisplayedName),
                        (TaggedBy, _intranetMemberService.Get(model.NotifierId).DisplayedName),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;

                case GroupInvitationDataModel model:
                    message.NotificationType = model.NotificationType;
                    message.Url = model.Url;
                    message.NotifierId = model.NotifierId;
                    tokens = new[]
                    {
                        (ActivityTitle, model.Title),
                        (FullName, _intranetMemberService.Get(model.ReceiverId).DisplayedName),
                        (Url, model.Url.ToString()),
                        (Title, model.Title),
                        (TaggedBy, _intranetMemberService.Get(model.NotifierId).DisplayedName),
                        (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
                    };
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }

            message.Message = ReplaceTokens(template.Message, tokens);
            return message;
        }

        //public async Task<UiNotificationMessage> MapAsync(INotifierDataValue notifierData, UiNotifierTemplate template, IIntranetMember receiver)
        //{
        //    var message = new UiNotificationMessage
        //    {
        //        ReceiverId = receiver.Id,
        //        IsPinned = notifierData.IsPinned,
        //        IsPinActual = notifierData.IsPinActual,
        //        //IsDesktopNotificationEnabled = template.IsDesktopNotificationEnabled
        //    };
        //    (string, string)[] tokens;
        //    switch (notifierData)
        //    {
        //        case ActivityNotifierDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            message.NotifierId = model.NotifierId;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (ActivityType, model.ActivityType.ToString()),
        //                (FullName, (await _intranetMemberService.GetAsync(model.NotifierId)).DisplayedName),
        //                (NotifierFullName, receiver.DisplayedName),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;
        //        case ActivityReminderDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (ActivityType, model.ActivityType.ToString()),
        //                (StartDate, model.StartDate.ToShortDateString()),
        //                (FullName, receiver.DisplayedName),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;
        //        case CommentNotifierDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            message.NotifierId = model.NotifierId;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (FullName, (await _intranetMemberService.GetAsync(model.NotifierId)).DisplayedName),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;
        //        case LikesNotifierDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            message.NotifierId = model.NotifierId;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (ActivityType, model.ActivityType.ToString()),
        //                (FullName, (await _intranetMemberService.GetAsync(model.NotifierId)).DisplayedName),
        //                (CreatedDate, model.CreatedDate.ToShortDateString()),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;
        //        case UserMentionNotifierDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            message.NotifierId = model.NotifierId;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (FullName, (await _intranetMemberService.GetAsync(model.ReceiverId)).DisplayedName),
        //                (TaggedBy, (await _intranetMemberService.GetAsync(model.NotifierId)).DisplayedName),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;

        //        case GroupInvitationDataModel model:
        //            message.NotificationType = model.NotificationType;
        //            message.Url = model.Url;
        //            message.NotifierId = model.NotifierId;
        //            tokens = new[]
        //            {
        //                (ActivityTitle, model.Title),
        //                (FullName, (await _intranetMemberService.GetAsync(model.ReceiverId)).DisplayedName),
        //                (Url, model.Url),
        //                (Title, model.Title),
        //                (TaggedBy, (await _intranetMemberService.GetAsync(model.NotifierId)).DisplayedName),
        //                (NotificationType, model.NotificationType.ToString().SplitOnUpperCaseLetters())
        //            };
        //            break;

        //        default:
        //            throw new IndexOutOfRangeException();
        //    }

        //    message.Message = ReplaceTokens(template.Message, tokens);
        //    return message;
        //}

        private string ReplaceTokens(string source, params (string token, string value)[] replacePairs) =>
            replacePairs
                .Aggregate(source ?? string.Empty, (acc, pair) => acc.Replace(pair.token, pair.value));
    }
}