using System;

namespace Assets.Scripts.Classes.GameData
{
    public class RtsResource
    {
        public string IdName { get; private set; }
        public string DisplayName { get; set; }

        public RtsResource(string idName)
        {
            IdName = idName;
        }

        public override bool Equals(object obj)
        {
            return obj is RtsResource resource &&
                   IdName == resource.IdName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IdName);
        }
    }
}
