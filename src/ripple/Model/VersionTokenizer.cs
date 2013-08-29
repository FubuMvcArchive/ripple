using System.Collections.Generic;
using System.Linq;
using NuGet;
using ripple.Model.Versioning;

namespace ripple.Model
{
    public class VersionTokenizer
    {
        private readonly IList<char> _characters = new List<char>();
        private readonly IList<IVersionRule> _rules = new List<IVersionRule>();
        private IVersionMode _mode;
        private string _token;

        public IEnumerable<IVersionRule> Tokenize(string value)
        {
            _rules.Clear();
            _characters.Clear();

            _mode = new TokenSearch(this);
            value.Each(x => _mode.Read(x));

            _mode.End();

            return _rules;
        }

        private string endToken()
        {
            var token = new string(_characters.ToArray()).Trim();
            _characters.Clear();

            return token;
        }

        public interface IVersionMode
        {
            void Read(char c);
            void End();
        }

        public class TokenSearch : IVersionMode
        {
            private readonly VersionTokenizer _tokenizer;

            public TokenSearch(VersionTokenizer tokenizer)
            {
                _tokenizer = tokenizer;
            }

            public void Read(char c)
            {
                if (char.IsDigit(c))
                {
                    _tokenizer._token = _tokenizer.endToken();
                    _tokenizer._mode = new VersionSearch(_tokenizer);
                    _tokenizer._mode.Read(c);
                    return;
                }

                _tokenizer._characters.Add(c);
            }

            public void End()
            {
            }
        }

        public class VersionSearch : IVersionMode
        {
            private readonly VersionTokenizer _tokenizer;

            public VersionSearch(VersionTokenizer tokenizer)
            {
                _tokenizer = tokenizer;
            }

            public void Read(char c)
            {
                if (c == ',' || char.IsWhiteSpace(c))
                {
                    End();
                    _tokenizer._mode = new TokenSearch(_tokenizer);
                    return;
                }

                _tokenizer._characters.Add(c);
            }

            public void End()
            {
                var version = SemanticVersion.Parse(_tokenizer.endToken());
                var builder = DependencyVersion.VersionRules[_tokenizer._token];
                var rule = builder(version);

                _tokenizer._rules.Add(rule);

                _tokenizer._token = null;
            }
        }
    }
}