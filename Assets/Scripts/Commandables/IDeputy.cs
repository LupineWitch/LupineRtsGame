using Assets.Scripts.Classes.Commands;

namespace Assets.Scripts.Commandables
{
    public interface IDeputy
    {
        public void SetCommand(Command<ICommander, IDeputy> command);
    }
}