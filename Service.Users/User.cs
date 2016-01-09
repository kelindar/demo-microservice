using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Users
{
    /// <summary>
    /// Represents user.
    /// </summary>
    public class User : ServiceMessage
    {
        /// <summary>
        /// The token of the user.
        /// </summary>
        [JsonProperty("token")]
        public string Token;

        /// <summary>
        /// The name of the user.
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// The avatar of the user.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar;

        /// <summary>
        /// The avatar of the user.
        /// </summary>
        [JsonIgnore]
        public DateTime LastPing;

        /// <summary>
        /// Whether this user is active or not.
        /// </summary>
        [JsonProperty("active")]
        public bool Active;

        #region Static Members
        /// <summary>
        /// Sequence for the id.
        /// </summary>
        private static int SequenceId = 0;

        /// <summary>
        /// Creates a new user.
        /// </summary>
        internal static User Create(string token)
        {
            return new User()
            {
                Id = (++SequenceId),
                Token = token,
                LastPing = DateTime.UtcNow,
                Active = false
            };
        }

        #endregion
    }
}
