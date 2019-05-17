using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorValues : MonoBehaviour
{
    public ImpactValues impactValues;

    [System.Serializable]
    public class ImpactValues : System.Object
    {
        [SerializeField] float mass;
        [SerializeField] float groundFriction;
        [SerializeField] float airFriction;
        [SerializeField] float recoveryImpact;

        public float Mass { get { return mass; } }
        public float GroundFriction { get { return groundFriction; } }
        public float AirFriction { get { return airFriction; } }
        public float RecoveryImpact { get { return recoveryImpact; } }

    }
}
