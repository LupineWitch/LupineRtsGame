using System;

namespace Assets.Scripts.Classes.UI.UIModels
{
    public interface IGuiNotifier
    {
        public event Action<object, EventArgs> UIPropertyChanged;
    }
}
