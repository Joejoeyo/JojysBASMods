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
     * Working off of RopeSimple, but not gonna use it directly since I don't think RopeSimple actually works how I intended
     * </remarks>
     */
    class RopeSmart : MonoBehaviour
    {
        public GameObject objectA;
        public GameObject objectB;

        public Rigidbody rigidA;
        public Rigidbody rigidB;

        public Transform ropePointA;
        public Transform ropePointB;

        public SpringJoint springJoint;

        //just for visuals
        public RopeSimple ropeSimple;


        //parameters
        public float spring = 0;
        public float damper = 0;
        public Material material;
        public float maxDistance = 10f;
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
            /*
            ropeSimple = objectA.AddComponent<RopeSimple>();
            ropeSimple.targetAnchor = ropePointB;
            ropeSimple.connectedBody = objectB.GetComponent<Rigidbody>();
            ropeSimple.maxDistance = Mathf.Infinity;
            ropeSimple.minDistance = 0;
            ropeSimple.spring = 0;
            ropeSimple.damper = 0;*/

        }

    }
}
