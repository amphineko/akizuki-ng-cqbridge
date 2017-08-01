using System;
using Newtonsoft.Json;

namespace moe.futa.akizuki.cqbridge.Messages
{
    internal class GroupMessage : UserGeneratedMessage
    {
        public readonly String GroupId;

        public GroupMessage(String content, String groupId, Int32 time, String userId) : base(content, time, userId)
        {
            GroupId = groupId;
        }
    }

    internal class PrivateMessage : UserGeneratedMessage
    {
        public PrivateMessage(String content, Int32 time, String userId) : base(content, time, userId)
        {
        }
    }

    internal class UserGeneratedMessage : SerializableMessage
    {
        public readonly String Content;
        public readonly String UserId;

        public UserGeneratedMessage(String content, Int32 time, String userId) : base(time)
        {
            Content = content;
            UserId = userId;
        }
    }

    internal abstract class SerializableMessage
    {
        public readonly Int32 Time;

        protected SerializableMessage(Int32 time)
        {
            Time = time;
        }

        public String ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}