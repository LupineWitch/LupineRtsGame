using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Classes.UI.UIModels
{
    public interface IGuiNotifier
    {
        public event Action<object, EventArgs> UIPropertyChanged;
    }
}
