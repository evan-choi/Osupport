using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Osupport.Beatmap.Net
{
    internal sealed class QueryStringBuilder
    {
        private readonly List<KeyValuePair<string, string>> _parameters = new List<KeyValuePair<string, string>>();

        public QueryStringBuilder Add(string key, object value)
        {
            _parameters.Add(new KeyValuePair<string, string>(key, value.ToString()));
            return this;
        }

        public QueryStringBuilder AddArray(string key, IEnumerable<object> values)
        {
            foreach (var value in values)
            {
                Add(key, value);
            }

            return this;
        }

        public string Build()
        {
            var builder = new StringBuilder();

            foreach (KeyValuePair<string, string> parameter in _parameters)
            {
                if (builder.Length > 0)
                    builder.Append("&");

                builder.Append(HttpUtility.UrlEncode(parameter.Key));
                builder.Append('=');
                builder.Append(HttpUtility.UrlEncode(parameter.Value));
            }

            return builder.ToString();
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
