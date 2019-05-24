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

    [DebuggerDisplay(@"[{Type}] {RelativeDirectory} ({string.Join("" "", Tags)")]
    public sealed class RepositoryDefinition
    {
        private IList<string> _tags;

        [JsonProperty("type")]
        public string Type { get; internal set; }

        [JsonProperty("dir")]
        public string RelativeDirectory { get; internal set; }

        [JsonProperty("tags")]
        public IList<string> Tags
        {
            get => _tags ?? (_tags = new List<string>());
            internal set => _tags = value;
        }

        public bool HasTags(IEnumerable<string> tags)
        {
            return tags.All(tag => Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
