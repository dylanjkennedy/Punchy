using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
		[SerializeField] private Image chargeWheel;
		[SerializeField] private float attackRange;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

		private bool charging;
		private bool chargeCooling;
		private float currentCharge;
		private bool charged;
		private float chargedTime;
		[SerializeField] private float slowmoTimescale;
		private float fullspeedTimescale = 1f;
		[SerializeField] private float chargeTimeout;
		[SerializeField] private float timeToCharge;
		[SerializeField] private float chargeCooldownTime;
        private LayerMask enemyMask;


        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
			currentCharge = 0;
			charging = false;
			chargedTime = 0;
			chargeWheel.type = Image.Type.Filled;
			chargeWheel.fillMethod = Image.FillMethod.Radial360;
			chargeWheel.fillAmount = 0f;
            enemyMask = LayerMask.GetMask("Enemy");
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }
				
			charging = Input.GetButton ("Fire1");



		

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                m_MoveDir.y = 0f;
                m_Jumping = false;
                PlayLandSound();
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

			if (!charging && !charged && !chargeCooling && currentCharge > 0) {
				currentCharge -= Time.fixedDeltaTime * 2;
			}

			if (chargeCooling) {
				chargedTime += Time.deltaTime;
				if (chargedTime >= chargeCooldownTime) {
					chargeCooling = false;
					chargedTime = 0;
					chargeWheel.color = Color.white;
				}
			}

			if (charging && !charged && !chargeCooling) {
				currentCharge += Time.fixedDeltaTime;
				if (currentCharge >= timeToCharge) {
					charged = true;
					changeTimeScale (slowmoTimescale);
					currentCharge = 0;
				}
			}


			if (charged) {
				chargedTime += Time.fixedUnscaledDeltaTime;

                

                if (!charging)
                {
                    if (tryAttack())
                    {
                        chargeWheel.color = Color.white;
                    }
                    else
                    {
                        chargeCooling = true;
                        chargeWheel.color = Color.red;
                    }
                    changeTimeScale(fullspeedTimescale);
                    chargedTime = 0;
                    charged = false;
                }
                else
                {
                    if (checkAttack().collider != null)
                    {
                        chargeWheel.color = Color.green;
                    }
                    else
                    {
                        chargeWheel.color = Color.white;
                    }

                    if (chargedTime >= chargeTimeout)
                    {
                        changeTimeScale(fullspeedTimescale);
                        chargedTime = 0;
                        charged = false;
                        chargeCooling = true;
                        chargeWheel.color = Color.red;
                    }
                }

                
            }

			if (charged) {
				chargeWheel.fillAmount = 1;
			}
			else if (!chargeCooling) {
				chargeWheel.fillAmount = currentCharge / timeToCharge;
			}
			else {
				chargeWheel.fillAmount = 1-(chargedTime / chargeCooldownTime);
			}

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;



            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);


            m_MouseLook.UpdateCursorLock();
        }

		private void changeTimeScale(float newTime){
			Time.timeScale = newTime;
			Time.fixedDeltaTime = 0.01666667f * Time.timeScale;
		}

		private void fillWheel(){
			chargeWheel.fillAmount = currentCharge/timeToCharge;
		}

        private RaycastHit checkAttack()
        {
            Ray attackRay;
            RaycastHit attackHit;
            attackRay = m_Camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

            if (Physics.Raycast(attackRay, out attackHit, attackRange, enemyMask))
            {
                if (attackHit.collider.gameObject.tag == "Enemy")
                {
                    return attackHit;
                }
            }
            
            return attackHit;
        }

		private bool tryAttack(){
            RaycastHit hit = checkAttack();
            if (hit.collider == null)
            {
                return false;
            }
            GameObject enemy = hit.collider.gameObject;
            enemy.GetComponent<EnemyController>().takeDamage(hit.point);

            // puts the player one unit away from the enemy along the vector between them
            transform.position = enemy.transform.position - Vector3.Normalize(enemy.transform.position - transform.position);
            return true;
		}


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void PlayLandSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

		/*
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
		*/
    }
}
