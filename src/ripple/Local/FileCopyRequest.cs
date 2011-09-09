using FubuCore;

namespace ripple.Local
{
    public class FileCopyRequest
    {
        public FileSet Matching { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public bool Equals(FileCopyRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Matching, Matching) && Equals(other.From, From) && Equals(other.To, To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FileCopyRequest)) return false;
            return Equals((FileCopyRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Matching != null ? Matching.GetHashCode() : 0);
                result = (result * 397) ^ (From != null ? From.GetHashCode() : 0);
                result = (result * 397) ^ (To != null ? To.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("Matching: {0}, From: {1}, To: {2}", Matching, From, To);
        }
    }
}