using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorValues : MonoBehaviour
{
    public ImpactValues impactValues;

    [System.Serializable]
    public class ImpactValues : System.Object
    {
        [SerializeField] private float mass;

        public float Mass
        {
            get
            {
                return mass;
            }
        }
    }
}
