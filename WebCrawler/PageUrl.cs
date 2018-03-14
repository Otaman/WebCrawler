using System;

namespace WebCrawler
{
    public struct PageUrl : IEquatable<PageUrl>
    {
        public int Depth { get; }
        public string ParentUrl { get; }
        public string CurrentUrl { get; }

        public PageUrl(int depth, string parentUrl, string currentUrl)
        {
            Depth = depth;
            ParentUrl = parentUrl;
            CurrentUrl = currentUrl;
        }

        public bool Equals(PageUrl other)
        {
            return Depth == other.Depth
                   && string.Equals(ParentUrl, other.ParentUrl)
                   && string.Equals(CurrentUrl, other.CurrentUrl);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            
            return obj is PageUrl && Equals((PageUrl) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Depth;
                hashCode = (hashCode * 397) ^ (ParentUrl != null ? ParentUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CurrentUrl != null ? CurrentUrl.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}