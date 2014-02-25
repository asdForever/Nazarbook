using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlmaCloud.Classes.test
{
    public class TwitterSettings
    {
        public static string ConsumerKey = "cnM6CMx8gubv408S2eUYA";
        public static string ConsumerKeySecret = "N1Xo1oXgL19xdN8YHqReBIn2kR7lyA8h1MPSvW4";
        public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string OAuthVersion = "1.1";
        public static string CallbackUri = "http://www.bing.com";
        public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string TwitterAccess = "TweetSamples"; // settings name
    }
    public class TwitterAccess
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string UserId { get; set; }
        public string ScreenName { get; set; }
    }
}
