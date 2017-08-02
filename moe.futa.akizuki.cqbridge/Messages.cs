using System;
using System.Reflection;
using Newtonsoft.Json;

namespace moe.futa.akizuki.cqbridge.Messages
{
    [MessageType("group")]
    internal class GroupMessage : UserGeneratedMessage
    {
        public readonly String GroupId;

        public GroupMessage(String content, String groupId, Int32 time, String userId) : base(content, time, userId)
        {
            GroupId = groupId;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class MessageType : Attribute
    {
        public readonly String Name;

        public MessageType(String name)
        {
            Name = name;
        }
    }

    [MessageType("private")]
    internal class PrivateMessage : UserGeneratedMessage
    {
        public PrivateMessage(String content, Int32 time, String userId) : base(content, time, userId)
        {
        }
    }
    
    internal abstract class UserGeneratedMessage : SerializableMessage
    {
        public readonly String Content;
        public readonly String UserId;

        protected UserGeneratedMessage(String content, Int32 time, String userId) : base(time)
        {
            Content = content;
            UserId = userId;
        }
    }

    internal abstract class SerializableMessage
    {
        public readonly Int32 Time;
        public readonly String Type;

        protected SerializableMessage(Int32 time)
        {
            Time = time;
            Type = GetType().GetCustomAttribute<MessageType>().Name;
        }

        public String ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}