using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	[SerializeField] Image healthBar;
	[SerializeField] Image damageImage;
    [SerializeField] Text gameOverText;
	[SerializeField] float flashSpeed = 5f;
	[SerializeField] Color flashColor = new Color (1f, 0f, 0f, 0.1f);
	[SerializeField] private int maxHealth;
	private float health;
	private bool damaged;
    private ImpactReceiver impactReceiver;
    PlayerMover playerMover; 

	// Use this for initialization
	void Start () {
		health = maxHealth;
        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        healthBar.fillAmount = 1f;
        impactReceiver = gameObject.GetComponent<ImpactReceiver>();
        playerMover = gameObject.GetComponent<PlayerMover>();

    }
	
	// Update is called once per frame
	void Update () {
		if (damaged) {
			damageImage.color = flashColor;
		} else {
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		damaged = false;
	}

	public void TakeDamage(int damage, Vector3 direction, float force){
        if (playerMover.isVulnerable())
        {
            health -= damage;
            damaged = true;
            healthBar.fillAmount = health / maxHealth;
            if (health <= 0)
            {
                gameOver();
            }
            impactReceiver.AddImpact(direction.normalized, force);

            //moveDamageIndicator(direction);
        }
    }

    private void gameOver()
    {
        gameOverText.gameObject.SetActive(true);
        playerMover.death();
    }

    //to be implemented
    private void moveDamageIndicator(Vector3 direction)
    {

    }
}
