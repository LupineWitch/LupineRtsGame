using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Commandables
{
    public class SharedCommandContext : IDeputy
    {
        public IReadOnlyCollection<CommandDirective> AvailableDirectives => sharedDirectives;
        public CommandDirective DefaultDirective => sharedDirectives?.First() ?? null;
        
        private CommandDirective[] sharedDirectives;
        private IEnumerable<IDeputy> selection;

        public void SetCommand(Command<ICommander, IDeputy> command)
        {
            foreach(IDeputy deputy in selection)
                deputy.SetCommand(command);
        }

        public void SetSubcommand(Command<ICommander, IDeputy> command)
        {
            foreach (IDeputy deputy in selection)
                deputy.SetSubcommand(command);
        }

        public SharedCommandContext(IEnumerable<IDeputy> selectedDeputies)
        {
            Dictionary<CommandDirective, int> directivesCounts = new Dictionary<CommandDirective, int>();
            //Define common
            foreach(IDeputy deputy in selectedDeputies)
            {
                foreach (CommandDirective deputyDirective in deputy.AvailableDirectives)
                {
                    if (deputyDirective == default)
                        continue;

                    if (!directivesCounts.ContainsKey(deputyDirective))
                        directivesCounts.Add(deputyDirective, 1);
                    else
                        directivesCounts[deputyDirective]++;
                }
            }

            sharedDirectives = directivesCounts.Where(dc => dc.Value > 1).OrderByDescending(dc => dc.Value).Take(9).Select(dc => dc.Key).ToArray();
        }
    }
}
