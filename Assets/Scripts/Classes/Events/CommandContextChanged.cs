using Assets.Scripts.Commandables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Classes.Events
{

    public delegate void CommandContextChangedEvent(object sender, CommandContextChangedArgs args);

    public class CommandContextChangedArgs : EventArgs
    {
        public IReadOnlyCollection<CommandDirective> MenuCommands { get; protected set; }

        public CommandContextChangedArgs(IReadOnlyCollection<CommandDirective> menuCommands)
        {
            MenuCommands = menuCommands;
        }
    }
}
