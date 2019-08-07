using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

namespace Core
{
    public sealed class Manifest
    {
        [JsonProperty("repos")]
        public IDictionary<string, RepositoryDefinition> Repositories { get; } = new Dictionary<string, RepositoryDefinition>();
    }

    [DebuggerDisplay(@"[{Type}] {RepositoryLocation} ({string.Join("" "", Tags)})")]
    public sealed class RepositoryDefinition
    {
        private IList<string> _tags;

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; internal set; }

        [JsonProperty("repo")]
        public string RepositoryLocation { get; internal set; }

        [JsonProperty("tags")]
        public IList<string> Tags
        {
            get => _tags ?? (_tags = new List<string>());
            internal set => _tags = value;
        }

        public bool HasAllTags(IEnumerable<string> tags)
        {
            return tags.All(tag => Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }

        public bool HasAnyTag(IEnumerable<string> tags)
        {
            return tags.Any(tag => Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
