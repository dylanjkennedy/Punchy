using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour {
	private float currentCharge;
	private float chargedTime;
	//private float currentCooldown;
	//private bool charging;
	private bool chargeCooling;
	private bool charged;
	[SerializeField] private float chargeTimeout;
	[SerializeField] private float timeToCharge;
	[SerializeField] private float chargeCooldownTime;
	[SerializeField] private float slowmoTimescale;
	[SerializeField] private Image chargeWheel;
	[SerializeField] private float attackRange;
	private float fullspeedTimescale = 1f;
	private LayerMask enemyMask;
    private LayerMask emptyMask;
	private PlayerMover playerMover;
    TimeScaleManager timeScaleManager;
	Camera camera;

	// Use this for initialization
	void Start () {
		currentCharge = 0;
		//charging = false;
		chargedTime = 0;
		chargeWheel.type = Image.Type.Filled;
		chargeWheel.fillMethod = Image.FillMethod.Radial360;
		chargeWheel.fillAmount = 1f;
		enemyMask = LayerMask.GetMask("Enemy");
        emptyMask = LayerMask.GetMask();
		playerMover = gameObject.GetComponent<PlayerMover> ();
		camera = Camera.main;
        timeScaleManager = camera.GetComponent<TimeScaleManager>();
	}

	private void UpdateChargeWheel()
	{
        /*
		if (chargeCooling)
		{
			chargeWheel.fillAmount = 1 - (chargedTime / chargeCooldownTime);
		} 
		else
		{
			chargeWheel.fillAmount = currentCharge / timeToCharge;
		}
        */
	}


	//returns true if we should attack
	public RaycastHit Charge(bool charging)
	{
        RaycastHit hit = getNullHit();
		if (!charging && !charged && !chargeCooling && currentCharge > 0) {
			currentCharge -= Time.fixedDeltaTime * 2;
			UpdateChargeWheel ();
			return hit;
		}

		if (chargeCooling) {
			chargedTime += Time.deltaTime;
			if (chargedTime >= chargeCooldownTime) {
				chargeCooling = false;
				chargedTime = 0;
				chargeWheel.color = Color.white;
			}
			UpdateChargeWheel ();
			return hit;
		}

		if (charging && !charged && !chargeCooling) {
			currentCharge += Time.fixedDeltaTime;
			if (currentCharge >= timeToCharge)
			{
				charged = true;
                timeScaleManager.changeTimeScale(slowmoTimescale, chargeTimeout, 0f);
				currentCharge = 0;
			} 
			else
			{
				UpdateChargeWheel ();
				return hit;
			}
		}
			
		if (charged) {
			chargedTime += Time.fixedUnscaledDeltaTime;
			chargeWheel.fillAmount = 1;

			//if we're charge and fire button released, check if we can successfully attack
			if (!charging)
			{
                hit = checkAttack();
				if (hit.collider != null)
				{
					chargeWheel.color = Color.white;
				}
				else
				{
					chargeCooling = true;
					chargeWheel.color = Color.red;
                    hit = getNullHit();
				}
				timeScaleManager.fullspeedTimeScale();
				chargedTime = 0;
				charged = false;
				UpdateChargeWheel ();
				return hit;
			}

			//if we're charged too long, initiate cooldown
			if (chargedTime >= chargeTimeout)
			{
				chargedTime = 0;
				charged = false;
				chargeCooling = true;
				chargeWheel.color = Color.red;
				UpdateChargeWheel ();
				return hit;
			}

			//while charged, reticle is green while enemy in range and in sights
			if (checkAttack().collider != null)
			{
				chargeWheel.color = Color.green;
				return hit;
			} 
			else
			{
				chargeWheel.color = Color.white;
			}
		}
		return hit;
	}

    private RaycastHit getNullHit()
    {
        Ray attackRay = new Ray();
        RaycastHit attackHit;

        Physics.Raycast(attackRay, out attackHit, attackRange, emptyMask);
        return attackHit;
    }


	public RaycastHit checkAttack()
	{
		Ray attackRay;
		RaycastHit attackHit;
		attackRay = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		if (Physics.Raycast(attackRay, out attackHit, attackRange, enemyMask))
		{
			if (attackHit.collider.gameObject.tag == "Enemy")
			{
				return attackHit;
			}
		}
		return getNullHit();
	}
}
