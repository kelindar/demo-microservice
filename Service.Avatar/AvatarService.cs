using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using RandomNameGeneratorLibrary;

namespace Service.Avatar
{
    public class AvatarService : ServiceBase
    {
        private readonly PersonNameGenerator NameGenerator 
            = new PersonNameGenerator();

        /// <summary>
        /// A queue for processing requests.
        /// </summary>
        private readonly ConcurrentQueue<AvatarRequest> RequestQueue
            = new ConcurrentQueue<AvatarRequest>();

        /// <summary>
        /// Main entry point to the program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            new AvatarService().Start();
        }

        /// <summary>
        /// Main execution loop of the service.
        /// </summary>
        protected override void Run()
        {
            // Subscribe to the avatar request channel
            Emitter.On(Key.AvatarRequest, "avatar-request/v1/", OnRequest);

            // Process requests
            while (true)
            {
                // Dequeue the request
                var request = RequestQueue.Dequeue();
                if (request == null)
                    continue;

                // Create a response
                var response = new AvatarResponse()
                {
                    Id = request.Id,
                    Token = request.Token,
                    Avatar = AvatarRenderer.CreateAvatar(request.Token),
                    Name = NameGenerator.GenerateRandomFirstAndLastName()
                };

                // Publish the response
                Emitter.Publish(Key.AvatarResponse, "avatar-response/v1/", response.Serialize());
            }
        }

        /// <summary>
        /// Occurs when a new request is received.
        /// </summary>
        private void OnRequest(string channel, byte[] message)
        {
            // Enqueue the request
            RequestQueue.Enqueue(
                ServiceMessage.Deserialize<AvatarRequest>(message)
                );
        }

    }
}
