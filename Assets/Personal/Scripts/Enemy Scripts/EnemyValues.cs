using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyValues : ActorValues
{
    public GeneralValues generalValues;

    [System.Serializable]
    public class GeneralValues : System.Object
    {
        [SerializeField] private float healthValue;
        [SerializeField] private float impactToKill;
        [SerializeField] private float knockbackModifier;

        public float HealthValue { get { return healthValue; } }
        public float ImpactToKill { get { return impactToKill; } }
        public float KnockbackModifier {  get { return knockbackModifier; } }
    }
}
