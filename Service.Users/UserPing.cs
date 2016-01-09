using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Users
{
    /// <summary>
    /// Represents a user ping request.
    /// </summary>
    public class UserPing : ServiceMessage
    {
        /// <summary>
        /// The user token.
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// Whether this is the first ping or not.
        /// </summary>
        [JsonProperty("greet")]
        public bool Greet;

    }
}
