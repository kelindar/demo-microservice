using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emitter;
using Newtonsoft.Json;
using Service.Avatar;

namespace Service.Users
{
    public class UserService : ServiceBase
    {
        /// <summary>
        /// Main entry point to the program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            new UserService().Start();
        }

        /// <summary>
        /// Main execution loop of the service.
        /// </summary>
        protected override void Run()
        {
            // Hook db events
            UserManager.Default.UserCreated += OnUserCreate;
            UserManager.Default.UserJoined += OnUserJoin;
            UserManager.Default.UserLeft += OnUserLeave;

            // Subscribe to the requests
            Emitter.On(Key.UserPing, "user-ping/v1/", OnPing);
            Emitter.On(Key.AvatarResponse, "avatar-response/v1/", OnAvatarResponse);

            while(true)
            {
                UserManager.Default.CheckUsers();
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Occurs when a new user was created.
        /// </summary>
        private void OnUserCreate(User user)
        {
            // Create a request for the avatar
            var request = new AvatarRequest()
            {
                Token = user.Token
            };

            // Publish the request
            Emitter.Publish(Key.AvatarRequest, "avatar-request/v1/", request.Serialize());
        }

        /// <summary>
        /// Occurs when a new user was created.
        /// </summary>
        private void OnUserJoin(User user)
        {
            Emitter.Publish(Key.UserNotify, "user-notify/v1/join/", user.Serialize());
        }

        /// <summary>
        /// Occurs when a new user was created.
        /// </summary>
        private void OnUserLeave(User user)
        {
            Emitter.Publish(Key.UserNotify, "user-notify/v1/leave/", user.Serialize());
        }

        /// <summary>
        /// Occurs when a new request is received.
        /// </summary>
        private void OnPing(string channel, byte[] message)
        {
            var ping = ServiceMessage.Deserialize<UserPing>(message);
            var user = UserManager.Default.PingUser(ping.Token);

            // If user is already active but requets to greet, resend the join
            if (user.Active && ping.Greet)
                Emitter.Publish(Key.UserNotify, "user-notify/v1/join/", user.Serialize());
        }

        /// <summary>
        /// Occurs when a new request is received.
        /// </summary>
        private void OnAvatarResponse(string channel, byte[] message)
        {
            // Deserialize the response
            var response = ServiceMessage.Deserialize<AvatarResponse>(message);

            // Update the user info
            UserManager.Default.UpdateUserInfo(response.Token, response.Avatar, response.Name);
        }
    }
}
