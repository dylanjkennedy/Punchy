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
	// Use this for initialization
	void Start () {
		health = maxHealth;
        healthBar.type = Image.Type.Filled;
        //healthBar.fillMethod = Image.fillMethod.Horizontal;
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

	public void TakeDamage(int damage){
		health -= damage;
		damaged = true;
		healthBar.fillAmount = health/maxHealth;
        if (health <= 0)
        {
            gameOver();
        }
	}

    private void gameOver()
    {
        gameOverText.gameObject.SetActive(true);
        this.gameObject.GetComponent<PlayerMover>().death();
    }
}
