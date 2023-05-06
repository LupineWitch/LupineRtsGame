using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Classes.Helpers
{
    public static class MonoBehaviourExtension
    {
        public static ReferenceManager GetReferenceManagerInScene(this MonoBehaviour component)
        {
            foreach (var rootGameObj in SceneManager.GetActiveScene().GetRootGameObjects())
                if (rootGameObj.TryGetComponent(out ReferenceManager manager))
                    return manager;

            return null;
        }
    }
}
