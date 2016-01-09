using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Avatar
{
    /// <summary>
    /// Represents a sentiment analysis request.
    /// </summary>
    public class AvatarRequest : ServiceMessage
    {
        /// <summary>
        /// The token of the user.
        /// </summary>
        [JsonProperty("token")]
        public string Token;
    }
}
