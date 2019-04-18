// Copyright (c) 2019 JensenJ
// NAME: Attributes
// PURPOSE: Keeps track of attributes for an object

using UnityEngine;
using UnityEngine.UI;

public class Attributes : MonoBehaviour
{
    //Health
    [Header("Health:")]
    [SerializeField] [Range(1, 1000)] private float maxHealthMeter = 100.0f;
    [SerializeField] [Range(0.01f, 3)] private float healthMeterDrainSpeed = 1.0f;
    [SerializeField] [Range(0.01f, 3)] private float healthMeterRegenSpeed = 0.5f;
    [SerializeField] [Range(0, 1)] private float healthPercentage = 1.0f;
    [SerializeField] public bool bIsStaticHealth = false;
    [SerializeField] public bool bIsDead = false;
    [SerializeField] public bool bCanRegenHealth = true;
    private float healthMeter;
    private Image healthBar = null;

    //Stamina
    [Header("Stamina:")]
    [SerializeField] [Range(1, 1000)] private float maxStaminaMeter = 100.0f;
    [SerializeField] [Range(0.01f, 20)] private float staminaMeterDrainSpeed = 10.0f;
    [SerializeField] [Range(0.01f, 10)] private float staminaMeterRegenSpeed = 3.0f;
    [SerializeField] [Range(0, 1)] private float staminaPercentage = 1.0f;
    [SerializeField] public bool bIsStaticStamina = false;
    [SerializeField] public bool bIsExhausted = false;
    [SerializeField] public bool bCanRegenStamina = true;
    private float staminaMeter;
    private Image staminaBar = null;

    //Hunger
    [Header("Hunger:")]
    [SerializeField] [Range(1, 1000)] private float maxHungerMeter = 100.0f;
    [SerializeField] [Range(0.01f, 3)] private float hungerMeterDrainSpeed = 0.125f;
    [SerializeField] [Range(0.01f, 20)] private float hungerMeterRegenSpeed = 10.0f;
    [SerializeField] [Range(0, 1)] private float hungerPercentage = 1.0f;
    [SerializeField] public bool bIsStaticHunger = false;
    [SerializeField] public bool bIsStarving = false;
    [SerializeField] public bool bCanRegenHunger = false;
    private float hungerMeter;
    private Image hungerBar = null;

    //Thirst
    [Header("Thirst:")]
    [SerializeField] [Range(1, 1000)] private float maxThirstMeter = 100.0f;
    [SerializeField] [Range(0.01f, 3)] private float thirstMeterDrainSpeed = 0.25f;
    [SerializeField] [Range(0.01f, 20)] private float thirstMeterRegenSpeed = 10.0f;
    [SerializeField] [Range(0, 1)] private float thirstPercentage = 1.0f;
    [SerializeField] public bool bIsStaticThirst = false;
    [SerializeField] public bool bIsDehydrated = false;
    [SerializeField] public bool bCanRegenThirst = false;
    private float thirstMeter;
    private Image thirstBar = null;

    void Start()
    {
        //UI Transforms
        Transform panelTransform = transform.root.GetChild(1).GetChild(0).GetChild(0);
        healthBar = panelTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        staminaBar = panelTransform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        thirstBar = panelTransform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        hungerBar = panelTransform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();

        //Set initial health meter values
        healthMeter = maxHealthMeter;
        staminaMeter = maxStaminaMeter;
        hungerMeter = maxHungerMeter;
        thirstMeter = maxThirstMeter;

        thirstMeterDrainSpeed /= 10;
        hungerMeterDrainSpeed /= 10;
    }

    void Update()
    {
        //Update meters.
        HealthMeter();
        StaminaMeter();
        HungerMeter();
        ThirstMeter();
    }

    //Manages health meter
    void HealthMeter()
    {
        //Checks whether health can be changed
        if (!bIsStaticHealth)
        {
            //Checks whether regeneration can happen
            if (bCanRegenHealth)
            {
                ChangeHealthLevel(Time.deltaTime, true);
            }

            //Makes sure health does not go negative, and sets bool values.
            if (healthMeter <= 0.0f)
            {
                bIsDead = true;
                bCanRegenHealth = false;
                healthMeter = 0.0f;

            }
            //Otherwise, keep normal
            else
            {
                bIsDead = false;
                bCanRegenHealth = true;
            }

            //Makes sure health does not regenerate over the max limit
            if (healthMeter >= maxHealthMeter)
            {
                healthMeter = maxHealthMeter;
            }
            //Calculates percentage and applies to bar.
            healthPercentage = healthMeter / maxHealthMeter;
            healthBar.fillAmount = healthPercentage;
        }
    }

    //Manages stamina meter
    void StaminaMeter()
    {
        //Checks whether stamina can be changed
        if (!bIsStaticStamina)
        {
            //Checks whether regeneration can happen
            if (bCanRegenStamina)
            {
                staminaMeter += Time.deltaTime * staminaMeterRegenSpeed;
            }
            //Makes sure stamina does not go negative, and sets bool values.
            if (staminaMeter <= 0.0f)
            {
                bIsExhausted = true;
                staminaMeter = 0.0f;

            }
            //Otherwise, keep normal
            else
            {
                bIsExhausted = false;
            }

            //Makes sure stamina does not regenerate over the max limit
            if (staminaMeter >= maxStaminaMeter)
            {
                staminaMeter = maxStaminaMeter;
            }

            //Calculates percentage and applies to bar.
            staminaPercentage = staminaMeter / maxStaminaMeter;
            staminaBar.fillAmount = staminaPercentage;
        }
    }

    //Manages hunger meter
    void HungerMeter()
    {
        //Checks whether hunger can be changed
        if (!bIsStaticHunger)
        {
            //Drains hunger over time
            ChangeHungerLevel(-Time.deltaTime, true);

            //Checks whether regeneration can happen
            if (bCanRegenHunger)
            {
                ChangeHungerLevel(Time.deltaTime, true);
            }
            //Makes sure hunger does not go negative, and sets bool values. 
            //Also drains health and stamina if hunger is empty
            if (hungerMeter <= 0.0f)
            {
                bIsStarving = true;
                ChangeHealthLevel(-Time.deltaTime, true);
                ChangeStaminaLevel(-Time.deltaTime, true);
                hungerMeter = 0.0f;

            }
            //Otherwise, keep normal
            else
            {
                bIsStarving = false;
            }

            //Makes sure hunger does not regenerate over the max limit
            if (hungerMeter >= maxHungerMeter)
            {
                hungerMeter = maxHungerMeter;
            }

            //Calculates percentage and applies to bar.
            hungerPercentage = hungerMeter / maxHungerMeter;
            hungerBar.fillAmount = hungerPercentage;
        }
    }

    //Manages thirst meter
    void ThirstMeter()
    {
        //Checks whether thirst can be changed
        if (!bIsStaticThirst)
        {
            //Drains thirst over time
            ChangeThirstLevel(-Time.deltaTime, true);

            //Checks whether regeneration can happen
            if (bCanRegenThirst)
            {
                ChangeThirstLevel(Time.deltaTime, true);
            }
            //Makes sure thirst does not go negative, also sets bool values.
            //Also drains health and stamina if thirst is empty
            if (thirstMeter <= 0.0f)
            {
                bIsDehydrated = true;
                ChangeHealthLevel(-Time.deltaTime, true);
                ChangeStaminaLevel(-Time.deltaTime, true);
                thirstMeter = 0.0f;
            }
            //Otherwise, keep normal
            else
            {
                bIsDehydrated = false;
            }

            //Makes sure thirst does not regenerate over the max limit
            if (thirstMeter >= maxThirstMeter)
            {
                thirstMeter = maxThirstMeter;
            }

            //Calculates percentage and applies to bar.
            thirstPercentage = thirstMeter / maxThirstMeter;
            thirstBar.fillAmount = thirstPercentage;
        }
    }

    //Function for changing health level, negatives allowed for deducting health
    public void ChangeHealthLevel(float m_amount, bool m_bIsChangeOverTime)
    {
        //Checks whether health can be changed
        if (!bIsStaticHealth)
        {
            //Checks whether this is instant damage or over time
            if (m_bIsChangeOverTime)
            {
                //If over time, applies either positive or negative damage by the appropriate multiplier.
                if (m_amount < 0)
                {
                    healthMeter += m_amount * healthMeterDrainSpeed;
                }
                else
                {
                    healthMeter += m_amount * healthMeterRegenSpeed;
                }
            }
            else
            {
                //If instant damange, adds value to health meter, this value can be negative to deduct health.
                healthMeter += m_amount;
            }
        }
    }

    //Function for changing stamina level, negatives allowed for deducting stamina
    public void ChangeStaminaLevel(float m_amount, bool m_bIsChangeOverTime)
    {
        //Checks whether stamina can be changed
        if (!bIsStaticStamina)
        {
            //Checks whether this is instant damage or over time
            if (m_bIsChangeOverTime)
            {
                //If over time, applies either positive or negative damage by the appropriate multiplier.
                if (m_amount < 0)
                {
                    staminaMeter += m_amount * staminaMeterDrainSpeed;
                }
                else
                {
                    staminaMeter += m_amount * staminaMeterRegenSpeed;
                }
            }
            else
            {
                //If instant damange, adds value to stamina meter, this value can be negative to deduct stamina.
                staminaMeter += m_amount;
            }
        }
    }

    //Function for changing hunger level, negatives allowed for deducting hunger
    public void ChangeHungerLevel(float m_amount, bool m_bIsChangeOverTime)
    {
        //Checks whether hunger can be changed
        if (!bIsStaticHunger)
        {
            //Checks whether this is instant damage or over time
            if (m_bIsChangeOverTime)
            {
                //If over time, applies either positive or negative damage by the appropriate multiplier.
                if (m_amount < 0)
                {
                    hungerMeter += m_amount * hungerMeterDrainSpeed;
                }
                else
                {
                    hungerMeter += m_amount * hungerMeterRegenSpeed;
                }
            }
            else
            {
                //If instant damange, adds value to hunger meter, this value can be negative to deduct hunger.
                hungerMeter += m_amount;
            }
        }
    }

    //Function for changing thirst level, negatives allowed for deducting thirst
    public void ChangeThirstLevel(float m_amount, bool m_bIsChangeOverTime)
    {
        //Checks whether thirst can be changed
        if (!bIsStaticThirst)
        {
            //Checks whether this is instant damage or over time
            if (m_bIsChangeOverTime)
            {
                //If over time, applies either positive or negative damage by the appropriate multiplier.
                if (m_amount < 0)
                {
                    thirstMeter += m_amount * thirstMeterDrainSpeed;
                }
                else
                {
                    thirstMeter += m_amount * thirstMeterRegenSpeed;
                }
            }
            else
            {
                //If instant damange, adds value to thirst meter, this value can be negative to deduct thirst.
                thirstMeter += m_amount;
            }
        }
    }

    //Getter for health
    public float GetHealthLevel()
    {
        return healthMeter;
    }
    //Getter for stamina
    public float GetStaminaLevel()
    {
        return staminaMeter;
    }
    //Getter for hunger
    public float GetHungerLevel()
    {
        return hungerMeter;
    }
    //Getter for thirst
    public float GetThirstLevel()
    {
        return thirstMeter;
    }
}