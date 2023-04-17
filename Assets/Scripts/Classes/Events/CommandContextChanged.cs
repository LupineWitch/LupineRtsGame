using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;


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
