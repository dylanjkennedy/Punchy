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
	private PlayerMover playerMover;
	Camera camera;

	// Use this for initialization
	void Start () {
		currentCharge = 0;
		//charging = false;
		chargedTime = 0;
		chargeWheel.type = Image.Type.Filled;
		chargeWheel.fillMethod = Image.FillMethod.Radial360;
		chargeWheel.fillAmount = 0f;
		enemyMask = LayerMask.GetMask("Enemy");
		playerMover = gameObject.GetComponent<PlayerMover> ();
		camera = Camera.main;
	}

	private void UpdateChargeWheel()
	{
		if (chargeCooling)
		{
			chargeWheel.fillAmount = 1 - (chargedTime / chargeCooldownTime);
		} 
		else
		{
			chargeWheel.fillAmount = currentCharge / timeToCharge;
		}
	}


	//returns true if we should attack
	public bool Charge(bool charging)
	{
		if (!charging && !charged && !chargeCooling && currentCharge > 0) {
			currentCharge -= Time.fixedDeltaTime * 2;
			UpdateChargeWheel ();
			return false;
		}

		if (chargeCooling) {
			chargedTime += Time.deltaTime;
			if (chargedTime >= chargeCooldownTime) {
				chargeCooling = false;
				chargedTime = 0;
				chargeWheel.color = Color.white;
			}
			UpdateChargeWheel ();
			return false;
		}

		if (charging && !charged && !chargeCooling) {
			currentCharge += Time.fixedDeltaTime;
			if (currentCharge >= timeToCharge)
			{
				charged = true;
				changeTimeScale (slowmoTimescale);
				currentCharge = 0;
			} 
			else
			{
				UpdateChargeWheel ();
				return false;
			}
		}
			
		if (charged) {
			chargedTime += Time.fixedUnscaledDeltaTime;
			chargeWheel.fillAmount = 1;

			//if we're charge and fire button released, check if we can successfully attack
			if (!charging)
			{
				bool hit = false;
				if (tryAttack())
				{
					chargeWheel.color = Color.white;
					hit = true;
				}
				else
				{
					chargeCooling = true;
					chargeWheel.color = Color.red;
				}
				changeTimeScale(fullspeedTimescale);
				chargedTime = 0;
				charged = false;
				UpdateChargeWheel ();
				return hit;
			}

			//if we're charged too long, initiate cooldown
			if (chargedTime >= chargeTimeout)
			{
				changeTimeScale (fullspeedTimescale);
				chargedTime = 0;
				charged = false;
				chargeCooling = true;
				chargeWheel.color = Color.red;
				UpdateChargeWheel ();
				return false;
			}

			//while charged, reticle is green while enemy in range and in sights
			if (checkAttack ().collider != null)
			{
				chargeWheel.color = Color.green;
				return false;
			} 
			else
			{
				chargeWheel.color = Color.white;
			}
		}
		return false;
	}


	private RaycastHit checkAttack()
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
		return attackHit;
	}

	//should be deprecated soon
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


	private void changeTimeScale(float newTime){
		Time.timeScale = newTime;
		Time.fixedDeltaTime = 0.01666667f * Time.timeScale;
	}
}
