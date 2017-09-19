﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uIntra.Core.Activity;
using uIntra.Core.Exceptions;
using uIntra.Core.Extentions;
using uIntra.Core.User;
using uIntra.Notification.Base;

namespace uIntra.Notification
{
    public abstract class MonthlyEmailServiceBase : IMonthlyEmailService
    {
        private readonly IMailService _mailService;
        private readonly IExceptionLogger _logger;       
        private readonly IIntranetUserService<IIntranetUser> _intranetUserService;        

        protected MonthlyEmailServiceBase(IMailService mailService,
            IIntranetUserService<IIntranetUser> intranetUserService,
            IExceptionLogger logger)
        {
            _mailService = mailService;
            _intranetUserService = intranetUserService;
            _logger = logger;
        }

        public void SendEmail()
        {
            IEnumerable<IIntranetUser> users = _intranetUserService.GetAll();
            foreach (var user in users)
            {
                try
                {
                    var activities = GetUserActivitiesFilteredByUserTags(user.Id);
                    if (activities.Any())
                    {
                        string activityListString = GetActivityListString(activities);
                        var monthlyMail = GetMonthlyMailModel(activityListString, user);
                        _mailService.Send(monthlyMail);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log(ex);
                }

            }
            
        }

        protected abstract List<Tuple<IIntranetActivity, string>> GetUserActivitiesFilteredByUserTags(Guid userId);        

        private MonthlyMail GetMonthlyMailModel(string userActivities, IIntranetUser user)
        {
            var recipient = new MailRecipient { Email = user.Email, Name = user.DisplayedName };
            return new MonthlyMail
            {
                FullName = user.DisplayedName,
                ActivityList = userActivities,
                Recipients = recipient.ToListOfOne()
            };
        }

        private string GetActivityListString(IEnumerable<Tuple<IIntranetActivity, string>> activities)
        {
            var builder = new StringBuilder();
            foreach (var activity in activities)
            {
                builder.AppendLine($"<a href='{activity.Item2}'>{activity.Item1.Title}</a>");
            }
            return builder.ToString();
        }

    }
}