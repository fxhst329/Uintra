﻿using System.Configuration;
using System.Globalization;
using Compent.uIntra.Core.Search.Entities;

namespace Compent.uIntra.Core.Search
{
    public class SearchScoreProvider : ISearchScoreProvider
    {
        private const string UserNameScoreKey = "Search.UserNameScore";
        private const string UserEmailScoreKey = "Search.UserEmailScore";
        private const string UserPhoneScoreKey = "Search.UserPhoneScore";
        private const string TitleScoreKey = "Search.TitleScore";

        public SearchScoreModel GetScores()
        {
            var scores = new SearchScoreModel()
            {
                UserNameScore = double.Parse(ConfigurationManager.AppSettings[UserNameScoreKey], CultureInfo.InvariantCulture),
                UserEmailScore = double.Parse(ConfigurationManager.AppSettings[UserEmailScoreKey], CultureInfo.InvariantCulture),
                UserPhoneScore = double.Parse(ConfigurationManager.AppSettings[UserPhoneScoreKey], CultureInfo.InvariantCulture),
                TitleScore = double.Parse(ConfigurationManager.AppSettings[TitleScoreKey], CultureInfo.InvariantCulture)
            };

            return scores;
        }
    }
}