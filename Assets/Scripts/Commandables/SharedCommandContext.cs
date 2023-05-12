using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Commandables.Directives;
using System.Collections.Generic;
using System.Linq;

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
            foreach (IDeputy deputy in selection)
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
            var deputies = selectedDeputies.Where(sd => sd is IDeputy);
            //Define common
            foreach (IDeputy deputy in deputies)
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
