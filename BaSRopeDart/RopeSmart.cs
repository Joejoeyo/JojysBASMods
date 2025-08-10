using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public GameObject objectA;
        public GameObject objectB;

        public Rigidbody rigidA;
        public Rigidbody rigidB;

        public Transform ropePointA;
        public Transform ropePointB;

        public SpringJoint springJoint;

        public float distance;

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
        public float audioMinForce = 400f;
        public float audioMaxForce = 1000f;
        public float audioMinSpeed = 0.25f;
        public float audioMaxSpeed = 2f;


        //parameters
        public float spring = 0;
        public float damper = 0;
        public float maxDistance = 10f;
        public float minDistance = 0.01f;
        public bool enableCollision = false;




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
         * Destroys the joint, mesh, and effects, hopefully without leaving anything behind.
         * </summary>
         * 
         */
        public void Destroy()
        {
            Destroy(springJoint);
            effectInstance?.Stop();
            effectInstance?.Despawn();
            mesh.enabled = false;
            Destroy(mesh);
            Destroy(this);
        }

        //also just copying from rope simple..
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
        }

    }
}
