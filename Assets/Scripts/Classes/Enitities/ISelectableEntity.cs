using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.Enitities
{
    internal interface ISelectableEntity
    {
        public bool IsSelectedBy(BasicCommandControler possibleOwner);
        public bool CanBeSelectedBy(BasicCommandControler selector);
        public bool TrySelect(BasicCommandControler selector);
        public bool TryUnselect(BasicCommandControler selector);
        public virtual bool TrySetSelection(BasicCommandControler selector, bool select) => select ? TrySelect(selector) : TryUnselect(selector);
    }
}
