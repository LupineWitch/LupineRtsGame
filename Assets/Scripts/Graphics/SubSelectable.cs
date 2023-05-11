using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using Assets.Scripts.Faction;
using System;
using UnityEngine;

namespace Assets.Scripts.Graphics
{
    /// <summary>
    /// Serves to delegate all interface members to it's parent object
    /// </summary>
    public class SubSelectable : MonoBehaviour, ISelectable
    {
        public Sprite Preview
        {
            get => parent.Preview;
            set => parent.Preview = value;
        }
        public string DisplayLabel { get => parent.DisplayLabel; set => parent.DisplayLabel = value; }

        public BaseFaction Faction => parent.Faction;

        public bool Highlighted => parent.Highlighted;
        public event SelectedEvent Selected;

        private ISelectable parent;

        public void Awake()
        {
            parent = gameObject.GetComponentInParent<ISelectable>();
            parent.Selected += (sender, args) => parent.RaiseSelectedEvent(sender, args);
        }

        public bool CanBeSelectedBy(CommandControllerBase selector) => parent.CanBeSelectedBy(selector);

        public bool IsSelectedBy(CommandControllerBase possibleOwner) => parent.IsSelectedBy(possibleOwner);

        public bool TrySelect(CommandControllerBase selector) => parent.TrySelect(selector);

        public bool TryUnselect(CommandControllerBase selector) => parent.TryUnselect(selector);

        public void RaiseSelectedEvent(object sender, EventArgs e)
        {
            Selected?.Invoke(sender as ISelectable, e as SelectedEventArgs);
        }

        public void HighlightEntity(bool enable) => parent.HighlightEntity(enable);
    }
}
