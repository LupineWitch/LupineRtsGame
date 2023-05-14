using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Faction
{
    public enum ControllerType
    {
        AI = 0,
        Player = 1
    }

    public class BaseFaction : MonoBehaviour
    {
        public virtual Color FactionColor => factionColor;
        public virtual Color FactionHighlightColor => factionColor * 0.9f;
        public virtual string FactionName { get => factionName; set => factionName = value; }
        public virtual ControllerType WhoIsControlling { get => whoIsControlling; }

        [SerializeField]
        protected Color factionColor;
        [SerializeField]
        private string factionName;
        [SerializeField]
        private ControllerType whoIsControlling;

        public override bool Equals(object obj)
        {
            return obj is BaseFaction faction &&
                   base.Equals(obj) &&
                   factionName == faction.factionName &&
                   whoIsControlling == faction.whoIsControlling;
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), factionName, whoIsControlling);
    }
}
