﻿using System;
using System.Linq;
using Uintra.Core.User;
using Uintra.Notification;
using Uintra.Notification.Base;
using Uintra.Notification.Configuration;
using Uintra.Notification.DefaultImplementation;

namespace Compent.Uintra.Core.Notification
{
    public class MailNotifierService : INotifierService
    {
        private readonly IMailService _mailService;
        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;
        private readonly INotificationModelMapper<EmailNotifierTemplate, EmailNotificationMessage> _notificationModelMapper;
        private readonly NotificationSettingsService _notificationSettingsService;
        private readonly NotifierTypeProvider _notifierTypeProvider;

        public MailNotifierService(
            IMailService mailService,
            IIntranetUserService<IIntranetUser> intranetUserService,
            INotificationModelMapper<EmailNotifierTemplate, EmailNotificationMessage> notificationModelMapper,
            NotifierTypeProvider notifierTypeProvider,
            NotificationSettingsService notificationSettingsService)
        {
            _mailService = mailService;
            _intranetUserService = intranetUserService;
            _notificationModelMapper = notificationModelMapper;
            _notifierTypeProvider = notifierTypeProvider;
            _notificationSettingsService = notificationSettingsService;
        }

        public Enum Type => NotifierTypeEnum.EmailNotifier;

        public void Notify(NotifierData data)
        {
            var identity = new ActivityEventIdentity(data.ActivityType, data.NotificationType).AddNotifierIdentity(Type);
            var settings = _notificationSettingsService.Get<EmailNotifierTemplate>(identity);
            if (!settings.IsEnabled) return;
            var receivers = _intranetUserService.GetMany(data.ReceiverIds).ToList();
            foreach (var receiverId in data.ReceiverIds)
            {
                var user = receivers.Find(receiver => receiver.Id == receiverId);

                var message = _notificationModelMapper.Map(data.Value, settings.Template, user);
                _mailService.Send(message);
            }
        }
    }
}