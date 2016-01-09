using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emitter;
using Newtonsoft.Json;

namespace Service.Users
{
    public class UserManager 
    {
        public readonly static UserManager Default 
            = new UserManager();

        private readonly ConcurrentDictionary<string, User> Users =
            new ConcurrentDictionary<string, User>();

        public event Action<User> UserCreated;
        public event Action<User> UserJoined;
        public event Action<User> UserLeft;

        /// <summary>
        /// Updates the key on ping.
        /// </summary>
        public User PingUser(string key)
        {
            return this.Users.AddOrUpdate(key, CreateUser, UpdateUserActivity);
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        private User CreateUser(string key)
        {
            // If we don't have the user, create it
            var user = User.Create(key);

            // Invoke the create event
            if (this.UserCreated != null)
                this.UserCreated(user);
            return user;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        private User UpdateUserActivity(string key, User user)
        {
            // Update the last time we've pinged it.
            user.LastPing = DateTime.UtcNow;
            return user;
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        public User UpdateUserInfo(string key, string avatar, string name)
        {
            User user;
            if (!this.Users.TryGetValue(key, out user))
                return null;

            user.Avatar = avatar;
            user.Name = name;
            user.Active = true;

            // Invoke user join event now
            if (this.UserJoined != null)
                this.UserJoined(user);
            return user;
        }

        /// <summary>
        /// Checks all users and notifies if someone has left.
        /// </summary>
        public void CheckUsers()
        {
            var now = DateTime.UtcNow;
            foreach (var user in this.Users.Values)
            {
                // Check if the user is active
                var active = now - user.LastPing < TimeSpan.FromSeconds(10) && user.Avatar != null;
                if (active == user.Active)
                    continue;

                // Set the state and notify
                user.Active = active;
                if (active)
                {
                    if (this.UserJoined != null)
                        this.UserJoined(user);
                }
                else
                {
                    if (this.UserLeft != null)
                        this.UserLeft(user);
                }
            }
        }

    }
}
