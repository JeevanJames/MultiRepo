
using Newtonsoft.Json;

namespace Core
{
    public sealed class MarkerFile
    {
        [JsonProperty("localDir", Required = Required.Always)]
        public string LocalDirectory { get; set; }

        [JsonProperty("repo", Required = Required.Always)]
        public string RepositoryUrl { get; set; }

        [JsonProperty("branch", Required = Required.DisallowNull)]
        public string Branch { get; set; }

        [JsonProperty("repoDir", Required = Required.DisallowNull)]
        public string RepositoryDirectory { get; set; }
    }
}
