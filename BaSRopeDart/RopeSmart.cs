using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;


namespace BaSRopeDart
{

    /**
     * <summary>
     * Connects a Rope Between 2 objects with rigidbodies.
     * </summary>
     * <remarks>
     * Basically a copy of RopeSimple, but with some changes that make it easier to set up and clean up.
     * </remarks>
     */
    class RopeSmart : MonoBehaviour
    {
        //technical stuff
        public RopeHandler ropeHandler;

        public GameObject objectA;
        public GameObject objectB;

        public Rigidbody rigidA;
        public Rigidbody rigidB;

        public Transform ropePointA;
        public Transform ropePointB;

        public SpringJoint springJoint;

        public float distance;

        public Handle lengthHandle;

        //visual representation of this rope
        public Material material;

        public float radius;
        public float tilingOffset;
        public MeshRenderer mesh;
        public Transform meshTransform;

        public LightVolumeReceiver lightVolumeReceiver;

        public static readonly int BaseMapSt = Shader.PropertyToID("_BaseMap_ST");

        public Vector4 ropeScaling = Vector4.zero;


        //audio effects
        public EffectInstance effectInstance;
        public List<ValueDropdownItem<string>> GetAllEffectID()
        {
            return Catalog.GetDropdownAllID(Category.Effect);
        }

        public string effectId;
        public float audioMinForce;
        public float audioMaxForce;
        public float audioMinSpeed;
        public float audioMaxSpeed;


        //parameters
        public float spring;
        public float damper;
        public float maxDistance;
        public float minDistance;
        public bool enableCollision;
        public bool enableHandle;



        //basically copied from ropesimple (but with a few changes to the spring joint creation), until stated otherwise
        public void Awake()
        {
            if (objectA == null)
            {
                Debug.LogError("RopeSmart was awakened but is missing object A!");
                enabled = false;
                return;
            }
            if (objectB == null)
            {
                Debug.LogError("RopeSmart was awakened but is missing object B!");
                enabled = false;
                return;
            }

            rigidA = objectA.GetComponentInParent<Rigidbody>();
            rigidB = objectB.GetComponentInParent<Rigidbody>();

            if (rigidA == null)
            {
                Debug.LogError("RopeSmart was awakened but is missing rigidbody A!");
                enabled = false;
                return;
            }
            if (rigidB == null)
            {
                Debug.LogError("RopeSmart was awakened but is missing rigidbody B!");
                enabled = false;
                return;
            }

            springJoint = objectA.AddComponent<SpringJoint>();
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = maxDistance;
            springJoint.minDistance = 0.01f;
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.enableCollision = enableCollision;
            springJoint.tolerance = 0;

            springJoint.connectedBody = rigidB;

            if (ropePointA == null)
                ropePointA = objectA.transform;
            if (ropePointB == null)
                ropePointB = objectB.transform;

            //not sure the best way to add these just yet. I don't think they're necessary, but they should make the physics more accurate to the rope's location
            //springJoint.anchor = ropePointA.position;
            //springJoint.connectedAnchor = ropePointB.position;

            //visual/audio setup
            mesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder).GetComponent<MeshRenderer>();
            DestroyImmediate(mesh.GetComponent<Collider>());
            meshTransform = mesh.transform;
            meshTransform.SetParent(ropePointB);
            mesh.name = "SmartRopeMesh";
            mesh.material = material;

            lightVolumeReceiver = mesh.gameObject.AddComponent<LightVolumeReceiver>();
            lightVolumeReceiver.volumeDetection = LightVolumeReceiver.VolumeDetection.StaticPerMesh;
            EffectData data = Catalog.GetData<EffectData>(effectId);
            if (data != null)
            {
                effectInstance = data.Spawn(transform.position, transform.rotation, transform, null, true, null, false, 1f, 1f);
                effectInstance.SetIntensity(0f);
            }

            //NEW STUFF

            //not sure if mesh is the best object to add the handle to, but i figure it's already in the right position and angle..?
            if (enableHandle)
            {
                mesh.gameObject.SetActive(false);
                lengthHandle = mesh.gameObject.AddComponent<Handle>();

                lengthHandle.interactableId = "ClimbRopeVertical";
                lengthHandle.axisLength = 0.1f; //this number doesn't seem to change anything
                lengthHandle.orientationDefaultLeft = lengthHandle.AddOrientation(Side.Left, Vector3.zero, new Quaternion());
                lengthHandle.orientationDefaultRight = lengthHandle.AddOrientation(Side.Right, Vector3.zero, new Quaternion());
                lengthHandle.artificialDistance = -0.5f;


                mesh.gameObject.SetActive(true);
            }
        }

        //don't know when this gets called, just copying it from rope simple
        private void Start()
        {
            ropeScaling.x = 1f;
            ropeScaling.y = 1f;
            ropeScaling.z = 0f;
            ropeScaling.w = 0f;
        }


        //also just copying this from rope simple
        protected void OnDisable()
        {
            if (effectInstance != null)
            {
                effectInstance.Stop();
            }
        }

        /**
         * <summary>
         * Destroys the joint, mesh, and effects, without leaving anything behind.
         * </summary>
         * 
         */
        public void Destroy()
        {
            Destroy(springJoint);
            effectInstance?.Stop();
            effectInstance?.Despawn();
            lengthHandle.enabled = false;
            Destroy(lengthHandle);
            mesh.enabled = false;
            Destroy(mesh);
            Destroy(this);
        }

        //also just copying from rope simple.. until stated otherwise >:)
        protected void LateUpdate()
        {
            if (rigidA.IsSleeping())
            {
                if (effectInstance != null && effectInstance.isPlaying)
                    effectInstance.Stop();

                return;
            }

            Vector3 meshFromPos = ropePointA.position;
            Vector3 meshToPos = ropePointB.position;

            meshTransform.position = Vector3.Lerp(meshFromPos, meshToPos, 0.5f);
            meshTransform.rotation = Quaternion.FromToRotation(mesh.transform.TransformDirection(Vector3.up), meshToPos - meshFromPos) * mesh.transform.rotation;

            distance = Vector3.Distance(meshFromPos, meshToPos);
            mesh.transform.localScale = new Vector3(radius, distance * 0.5f, radius);

            foreach (MaterialInstance materialInstance in lightVolumeReceiver.materialInstances)
            {
                if (materialInstance.CachedRenderer && materialInstance.CachedRenderer.isVisible)
                {
                    ropeScaling.y = distance * tilingOffset;
                    materialInstance.material.SetVector(BaseMapSt, ropeScaling);
                }
            }

            if (effectInstance != null)
            {
                float intensity = Mathf.InverseLerp(audioMinForce, audioMaxForce, springJoint.currentForce.magnitude) *
                    Mathf.InverseLerp(audioMinSpeed, audioMaxSpeed, rigidA.velocity.magnitude);
                effectInstance.SetIntensity(intensity);
                if (!effectInstance.isPlaying)
                {
                    effectInstance.Play();
                }
            }

            //NEW STUFF
            if (lengthHandle != null)
            {
                lengthHandle.transform.position = meshTransform.position;
                lengthHandle.transform.rotation = meshTransform.rotation;

                float safeDistance = Mathf.Max(0.01f, distance);

                lengthHandle.axisLength = safeDistance;

                //RecalculateHandleCollider();
                if (lengthHandle.touchCollider is CapsuleCollider capCol)
                {
                    capCol.height = lengthHandle.axisLength + lengthHandle.touchRadius;
                }
            }
            
        }

        public void RecalculateHandleCollider()
        {
            if (lengthHandle.touchCollider == null)
            {
                return;
            }
            Collider oldCollider = lengthHandle.touchCollider;
            CapsuleCollider capCollider;
            lengthHandle.touchCollider = lengthHandle.gameObject.AddComponent<CapsuleCollider>();
            capCollider = (CapsuleCollider)lengthHandle.touchCollider;
            capCollider.isTrigger = true;
            capCollider.radius = lengthHandle.touchRadius;
            capCollider.center = lengthHandle.touchCenter;
            capCollider.height = lengthHandle.axisLength + lengthHandle.touchRadius;
            capCollider.direction = 1;


            DestroyImmediate(oldCollider);
        }


        public void SegmentGrabbed()
        {

        }
    }
}
