using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Commandables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Controllers
{
    public delegate void ContextCommandDelegator(CallbackContext obj, List<ISelectable> selectedObjects);
    
    public interface IContextCommandProvider
    {
        public ContextCommandDelegator GetCommandDelegator();
    }
}
