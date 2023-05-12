using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Models.Entity;
using Assets.Scripts.Commandables;
using Assets.Scripts.Faction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.ResourceNodes
{
    public class ResourceNodeBase : MonoBehaviour, ISerializableEntityComponent, ISelectable
    {
        public float TimeToGather = 4f;//s

        [JsonProperty]
        public virtual Type ComponentsType { get => this.GetType(); }
        public RtsResource Resource { get => resource; set => resource = value; }
        public int Amount { get => amount; set => amount = value; }
        public bool CanBeMined => Resource != null && Amount > 0;
        public string PrefabName { get => prefabName; set => prefabName = value; }
        public Sprite Preview { get => preview; set => preview = value; }
        public string DisplayLabel { get => displayLabel; set => displayLabel = value; }

        public event SelectedEvent Selected;
        public bool Highlighted => highlighted;
        public BaseFaction Faction
        {
            get
            {
                if (this.faction == null)
                    faction = this.GetReferenceManagerInScene().FactionContainer.transform
                                  .Find("Ambient")
                                  .GetComponent<BaseFaction>();

                return faction;
            }
        }

        [SerializeField]
        [JsonProperty]
        private string resourceId;
        [SerializeField]
        [JsonProperty]
        private int amount;
        [SerializeField]
        private string prefabName;
        [SerializeField]
        private string displayLabel;
        private RtsResource resource;
        private bool highlighted;
        private bool selected;
        private BaseFaction faction;
        private Sprite preview;

        protected virtual void Awake()
        {
            if (preview == null)
                preview = gameObject.GetComponent<SpriteRenderer>().sprite;

            Resource = new RtsResource(resourceId);
        }

        public bool CanBeSelectedBy(CommandControllerBase selector) => selector.Faction.WhoIsControlling == ControllerType.Player;
        public void HighlightEntity(bool enable)
        {
            highlighted = enable && !this.selected;
        }

        public bool IsSelectedBy(CommandControllerBase possibleOwner) => selected;


        public virtual int TryGather(int howMuch)
        {
            int ableToGet = Math.Min(Amount, howMuch);
            Amount = Math.Max(0, Amount - ableToGet);
            return ableToGet;
        }

        public bool TrySelect(CommandControllerBase selector)
        {
            selected = true;
            this.Selected?.Invoke(this, new SelectedEventArgs(selector, true));
            return true;
        }
        public bool TryUnselect(CommandControllerBase selector)
        {
            selected = false;
            this.Selected?.Invoke(this, new SelectedEventArgs(selector, false));
            return true;
        }
        public void RaiseSelectedEvent(object sender, EventArgs e) => this.Selected?.Invoke(sender as ISelectable, e as SelectedEventArgs);
    }
}
