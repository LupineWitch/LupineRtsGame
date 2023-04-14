using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Objects.Units
{
    public class WorkerUnit : BasicUnitScript
    {
        public int Capacity { get; set; } = 15;
        public float GatheringEfficiency { get; set; } = 1f;

        protected override void Awake()
        {
            base.Awake();
            DisplayLabel = "Worker";
            defaultDirective = new MoveCollectDirective();
            menuActions[1] = DefaultDirective;

            //TODO:
            //default action: move/collect
            //0 move/collect
            //1 collect
        }
    }
}
