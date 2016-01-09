using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emitter;
using Newtonsoft.Json;

namespace Service
{
    /// <summary>
    /// Represents a base class for every message.
    /// </summary>
    public abstract class ServiceMessage
    {
        /// <summary>
        /// Gets or sets an id for this request.
        /// </summary>
        [JsonProperty("id")]
        public int Id;


        /// <summary>
        /// Serializes the response.
        /// </summary>
        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Deserializes a request.
        /// </summary>
        /// <param name="message">The message content to deserialize</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] message)
            where T : ServiceMessage
        {
            // Deserialize the request
            var request = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message));

            // Print out and return the object
            Console.WriteLine("Request: " + request);
            return request;
        }

        /// <summary>
        /// Converts the request to a string representation.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
