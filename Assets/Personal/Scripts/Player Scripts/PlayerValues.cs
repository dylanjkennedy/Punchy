using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerValues : ActorValues
{
    public ChargeValues chargeValues;
    //public ImpactValues impactValues;
    public HealthValues healthValues;
    public MovementValues movementValues;
    public AirStateValues airStateValues;
    public ChargeAttackStateValues chargeAttackStateValues;
    public DashStateValues dashStateValues;
    public GroundPoundStateValues groundPoundStateValues;
    public GroundStateValues groundStateValues;
    public StaminaValues staminaValues;
    public GeneralValues generalValues;

    [System.Serializable]
    public class GeneralValues : System.Object
    {
        [SerializeField] private GameObject pauseMenu;

        public GameObject PauseMenu
        {
            get
            {
                return pauseMenu;
            }
        }
    }

    [System.Serializable]
    public class ChargeValues : System.Object
    {
        [SerializeField] private float chargeTimeout;
        [SerializeField] private float timeToCharge;
        [SerializeField] private float chargeCooldownTime;
        [SerializeField] private float slowmoTimescale;
        [SerializeField] private Image chargeWheel;
        [SerializeField] private Image slowmoWheel;
        [SerializeField] private float attackRange;

        public float ChargeTimeout
        {
            get
            {
                return chargeTimeout;
            }

            set
            {
                chargeTimeout = value;
            }
        }

        public float TimeToCharge
        {
            get
            {
                return timeToCharge;
            }

            set
            {
                timeToCharge = value;
            }
        }

        public float ChargeCooldownTime
        {
            get
            {
                return chargeCooldownTime;
            }

            set
            {
                chargeCooldownTime = value;
            }
        }

        public float SlowmoTimescale
        {
            get
            {
                return slowmoTimescale;
            }

            set
            {
                slowmoTimescale = value;
            }
        }

        public float AttackRange
        {
            get
            {
                return attackRange;
            }

            set
            {
                attackRange = value;
            }
        }

        public Image ChargeWheel
        {
            get
            {
                return chargeWheel;
            }
        }

        public Image SlowmoWheel
        {
            get
            {
                return slowmoWheel;
            }
        }
    }

    [System.Serializable]
    public class StaminaValues : System.Object
    {
        [SerializeField] private Image staminaBar;
        [SerializeField] private float maxStamina;
        [SerializeField] private float staminaRegen;
        [SerializeField] private float staminaRegenDelay;

        public Image StaminaBar
        {
            get
            {
                return staminaBar;
            }
        }

        public float MaxStamina
        {
            get
            {
                return maxStamina;
            }

            set
            {
                maxStamina = value;
            }
        }

        public float StaminaRegen
        {
            get
            {
                return staminaRegen;
            }

            set
            {
                staminaRegen = value;
            }
        }

        public float StaminaRegenDelay
        {
            get
            {
                return staminaRegenDelay;
            }

            set
            {
                staminaRegenDelay = value;
            }
        }
    }

    [System.Serializable]
    public class HealthValues : System.Object
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private Image damageImage;
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] private float flashSpeed;
        [SerializeField] private Color flashColor;
        [SerializeField] private int maxHealth;
        [SerializeField] private AudioClip hitSound;

        [SerializeField] private Image overshieldBar;

        public float FlashSpeed
        {
            get
            {
                return flashSpeed;
            }

            set
            {
                flashSpeed = value;
            }
        }

        public Image OvershieldBar
        {
            get
            {
                return overshieldBar;
            }
        }

        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }

            set
            {
                maxHealth = value;
            }
        }

        public Image HealthBar
        {
            get
            {
                return healthBar;
            }
        }

        public Image DamageImage
        {
            get
            {
                return damageImage;
            }
        }

        public Canvas GameOverCanvas
        {
            get
            {
                return gameOverCanvas;
            }
        }

        public Color FlashColor
        {
            get
            {
                return flashColor;
            }
        }

        public AudioClip HitSound
        {
            get
            {
                return hitSound;
            }
        }
    }

    [System.Serializable]
    public class MovementValues : System.Object
    {
        [SerializeField] private float speed;
        [SerializeField] private float stickToGroundForce;
        [SerializeField] private float jumpSpeed;

        public float Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public float JumpSpeed
        {
            get
            {
                return jumpSpeed;
            }

            set
            {
                jumpSpeed = value;
            }
        }

        public float StickToGroundForce
        {
            get
            {
                return stickToGroundForce;
            }
        }
    }

    [System.Serializable]
    public class AirStateValues : System.Object
    {
        [SerializeField] private float airGravityMultiplier;
        [SerializeField] private float airSpeedMultiplier;
        [SerializeField] private float groundPoundStaminaCost;

        public float GroundPoundStaminaCost
        {
            get
            {
                return groundPoundStaminaCost;
            }

            set
            {
                groundPoundStaminaCost = value;
            }
        }

        public float AirSpeedMultiplier
        {
            get
            {
                return airSpeedMultiplier;
            }
        }

        public float AirGravityMultiplier
        {
            get
            {
                return airGravityMultiplier;
            }
        }
    }

    [System.Serializable]
    public class ChargeAttackStateValues : System.Object
    {
        [SerializeField] private float endingDistance;
        [SerializeField] private float dashTime;
        [SerializeField] private float postHitTime;
        [SerializeField] private float slowestTimeScale;
        [SerializeField] private float slowmoLerpFactor;
        [SerializeField] private float staminaRegain;
        [SerializeField] private float explodeRadius;
        [SerializeField] private float explodePower;

        public float StaminaRegain
        {
            get
            {
                return staminaRegain;
            }

            set
            {
                staminaRegain = value;
            }
        }

        public float EndingDistance
        {
            get
            {
                return endingDistance;
            }
        }

        public float DashTime
        {
            get
            {
                return dashTime;
            }
        }

        public float PostHitTime
        {
            get
            {
                return postHitTime;
            }
        }

        public float SlowestTimeScale
        {
            get
            {
                return slowestTimeScale;
            }
        }

        public float SlowmoLerpFactor
        {
            get
            {
                return slowmoLerpFactor;
            }
        }

        public float ExplodeRadius
        {
            get
            {
                return explodeRadius;
            }
        }

        public float ExplodePower
        {
            get
            {
                return explodePower;
            }
        }
    }

    [System.Serializable]
    public class DashStateValues : System.Object
    {
        [SerializeField] private float dashTime;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashJumpImpulse;
        [SerializeField] private AudioClip dashSound;

        public float DashTime
        {
            get
            {
                return dashTime;
            }

            set
            {
                dashTime = value;
            }
        }

        public float DashSpeed
        {
            get
            {
                return dashSpeed;
            }

            set
            {
                dashSpeed = value;
            }
        }

        public float DashJumpImpulse
        {
            get
            {
                return dashJumpImpulse;
            }

            set
            {
                dashJumpImpulse = value;
            }
        }

        public AudioClip DashSound
        {
            get
            {
                return dashSound;
            }
        }
    }

    [System.Serializable]
    public class GroundPoundStateValues : System.Object
    {
        [SerializeField] private float groundPoundHopSpeed;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private float speedMaximum;
        [SerializeField] private float physicsMaxForce;
        [SerializeField] private ParticleSystem groundPoundParticles;
        [SerializeField] private AudioClip groundPoundSound;

        public ParticleSystem GroundPoundParticles
        {
            get
            {
                return groundPoundParticles;
            }
        }

        public AudioClip GroundPoundSound
        {
            get
            {
                return groundPoundSound;
            }
        }

        public float GroundPoundHopSpeed
        {
            get
            {
                return groundPoundHopSpeed;
            }

            set
            {
                groundPoundHopSpeed = value;
            }
        }

        public float GravityMultiplier
        {
            get
            {
                return gravityMultiplier;
            }

            set
            {
                gravityMultiplier = value;
            }
        }

        public float SpeedMaximum
        {
            get
            {
                return speedMaximum;
            }

            set
            {
                speedMaximum = value;
            }
        }

        public float PhysicsMaxForce
        {
            get
            {
                return physicsMaxForce;
            }
        }
    }

    [System.Serializable]
    public class GroundStateValues : System.Object
    {
        [SerializeField] private float dashCost;

        public float DashCost
        {
            get
            {
                return dashCost;
            }

            set
            {
                dashCost = value;
            }
        }
    }
}


