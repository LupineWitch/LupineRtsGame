using Assets.Scripts.Classes.Models.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Entity
{
    public class JsonEntityCustomPropertyResolver : DefaultContractResolver
    {
        private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return objectType.GetMembers(bindingFlags).Where(member => member.GetCustomAttributes(typeof(JsonPropertyAttribute)).Any()).ToList();
        }
    }
}