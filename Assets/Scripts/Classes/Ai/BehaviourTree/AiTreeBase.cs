using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Ai.BehaviourTree
{
    public abstract class AiTreeBase
    {
        private AiTreeNodeBase rootNode = null;

        public virtual void InitializeTree()
        {
            rootNode = SetupTree();
        }

        public virtual NodeState RunTree()
        {
            if (rootNode != null)
               return rootNode.Evaluate();
            else
                return NodeState.None;
        }

        protected abstract AiTreeNodeBase SetupTree();
    }
}
