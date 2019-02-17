using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    Image staminaBar;
    float maxStamina;
    float staminaRegen;
    float staminaRegenDelay;
    [SerializeField] PlayerValues playerValues;
    float currentStamina;
    float regenDelayTimer;
    bool regenerating;

    // Use this for initialization
    void Start()
    {
        staminaBar = playerValues.staminaValues.StaminaBar;
        maxStamina = playerValues.staminaValues.MaxStamina;
        staminaRegen = playerValues.staminaValues.StaminaRegen;
        staminaRegenDelay = playerValues.staminaValues.StaminaRegenDelay;
        currentStamina = maxStamina;
        staminaBar.type = Image.Type.Filled;
        staminaBar.fillMethod = Image.FillMethod.Horizontal;
        staminaBar.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStamina < maxStamina && regenerating)
        {
            currentStamina += staminaRegen * Time.deltaTime;
            staminaBar.fillAmount = currentStamina / maxStamina;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
        else if (!regenerating)
        {
            regenDelayTimer += Time.deltaTime;
            if (regenDelayTimer >= staminaRegenDelay)
            {
                regenerating = true;
            }
        }
    }

    public bool UseStamina(float staminaUsed)
    {
        if (staminaUsed > currentStamina)
        {
            return false;
        }
        else
        {
            currentStamina -= staminaUsed;
            regenerating = false;
            regenDelayTimer = 0;
            staminaBar.fillAmount = currentStamina / maxStamina;
            return true;
        }
    }

    public void RegainStaminaWithoutRegen(int staminaGain)
    {
        if ((staminaGain + currentStamina) <= maxStamina & currentStamina >= 0)
        {
            currentStamina += staminaGain;
        }
        else if ((currentStamina + staminaGain) > maxStamina)
        {
            currentStamina = maxStamina;
        }
        staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void RegainStamina(float staminaGained)
    {
        currentStamina += staminaGained;
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            regenerating = false;
        }
        else
        {
            regenerating = true;
        }
    }
}
