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
    [SerializeField] private Image slowmoWheel;
	[SerializeField] private float attackRange;
	private float fullspeedTimescale = 1f;
	private LayerMask enemyMask;
    private LayerMask emptyMask;
    private LayerMask enemyAndDefaultMask;

    private PlayerMover playerMover;
    TimeScaleManager timeScaleManager;
	Camera camera;

    //TEMPORARY
    Ray ray;
    Ray debugLine;
    Ray playerRay;
    Vector3 point;
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(ray.origin, ray.direction*10);
        Gizmos.DrawRay(debugLine.origin, debugLine.direction * 2);
        Gizmos.DrawRay(playerRay.origin, playerRay.direction * 10);

        Gizmos.DrawWireSphere(point, 0.1f);
    }

    // Use this for initialization
    void Start () {
		currentCharge = 0;
		//charging = false;
		chargedTime = 0;
		chargeWheel.type = Image.Type.Filled;
		chargeWheel.fillMethod = Image.FillMethod.Radial360;
		chargeWheel.fillAmount = 0f;
        slowmoWheel.type = Image.Type.Filled;
        slowmoWheel.fillMethod = Image.FillMethod.Radial360;
        slowmoWheel.fillAmount = 0f;
        enemyMask = LayerMask.GetMask("Enemy");
        enemyAndDefaultMask = LayerMask.GetMask("Enemy", "Default");
        emptyMask = LayerMask.GetMask();
		playerMover = gameObject.GetComponent<PlayerMover> ();
		camera = Camera.main;
        timeScaleManager = camera.GetComponent<TimeScaleManager>();
	}

    private void UpdateSlowmoWheel()
    {
       if (charged)
        {
            slowmoWheel.fillAmount = 1 - (chargedTime / chargeTimeout);
        }
       else
        {
            slowmoWheel.fillAmount = 0;
        }
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
            UpdateSlowmoWheel();
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
                UpdateSlowmoWheel();
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
                UpdateSlowmoWheel();
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


    private RaycastHit checkAttack()
    {

        Ray attackRay;
        RaycastHit attackHit;
        RaycastHit nullHit = getNullHit();
        attackRay = camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //TEMP FOR DEBUGGING;
        playerRay = attackRay;
        Ray checkRay;
        //Vector3 startSpherePoint = camera.transform.position;
        //Vector3 endSpherePoint = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 2f));
        float radius = 1f;

        if (Physics.Raycast(attackRay, out attackHit, attackRange, enemyAndDefaultMask))
        {
            if (attackHit.transform.gameObject.tag == "Enemy")
            {
                return attackHit;
            }
        }

        RaycastHit[] hits = Physics.SphereCastAll(attackRay, radius, attackRange, enemyMask);

        if (hits.Length > 0)
        {
            RaycastHit closestHit = nullHit;
            float distance = Mathf.Infinity;
            float tempDistance = 0;
            foreach (RaycastHit hit in hits)
            {
                //if hit is closest to attackRay and tagged as enemy
                //tempDistance = DistanceToNearestPointOnLine(attackRay, hit);
                //if (tempDistance < distance)
                checkRay = new Ray(camera.transform.position, (RayFromNearestPointOnLine(attackRay, hit).point - camera.transform.position).normalized);
                ray = checkRay;
                if (Physics.Raycast(checkRay, out attackHit, attackRange, enemyAndDefaultMask))
                {
                    if (attackHit.transform.gameObject.tag == "Enemy")
                    {
                        closestHit = hit;
                    }
                }
            }
            return closestHit;
        }
        return getNullHit();
    }
    
    private RaycastHit RayFromNearestPointOnLine(Ray line, RaycastHit hit)
    {
        Vector3 targetPosition = hit.collider.gameObject.transform.position;
        Vector3 positionDifference = targetPosition - camera.transform.position;
        float magnitude = Vector3.Dot(positionDifference, line.direction);
        Vector3 origin = camera.transform.position + line.direction * magnitude;
        RaycastHit rayHit;
        Ray ray = new Ray(origin, (targetPosition - origin).normalized);
        Physics.Raycast(ray, out rayHit, enemyAndDefaultMask);

        //TEMP for debugging
        debugLine = ray;
        point = rayHit.point;

        return rayHit;
    }

    private float DistanceToNearestPointOnLine(Ray line, RaycastHit hit)
    {
        Vector3 direction = hit.point - camera.transform.position;
        float angle = Vector3.Angle(direction, line.direction);
        return hit.distance * Mathf.Sin(angle);
        /*
        Vector3 lineDir = line.direction;
        Vector3 linePoint = li
        lineDir.Normalize();//this needs to be a unit vector
        var v = point - linePoint;
        var d = Vector3.Dot(v, lineDir);
        return d;
        */
    }
}
