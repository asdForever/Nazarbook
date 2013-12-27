using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlmaCloud.Classes
{
    static class GlobalVariables
    {
        public static string[] languageArray = new string[3] { "қаз", "рус", "eng" };
        public static string tempForKazText;
        public static string tempForRusText;
        public static string tempForEngText;

        public static readonly string FacebookAppId = "390380841096296"; //171880606320911
        public static string twitterConsumerKey = "cnM6CMx8gubv408S2eUYA";
        public static string twitterConsumerKeySecret = "N1Xo1oXgL19xdN8YHqReBIn2kR7lyA8h1MPSvW4";
        public static string twitterAccessToken { get; set; }
        public static string twitterAccessTokenSecret { get; set; }
    }
}
