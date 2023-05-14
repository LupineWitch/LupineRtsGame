using Assets.Scripts.Commandables;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.Classes.Helpers;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using Mono.Cecil;
using Assets.Scripts.Objects.ResourceNodes;
using Assets.Scripts.Classes.Ai.BehaviourTree;
using System.Collections;

namespace Assets.Scripts.Controllers
{
    public class BasicAIController : CommandControllerBase
    {
        public const string UNITS_CONTAINER_NAME = "Units";
        public const string BUILDINGS_CONTAINER_NAME = "Buildings";
        public const string AMBIENT_CONTAINER_NAME = "Ambient";

        private GameObject entityContainer;
        private AiTreeBase decisionTree;
        public int NumberOfEntities<T>(string containerName,Predicate<T> predicate) where T : EntityBase
        {
            var container = entityContainer.transform.Find(containerName);
            if (container == null)
                return -1;

            int count = 0;
            foreach(Transform entityTransform in container.transform)
            {
                if (entityTransform.TryGetComponent(out T component))
                    if (predicate(component))
                        count++;
            }
            return count;   
        }
        
        public IEnumerable<T> GetEntities<T>(string containerName, Predicate<T> predicate) where T : EntityBase
        {
            var container = entityContainer.transform.Find(containerName);
            if (container == null)
                yield break;

                foreach (Transform entityTransform in container.transform)
                {
                    if (entityTransform.TryGetComponent(out T component))
                        if (predicate(component))
                            yield return component;
                   //Else
                    continue;
                }   
        }

        protected override void Awake()
        {
            base.Awake();
            var referenceManger = this.GetReferenceManagerInScene();
            entityContainer = referenceManger.AmbientContainer.transform.parent.gameObject;
        }       

        protected override void Update()
        {
        }

        protected virtual void Start()
        {
            decisionTree = new BasicAiTree(this);
            StartCoroutine(RunDecisionTree());
        }

        public IEnumerable<T> GetResourceNodes<T>(string containerName, Predicate<T> predicate) where T : ResourceNodeBase
        {
            var container = entityContainer.transform.Find(string.IsNullOrEmpty(containerName) ? "Ambient" : containerName);
            if (container == null)
                yield break;

            foreach (Transform entityTransform in container.transform)
            {
                if (entityTransform.TryGetComponent(out T component))
                    if (predicate(component))
                        yield return component;
                //Else
                continue;
            }
        }

        private IEnumerator RunDecisionTree()
        {
            decisionTree.InitializeTree();
            while (true)
            {
                decisionTree.RunTree();
                yield return null;
            }
        }
    }
}
