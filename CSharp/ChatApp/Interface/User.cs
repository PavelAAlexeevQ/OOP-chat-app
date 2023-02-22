using System;
using System.Collections.Generic;
using ChatApp.Constant;

namespace ChatApp.Interface
{
    public interface IUser
    {
        string id { get; set; }
        string name { get; set; }
    }

    public class Users
    {
        private readonly Dictionary<string, IUser> usersByIdMap = new Dictionary<string, IUser>();
        private readonly HashSet<string> activeUserNamesSet = new HashSet<string>();
        private readonly HashSet<string> reservedUserNamesSet = new HashSet<string>(new[] { SERVICE_USER_NAME.admin });

        public IUser AddUser(string id, string name)
        {
            name = name.Trim().ToLower();

            var isNameReserved = reservedUserNamesSet.Contains(name);

            if (isNameReserved)
            {
                throw new Exception($"Name {name} cannot be used as a username");
            }

            var isUserExists = activeUserNamesSet.Contains(name);

            if (isUserExists)
            {
                throw new Exception($"User {name} is already exists");
            }

            var user = new User { id = id, name = name };

            usersByIdMap.Add(id, user);
            activeUserNamesSet.Add(name);
            return user;
        }

        public IUser? RemoveUser(string id)
        {
            if (usersByIdMap.TryGetValue(id, out var user))
            {
                activeUserNamesSet.Remove(user.name);
                usersByIdMap.Remove(id);
                return user;
            }

            return null;
        }

        public IUser GetUserById(string id)
        {
            if (usersByIdMap.TryGetValue(id, out var user))
            {
                return user;
            }

            throw new Exception($"User {id} is not exists");
        }

        public IEnumerable<IUser> GetAllUsers() => usersByIdMap.Values;

        private class User : IUser
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}