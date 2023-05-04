using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Ai.BehaviourTree
{
    public class SelectorNode : AiTreeNodeBase
    {
        public override NodeState Evaluate()
        {
            foreach (var child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure: continue;
                    case NodeState.Running: return NodeState.Running;
                    case NodeState.None: return NodeState.None;
                    case NodeState.Success: return NodeState.Success;
                    default: continue; //just go to next 
                }
            }
            //All children failed or node has invalid state
            return children != null && children.Count > 0 ? NodeState.Failure : NodeState.None;
        }
    }
}
