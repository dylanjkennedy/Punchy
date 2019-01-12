using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {
	Image healthBar;
	Image damageImage;
    Text gameOverText;
    [SerializeField] Canvas gameOverCanvas;
	float flashSpeed = 5f;
	Color flashColor = new Color (1f, 0f, 0f, 0.1f);
	private int maxHealth;
	private float health;
	private bool damaged;
    private ImpactReceiver impactReceiver;
    PlayerMover playerMover;
    PlayerValues playerValues;
    AudioSource audioSource;
    AudioClip hitSound;

	// Use this for initialization
	void Start () {
        impactReceiver = gameObject.GetComponent<ImpactReceiver>();
        playerMover = gameObject.GetComponent<PlayerMover>();
        audioSource = gameObject.GetComponent<AudioSource>();
        playerValues = playerMover.playerValues;

        healthBar = playerValues.healthValues.HealthBar;
        damageImage = playerValues.healthValues.DamageImage;
        gameOverCanvas = playerValues.healthValues.GameOverCanvas;
        flashSpeed = playerValues.healthValues.FlashSpeed;
        flashColor = playerValues.healthValues.FlashColor;
        maxHealth = playerValues.healthValues.MaxHealth;
        hitSound = playerValues.healthValues.HitSound;

        health = maxHealth;
        healthBar.type = Image.Type.Filled;
        healthBar.fillMethod = Image.FillMethod.Horizontal;
        healthBar.fillAmount = 1f;
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
            audioSource.PlayOneShot(hitSound, Mathf.Clamp(damage/4f, 0f, 1f));
            if (health <= 0)
            {
                GameOver();
            }
            impactReceiver.AddImpact(direction.normalized, force);

            //moveDamageIndicator(direction);
        }
    }

    private void GameOver()
    {
        //gameOverText.gameObject.SetActive(true);
        gameOverCanvas.gameObject.SetActive(true);
        playerMover.Die();
        playerMover.MouseLook.SetCursorLock(false);
    }

    //to be implemented
    private void MoveDamageIndicator(Vector3 direction)
    {

    }
}
