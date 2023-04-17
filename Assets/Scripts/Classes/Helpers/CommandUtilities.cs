using Assets.Scripts.Classes.Commands;

namespace Assets.Scripts.Classes.Helpers
{
    public static class CommandUtilities
    {
        public static bool IsActiveState(this CommandState state) => !(state == CommandState.Cold || state == CommandState.Queued || state == CommandState.Ended);
    }
}
