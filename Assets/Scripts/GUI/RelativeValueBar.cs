using Assets.Scripts.Classes.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.GUI
{
    public class RelativeValueBar : MonoBehaviour
    {
        public float Value => value;

        public float MinValue 
        { 
            get => minValue;
            set 
            {
                minValue = value;
                RefreshFillBarPosition();
            }
        }
        public float MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                RefreshFillBarPosition();

            }
        }

        [SerializeField]
        private float rightOffset = 0.04f;
        [SerializeField]
        private float leftOffset = 0.03f;
        [SerializeField]
        private RectTransform fillBar;
        [SerializeField]
        private float value = 0f;
        [SerializeField]
        private float minValue = 0f;
        [SerializeField]
        private float maxValue = 100f;

        private float fraction => value / MaxValue;


        void Awake()
        {
            RefreshFillBarPosition();
        }

        private void OnEnable()
        {
            RefreshFillBarPosition();
        }

        public void SetValue(float value)
        {
            float temp = Mathf.Min( value, MaxValue);
            temp = Mathf.Max( temp, MinValue );
            this.value = temp;
            RefreshFillBarPosition();
        }

        private void RefreshFillBarPosition()
        {
            float newX = rightOffset + fraction - leftOffset;
            fillBar.anchorMax = new Vector2(newX, fillBar.anchorMax.y);
        }

        public void IncrementValue(float value)
        {
            value += value;
            RefreshFillBarPosition();
        }

        public void RespondToUpdatedValue(object sender, ValueUpdatedEventArgs args)
        {
            if (args != null)
                SetValue(args.ChangedTo);
        }
    }
}
