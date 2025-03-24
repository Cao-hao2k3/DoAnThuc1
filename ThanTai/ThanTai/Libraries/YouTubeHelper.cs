using System;
using System.Web;

namespace ThanTai.Libraries
{
    public static class YouTubeHelper
    {
        public static string GetYouTubeEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                Uri uri = new Uri(url);
                var query = HttpUtility.ParseQueryString(uri.Query);

                // Trường hợp: https://www.youtube.com/watch?v=VIDEO_ID
                if (uri.Host.Contains("youtube.com") && query["v"] != null)
                {
                    return $"https://www.youtube.com/embed/{query["v"]}";
                }

                // Trường hợp: https://youtu.be/VIDEO_ID
                if (uri.Host.Contains("youtu.be"))
                {
                    return $"https://www.youtube.com/embed{uri.AbsolutePath}";
                }
            }
            catch
            {
                return string.Empty;
            }

            return url;
        }
    }
}
