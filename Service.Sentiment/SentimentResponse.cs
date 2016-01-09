using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Sentiment
{
    /// <summary>
    /// Represents a sentiment analysis response.
    /// </summary>
    public class SentimentResponse : ServiceMessage
    {
        /// <summary>
        /// Total sentiment score of the tokens
        /// </summary>
        [JsonProperty("sentiment")]
        public int Sentiment;

        /// <summary>
        /// Words that had negative sentiment
        /// </summary>
        [JsonProperty("negative")]
        public string[] Negative;

        /// <summary>
        /// Words that had positive sentiment
        /// </summary>
        [JsonProperty("positive")]
        public string[] Positive;
    }
}
