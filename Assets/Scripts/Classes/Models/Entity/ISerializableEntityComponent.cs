using System;

namespace Assets.Scripts.Classes.Models.Entity
{
    public interface ISerializableEntityComponent
    {
        public Type ComponentsType { get; set; }
        public string ToJsonObject();
    }
}