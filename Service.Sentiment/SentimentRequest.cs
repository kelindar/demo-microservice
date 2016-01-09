using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Sentiment
{
    /// <summary>
    /// Represents a sentiment analysis request.
    /// </summary>
    public class SentimentRequest : ServiceMessage
    {
        /// <summary>
        /// Gets or sets the text to analyze.
        /// </summary>
        [JsonProperty("text")]
        public string Text;

        /// <summary>
        /// Gets or sets the channel where we should reply.
        /// </summary>
        [JsonProperty("reply")]
        public string ReplyTo;
    }


}
