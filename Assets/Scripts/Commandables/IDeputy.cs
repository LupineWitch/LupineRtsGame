using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using System.Collections.Generic;

namespace Assets.Scripts.Commandables
{
    public interface IDeputy
    {
        public void SetCommand(Command<ICommander, IDeputy> command);
        public IReadOnlyCollection<CommandDirective> AvailableDirectives { get; }
        public CommandDirective DefaultDirective { get;}
    }
}