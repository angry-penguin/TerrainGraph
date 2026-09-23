using System;
using NodeGraph;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGraph
{
    public abstract class TerrainNode : BaseNode
    {
        private static HashSet<string> activeBakeIDs = new HashSet<string>();
        
        [SerializeField] private string bakeId = String.Empty;
        [NonSerialized] private bool bakeIDInitialized = false;

        public string BakeID
        {
            get
            {
                if (!bakeIDInitialized)
                {
                    while (string.IsNullOrEmpty(bakeId) || !activeBakeIDs.Add(bakeId))
                        bakeId = GenerateBakeID();

                    bakeIDInitialized = true;
                }

                return bakeId;
            }
        }

        public string GenerateBakeID()
        {
            string bakeId = Guid.NewGuid().ToString();
            return bakeId.Substring(bakeId.LastIndexOf('-') + 1);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            activeBakeIDs.Remove(bakeId);
        }
        
        protected float GetConnectionMin(string fieldName)
        {
            var node = GetConnectedNode(fieldName) as TerrainInputNode;
            return node != null ? node.Min : 0;
        }

        protected float GetConnectionMax(string fieldName)
        {
            var node = GetConnectedNode(fieldName) as TerrainInputNode;
            return node != null ? node.Max : 1;
        }

        public abstract void Process(TerrainGraph graph, CommandBuffer command);

        public virtual void OnFinishProcess(TerrainGraph graph, CommandBuffer command) { }
    }
}