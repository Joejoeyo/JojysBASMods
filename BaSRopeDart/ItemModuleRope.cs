using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BaSRopeDart
{
    public class ItemModuleRope : ItemModule
    {
        public float maxDistance;
        public float minDistance = 0f;
        public float spring = 10000f;
        public float damper;
        public string materialAddress = "JojyGuyRopeMaterial";
        public float radius = 0.015f;
        public float tilingOffset = 10f;

        public string effectId = "RopeSqueak";
        public float audioMinForce = 400f;
        public float audioMaxForce = 1000f;
        public float audioMinSpeed = 0.25f;
        public float audioMaxSpeed = 2f;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<RopeHandler>();
        }
    }
}
