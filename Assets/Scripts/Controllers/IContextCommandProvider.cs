using Assets.Scripts.Classes.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Controllers
{
    public delegate void ContextCommandDelegator(CallbackContext obj, List<BasicUnitScript> selectedObjects);
    
    public interface IContextCommandProvider
    {
        public ContextCommandDelegator GetCommandDelegator();
    }
}
