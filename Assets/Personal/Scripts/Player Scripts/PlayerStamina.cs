using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour {
    [SerializeField] Image staminaBar;
    [SerializeField] float maxStamina;
    [SerializeField] float staminaRegen;
    [SerializeField] float regenDelay;
    float stamina;
    float regenDelayTimer;
    bool regenerating;
    PlayerMover playerMover;

    // Use this for initialization
    void Start () {
        stamina = maxStamina;
        staminaBar.type = Image.Type.Filled;
        staminaBar.fillMethod = Image.FillMethod.Horizontal;
        staminaBar.fillAmount = 1f;
        playerMover = gameObject.GetComponent<PlayerMover>();
    }
	
	// Update is called once per frame
	void Update () {
		if (stamina < maxStamina && regenerating)
        {
            stamina += staminaRegen * Time.deltaTime;
            staminaBar.fillAmount = stamina / maxStamina;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
        else if (!regenerating)
        {
            regenDelayTimer += Time.deltaTime;
            if (regenDelayTimer >= regenDelay)
            {
                regenerating = true;
            }
        }
	}

    public bool UseStamina(float staminaUsed)
    {
        if (staminaUsed > stamina)
        {
            return false;
        }
        else
        {
            stamina -= staminaUsed;
            regenerating = false;
            regenDelayTimer = 0;
            staminaBar.fillAmount = stamina / maxStamina;
            return true;
        }
    }
}
