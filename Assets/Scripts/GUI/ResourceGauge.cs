using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.GUI
{
    public class ResourceGauge : MonoBehaviour
    {
        /// <summary>
        /// An 
        /// <see cref="Classes.GameData.RtsResource.IdName" /> property value of <see cref="Classes.GameData.RtsResource" to display />
        /// </summary>
        public string ForResource { get => forResource; }

        [SerializeField]
        private string forResource;
        [SerializeField]
        private TextMeshProUGUI counter;

        private int resourceAmount = 0;

        public void ResourceValueChanged(int to)
        {
            resourceAmount = to;
            counter.text = resourceAmount.ToString();
        }
    }
}
