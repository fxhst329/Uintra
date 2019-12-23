﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Compent.CommandBus;
using Uintra20.Core.Member.Commands;
using Uintra20.Core.Member.Models;
using Uintra20.Core.User;
using Uintra20.Infrastructure.Extensions;

namespace Uintra20.Core.Member.Services
{
    public class MentionService : IMentionService
    {
        private readonly ICommandPublisher _commandPublisher;
        private readonly IIntranetUserContentProvider _intranetUserContentProvider;

        public MentionService(
            ICommandPublisher commandPublisher,
            IIntranetUserContentProvider intranetUserContentProvider)
        {
            _commandPublisher = commandPublisher;
            _intranetUserContentProvider = intranetUserContentProvider;
        }

        private const string MentionDetectionRegex = "(?<=\\bdata-id=\")[^\"]*";


        public IEnumerable<Guid> GetMentions(string text)
        {
            var profilePrefix = _intranetUserContentProvider.GetProfilePage()?.Url?.AddIdParameter(string.Empty);

            var matches = Regex.Matches(text, MentionDetectionRegex)
                .Cast<Match>()
                .Value
                ?.Replace(profilePrefix, string.Empty);

            return LanguageExt.Prelude.parseGuid(matches)
                .Somes();
        }

        public void ProcessMention(MentionModel model)
        {
            var command = model.Map<MentionCommand>();
            _commandPublisher.Publish(command);
        }
    }
}