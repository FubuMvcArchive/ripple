using System;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using NuGet;

namespace ripple.Model
{
    public class VersionToken
    {
        public static readonly VersionToken Current = new VersionToken("Current", x => x);
        public static readonly VersionToken CurrentMajor = new VersionToken("CurrentMajor", x=>new SemanticVersion(x.Version.Major,0,0,0));
        public static readonly VersionToken NextMajor = new VersionToken("NextMajor", findNextMajor);
        public static readonly VersionToken NextMinor = new VersionToken("NextMinor", findNextMin);
        
        private static readonly FieldInfo[] Fields;
        private static readonly Cache<string, VersionToken> Tokens;
 
        static VersionToken()
        {
            Fields = typeof (VersionToken).GetFields(BindingFlags.Static | BindingFlags.Public);

            Tokens = new Cache<string, VersionToken>(key =>
            {
                var field = Fields.SingleOrDefault(x => StringExtensions.EqualsIgnoreCase(x.Name, key));
                if (field == null)
                {
                    throw new ArgumentOutOfRangeException("key", key + " is an invalid version token");
                }

                return field.GetValue(null).As<VersionToken>();
            });
        }

        private readonly string _key;
        private readonly Func<SemanticVersion, SemanticVersion> _version;

        public VersionToken(string key, Func<SemanticVersion, SemanticVersion> version)
        {
            _key = key;
            _version = version;
        }

        public string Key { get { return _key; } }

        public SemanticVersion Value(SemanticVersion version)
        {
            return _version(version);
        }

        protected bool Equals(VersionToken other)
        {
            return string.Equals(_key, other._key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VersionToken) obj);
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        private static SemanticVersion findNextMajor(SemanticVersion semantic)
        {
            var version = semantic.Version;
            return new SemanticVersion(version.Major + 1, 0, 0, 0);
        }

        private static SemanticVersion findNextMin(SemanticVersion semantic)
        {
            var version = semantic.Version;
            return new SemanticVersion(version.Major, version.Minor + 1, 0, 0);
        }

        public static VersionToken Find(string key)
        {
            return Tokens[key];
        }
    }
}