using Assets.Scripts.Classes.Ai.BehaviourTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Ai.BasicAiBehaviour
{
    public class PlayNode : AiTreeNodeBase
    {
        public override NodeState Evaluate() => NodeState.Running;
    }
}
