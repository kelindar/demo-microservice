using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emitter;
using Newtonsoft.Json;

namespace Service.Sentiment
{
    public class SentimentService : ServiceBase
    {
        // A queue for processing requests
        private readonly ConcurrentQueue<SentimentRequest> RequestQueue 
            = new ConcurrentQueue<SentimentRequest>();

        /// <summary>
        /// Main entry point to the program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            new SentimentService().Start();
        }

        /// <summary>
        /// Main execution loop of the service.
        /// </summary>
        protected override void Run()
        {
            // Subscribe to the requests
            Emitter.On(Key.SentimentRequest, "sentiment-request/v1/", OnRequest);

            // Process requests
            while (true)
            {
                // Dequeue the request
                var request = RequestQueue.Dequeue();
                if (request == null)
                    continue;

                // Perform the analysis
                var result = SentimentAnalysis.Analyze(request.Text);

                // Create a response
                var response = new SentimentResponse()
                {
                    Id = request.Id,
                    Sentiment = result.Sentiment,
                    Positive = result.Positive.ToArray(),
                    Negative = result.Negative.ToArray(),
                };
                
                // Publish the response
                Emitter.Publish(Key.SentimentResponse, "sentiment-response/v1/" + request.ReplyTo, response.Serialize());
            }
        }

        /// <summary>
        /// Occurs when a new request is received.
        /// </summary>
        private void OnRequest(string channel, byte[] message)
        {
            // Enqueue the request
            RequestQueue.Enqueue(
                ServiceMessage.Deserialize<SentimentRequest>(message)
                );
        }
    }
}
