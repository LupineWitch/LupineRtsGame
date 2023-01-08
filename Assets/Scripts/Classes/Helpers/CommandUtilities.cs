using Assets.Scripts.Classes.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Helpers
{
    public static class CommandUtilities
    {
        public static bool IsActiveState(this CommandState state) => !(state == CommandState.Cold || state == CommandState.Queued || state == CommandState.Ended);
    }
}
