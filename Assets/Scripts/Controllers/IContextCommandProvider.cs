using Assets.Scripts.Commandables;
using System.Collections.Generic;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Controllers
{
    public delegate void ContextCommandDelegator(CallbackContext obj, BasicCommandControler commander, List<ISelectable> selectedObjects);

    public interface IContextCommandProvider
    {
        public ContextCommandDelegator GetCommandDelegator();
    }
}
