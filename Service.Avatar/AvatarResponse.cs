using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Avatar
{
    /// <summary>
    /// Represents a sentiment analysis response.
    /// </summary>
    public class AvatarResponse : ServiceMessage
    {
        /// <summary>
        /// The token of the user.
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// The generated avatar.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar;

        /// <summary>
        /// The generated name.
        /// </summary>
        [JsonProperty("name")]
        public string Name;
    }
}
