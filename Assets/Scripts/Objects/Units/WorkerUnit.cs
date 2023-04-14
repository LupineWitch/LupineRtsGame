using Assets.Scripts.Classes.GameData;
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
        public Tuple<RtsResource, int> CarriedResource { get; set; }
        public float GatheringEfficiency { get; set; } = 1f;

        protected override void Start()
        {
            base.Awake();
            DisplayLabel = "Worker";
            defaultDirective = new MoveCollectDirective();
            menuActions[0] = defaultDirective;
        }
    }
}
