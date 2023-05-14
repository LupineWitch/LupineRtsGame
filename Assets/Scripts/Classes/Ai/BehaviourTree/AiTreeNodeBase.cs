using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Ai.BehaviourTree
{
    public enum NodeState
    {
        None,
        Running,
        Failure,
        Success
    }

    public abstract class AiTreeNodeBase : IComparable<AiTreeNodeBase>
    {
        public AiTreeNodeBase Parent { get; set; } = null;
        public int Weigth { get; set; } = 1;

        protected NodeState state;
        protected List<AiTreeNodeBase> children = new List<AiTreeNodeBase>();

        private Dictionary<string, object> nodeData = new Dictionary<string, object>();

        public AiTreeNodeBase() { }

        public AiTreeNodeBase(IEnumerable<AiTreeNodeBase> children) => this.children.AddRange(children);

        public AiTreeNodeBase(params AiTreeNodeBase[] children) => this.children.AddRange(children);

        public AiTreeNodeBase(List<AiTreeNodeBase> children) => this.children = children ?? throw new ArgumentNullException(nameof(children));

        public abstract NodeState Evaluate();

        public void Attach(AiTreeNodeBase nodeToAttach)
        {
            nodeToAttach.Parent = this;
            children.Add(nodeToAttach);
        }

        public void Attach(IEnumerable<AiTreeNodeBase> nodesToAttach)
        {
            foreach (var node in nodesToAttach)
            {
                node.Parent = this;
                children.Add(node);
            }
        }

        public void SetData(string key, object dataObject) => nodeData[key] = dataObject;

        public void SetDataAtRoot(string key, object dataObject)
        {
            if (this.Parent == null) //we are at the root
                SetData(key, dataObject);
            else // traverse tree up to the root 
                this.Parent.SetDataAtRoot(key, dataObject);
        }
        
        public void SetDataUpNode(string key, object dataObject, int upLevel)
        {
            if (upLevel == 0) //we arrived at proper level
                SetData(key, dataObject);
            // traverse tree up to the root 
            if(this.Parent == null)
                throw new ArgumentException($"{nameof(upLevel)} argument is is too big, tries to go above root. Current level: {upLevel}");

            this.Parent.SetDataUpNode(key, dataObject, upLevel - 1);
        }

        public object GetData(string key)
        {
            object data = null;
            if (nodeData.TryGetValue(key, out data)) return data;

            AiTreeNodeBase parentNode = this.Parent;
            if(parentNode != null) data = parentNode.GetData(key);

            return data;
        }

        public bool RemoveData(string key) 
        {
            if(nodeData.ContainsKey(key))
            {
                nodeData.Remove(key);
                return true;
            }

            AiTreeNodeBase parentNode = this.Parent;
            if(parentNode != null)
                return parentNode.RemoveData(key);

            return false;
        }

        public int CompareTo(AiTreeNodeBase other) => this.Weigth.CompareTo(other.Weigth);

    }
}
