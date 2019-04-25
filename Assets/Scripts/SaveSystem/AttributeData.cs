// Copyright (c) 2019 JensenJ
// NAME: AttributeData
// PURPOSE: Holds data for attributes for saving and loading

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeData
{
    //Attribute data
    public float[] maxValues;
    public float[] drainSpeeds;
    public float[] regenSpeeds;
    public bool[] canRegen;

    public AttributeData(Attributes attributes)
    {
        //Max values for attributes
        maxValues = new float[4];
        maxValues[0] = attributes.maxHealthMeter;
        maxValues[1] = attributes.maxStaminaMeter;
        maxValues[2] = attributes.maxThirstMeter;
        maxValues[3] = attributes.maxHungerMeter;

        //Drain speeds for attributes
        drainSpeeds = new float[4];
        drainSpeeds[0] = attributes.healthMeterDrainSpeed;
        drainSpeeds[1] = attributes.staminaMeterDrainSpeed;
        drainSpeeds[2] = attributes.thirstMeterDrainSpeed;
        drainSpeeds[3] = attributes.hungerMeterDrainSpeed;

        //Regen speeds for attributes
        regenSpeeds = new float[4];
        regenSpeeds[0] = attributes.healthMeterRegenSpeed;
        regenSpeeds[1] = attributes.staminaMeterRegenSpeed;
        regenSpeeds[2] = attributes.thirstMeterRegenSpeed;
        regenSpeeds[3] = attributes.hungerMeterRegenSpeed;

        //Whether regeneration can happen for the attributes
        canRegen = new bool[4];
        canRegen[0] = attributes.bCanRegenHealth;
        canRegen[1] = attributes.bCanRegenStamina;
        canRegen[2] = attributes.bCanRegenThirst;
        canRegen[3] = attributes.bCanRegenHunger;
    }
}
