using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Classes.Static
{
    public static class ManagedStructsExtensions
    {
        public static int GetUnevenInteger(this int number) => number % 2 == 0 ? number + 1 : number;

        public static int GetEvenInteger(this int number) => number % 2 == 0 ? number : number + 1;
    }
}
