using Assets.Scripts.Managers;

namespace Assets.Scripts.Commandables
{
    public interface ICommander
    {
        public abstract MapManager MapManager { get; }
    }
}