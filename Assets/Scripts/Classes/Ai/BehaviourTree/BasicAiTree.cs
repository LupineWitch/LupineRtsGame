using Assets.Scripts.Classes.Ai.BasicAiBehaviour;
using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Ai.BehaviourTree
{
    public class BasicAiTree : AiTreeBase
    {
        private BasicAIController controller;
        public BasicAiTree(BasicAIController aiController)
        {
            controller = aiController;
        }

        protected override AiTreeNodeBase SetupTree()
        {
            var rootNode = new CollectResourcesNode("wood");
            rootNode.SetData("AiController", controller);
            return rootNode;
        }
    }
}
