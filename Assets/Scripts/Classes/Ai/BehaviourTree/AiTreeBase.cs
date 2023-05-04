using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Ai.BehaviourTree
{
    public abstract class AiTreeBase : MonoBehaviour
    {
        private AiTreeNodeBase rootNode = null;

        protected virtual void Start()
        {
            rootNode = SetupTree();
        }

        protected virtual void Update()
        {
            if (rootNode != null)
                rootNode.Evaluate();
        }

        protected abstract AiTreeNodeBase SetupTree();
    }
}
