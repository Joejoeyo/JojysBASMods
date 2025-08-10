using IngameDebugConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BaSRopeDart
{
    public class RopeHandler : MonoBehaviour
    {
        //i think the spool should be index 0, and the blade should be the last index, and 'RopeGrips' all inbetween based on their actual order between the ends
        public List<Item> attachedItems;
        public Item ropeSpool;
        public Item ropeDart;
        public float totalLength = 10f;
        public float slack;

        protected ItemModuleRope module;

        public Material material = null;

        protected void Awake()
        {
            StartCoroutine(WaitToInitialize());
            
        }

        private void Holder_Snapped(Item item)
        {
            if (item == ropeSpool)
            {
                foreach (Item attachedItem in attachedItems)
                {
                    RemoveSmartRope(item);
                }
            }
            else
            {
                Debug.Log("Non-matching item was snapped into rope dart, unsnapping");
                item.holder.UnSnap(item, true);
            }

        }

        private void Holder_UnSnapped(Item item)
        {
            CreateSmartRopeBetween(ropeSpool.gameObject, ropeDart.gameObject);
        }

        IEnumerator WaitToInitialize()
        {
            yield return new WaitForSeconds(0.5f);


            attachedItems = new List<Item>();

            Item item = GetComponent<Item>();

            module = item.data.GetModule<ItemModuleRope>();
            Holder holder = item.childHolders?[0];

            if (holder != null)
            {
                Item heldItem;
                if ((heldItem = holder.items?[0]) != null)
                {
                    holder.UnSnapped += Holder_UnSnapped;
                    holder.Snapped += Holder_Snapped;
                    ropeDart = item;
                    ropeSpool = heldItem;
                    attachedItems.Add(ropeSpool);
                    attachedItems.Add(ropeDart);
                }
            }
            else
            {
                Debug.Log("RopeHandler object tried to create but didn't have holders containing an item!");
            }

            if (!string.IsNullOrEmpty(module.materialAddress))
                Catalog.LoadAssetAsync<Material>(module.materialAddress, mat => { material = mat; }, "RopeDart");

        }


        private void RemoveRope(GameObject componentOwner)
        {
            RopeSimple ropeComponent;

            if (componentOwner.TryGetComponent<RopeSimple>(out ropeComponent))
            {
                GameObject.Destroy(ropeComponent);
                Debug.Log("disconnected rope");
                return;
            }
            Debug.Log("No rope to disconnect");
        }

        private void RemoveSmartRope(Item item)
        {
            GameObject componentOwner = item.gameObject;

            if (componentOwner.TryGetComponent<RopeSmart>(out RopeSmart ropeComponent))
            {
                ropeComponent.Destroy();
                Debug.Log("disconnected rope");
                return;
            }
            Debug.Log("No rope to disconnect");
        }

        private void RemoveRopeBetween(GameObject objectA, GameObject objectB)
        {
            RopeSimple ropeComponent;
            if (objectA.TryGetComponent<RopeSimple>(out ropeComponent) && ropeComponent?.connectedBody?.gameObject == objectB)
            {
                GameObject.Destroy(ropeComponent);
                Debug.Log("disconnected rope on A");
            }
            if (objectB.TryGetComponent<RopeSimple>(out ropeComponent) && ropeComponent?.connectedBody?.gameObject == objectA)
            {
                GameObject.Destroy(ropeComponent);
                Debug.Log("disconnected rope on B");
            }
        }

        private void RemoveSmartRopeBetween(Item itemA, Item itemB)
        {
            GameObject objectA = itemA.gameObject;
            GameObject objectB = itemB.gameObject;
            RopeSmart ropeComponent;
            if (objectA.TryGetComponent<RopeSmart>(out ropeComponent) && ropeComponent?.objectB == objectB)
            {
                ropeComponent.Destroy();
                Debug.Log("disconnected rope on A");
            }
            if (objectB.TryGetComponent<RopeSmart>(out ropeComponent) && ropeComponent?.objectB == objectA)
            {
                ropeComponent.Destroy();
                Debug.Log("disconnected rope on B");
            }
        }

        /*private void CreateRopeBetween(GameObject objectA, GameObject objectB)
        {
            Item itemA;
            Item itemB;
            if (objectA.TryGetComponent<Item>(out itemA) && objectB.TryGetComponent<Item>(out itemB))
            {
                Transform ropePointA;
                if (!itemA.TryGetCustomReference("RopePoint", out ropePointA))
                    ropePointA = itemA.transform;
                Transform ropePointB;
                if (!itemB.TryGetCustomReference("RopePoint", out ropePointB))
                    ropePointB = itemB.transform;

                RemoveRopeBetween(objectA, objectB);

                //add rope from A to B
                {
                    objectA.SetActive(false);
                    RopeSimple ropeComponent = ropePointA.gameObject.AddComponent<RopeSimple>();

                    AssignValuesFromModule(ropeComponent);
                    ropeComponent.targetAnchor = ropePointB;
                    ropeComponent.connectedBody = objectB.GetComponent<Rigidbody>();
                    objectA.SetActive(true);
                }
                
                /*
                //add rope from B to A
                {
                    objectB.SetActive(false);
                    RopeSimple ropeComponent = ropePointB.gameObject.AddComponent<RopeSimple>();

                    AssignValuesFromModule(ropeComponent);
                    ropeComponent.targetAnchor = ropePointA;
                    ropeComponent.connectedBody = objectA.GetComponent<Rigidbody>();
                    objectB.SetActive(true);
                }

                Debug.Log("made rope maybe");
            }*/

            /*
            Rigidbody objectbRigidbody;
            if (objectB.TryGetComponent<Rigidbody>(out objectbRigidbody))
            {

                
                if (objectA.TryGetComponent<Item>(out itemA))
                {
                    Transform ropePoint = itemA.transform;
                    if (itemA.GetCustomReference("RopePoint", true) != null)
                        ropePoint = itemA.GetCustomReference("RopePoint", false);

                    RemoveRope(objectA);

                    objectA.SetActive(false);

                    objectA.AddComponent<RopeSimple>();

                    RopeSimple ropeComponent = objectA.GetComponent<RopeSimple>();

                    AssignValuesFromModule(ropeComponent);
                    ropeComponent.targetAnchor = ropePoint;
                    //ropeComponent.rigidA = objectA.GetComponent<Rigidbody>();
                    //ropeComponent.rigidB = objectB.GetComponent<Rigidbody>();
                    ropeComponent.connectedBody = objectbRigidbody;

                    objectA.SetActive(true);

                    Debug.Log($"created rope");
                    return;
                }
            }

            //Debug.Log("failed to create rope for whatever reason");

        }*/

        private void CreateSmartRopeBetween(GameObject objectA, GameObject objectB)
        {
            Item itemA;
            Item itemB;
            if (objectA.TryGetComponent<Item>(out itemA) && objectB.TryGetComponent<Item>(out itemB))
            {
                Transform ropePointA = GetRopePoint(itemA);
                Transform ropePointB = GetRopePoint(itemB);
                if (!itemB.TryGetCustomReference("RopePoint", out ropePointB))
                    ropePointB = itemB.transform;

                //RemoveRopeBetween(objectA, objectB);
                RemoveSmartRope(itemA);

                //add rope from A to B

                objectA.SetActive(false);
                RopeSmart ropeComponent = objectA.AddComponent<RopeSmart>();

                AssignValuesFromModule(ropeComponent);
                ropeComponent.objectA = objectA;
                ropeComponent.objectB = objectB;
                ropeComponent.ropePointA = ropePointA;
                ropeComponent.ropePointB = ropePointB;



                objectA.SetActive(true);

                /*
                //rope simple for visuals only, and only make 1
                ropePointA.gameObject.SetActive(false);
                RopeSimple ropeSimple = ropePointA.gameObject.AddComponent<RopeSimple>();
                ropeSimple.targetAnchor = ropePointB;
                ropeSimple.connectedBody = objectB.GetComponent<Rigidbody>();
                ropeSimple.maxDistance = Mathf.Infinity;
                AssignRopeSimpleValuesFromModule(ropeSimple);

                ropeComponent.ropeSimple = ropeSimple;
                */

                ropePointA.gameObject.SetActive(true);
                

                Debug.Log("made rope maybe");
            }

            /*
            Rigidbody objectbRigidbody;
            if (objectB.TryGetComponent<Rigidbody>(out objectbRigidbody))
            {

                
                if (objectA.TryGetComponent<Item>(out itemA))
                {
                    Transform ropePoint = itemA.transform;
                    if (itemA.GetCustomReference("RopePoint", true) != null)
                        ropePoint = itemA.GetCustomReference("RopePoint", false);

                    RemoveRope(objectA);

                    objectA.SetActive(false);

                    objectA.AddComponent<RopeSimple>();

                    RopeSimple ropeComponent = objectA.GetComponent<RopeSimple>();

                    AssignValuesFromModule(ropeComponent);
                    ropeComponent.targetAnchor = ropePoint;
                    //ropeComponent.rigidA = objectA.GetComponent<Rigidbody>();
                    //ropeComponent.rigidB = objectB.GetComponent<Rigidbody>();
                    ropeComponent.connectedBody = objectbRigidbody;

                    objectA.SetActive(true);

                    Debug.Log($"created rope");
                    return;
                }
            }*/

            //Debug.Log("failed to create rope for whatever reason");

        }

        public static Transform GetRopePoint(Item item)
        {
            Transform ropePoint;
            if (!item.TryGetCustomReference("RopePoint", out ropePoint))
                ropePoint = item.transform;
            return ropePoint;
        }

        private void AssignValuesFromModule(RopeSmart ropeComponent)
        {
            ropeComponent.maxDistance = module.maxDistance;
            ropeComponent.minDistance = module.minDistance;
            ropeComponent.spring = module.spring;
            ropeComponent.damper = module.damper;

            ropeComponent.material = material;
            ropeComponent.radius = module.radius;
            ropeComponent.tilingOffset = module.tilingOffset;

            ropeComponent.effectId = module.effectId;
            ropeComponent.audioMinForce = module.audioMinForce;
            ropeComponent.audioMaxForce = module.audioMaxForce;
            ropeComponent.audioMinSpeed = module.audioMinSpeed;
            ropeComponent.audioMaxSpeed = module.audioMaxSpeed;

        }


        private void AssignRopeSimpleValuesFromModule(RopeSimple ropeComponent)
        {

            ropeComponent.minDistance = 0;
            ropeComponent.spring = 0;
            ropeComponent.damper = 0;

            //Debug.Log($"rope is given material: {material}");
            ropeComponent.material = material;
            ropeComponent.radius = module.radius;
            ropeComponent.tilingOffset = module.tilingOffset;

            ropeComponent.effectId = module.effectId;
            ropeComponent.audioMinForce = module.audioMinForce;
            ropeComponent.audioMaxForce = module.audioMaxForce;
            ropeComponent.audioMinSpeed = module.audioMinSpeed;
            ropeComponent.audioMaxSpeed = module.audioMaxSpeed;
        }
    }

    
}
