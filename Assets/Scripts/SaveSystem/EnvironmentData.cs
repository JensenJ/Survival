// Copyright (c) 2019 JensenJ
// NAME: EnvironmentData
// PURPOSE: Holds environment data.

using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    //Time settins
    public float timeMultiplier;
    public int daysInMonth;
    public int MonthsInYear;
    public float clockWork;

    //Temp settings
    public float tempMultiplier;
    public bool tempFahrenheit;
    public float temperature;
    public int tempPrecision;
    
    //Wind
    public float windStrengthMultiplier;
    public float windStrength;
    public int windStrengthPrecision;
    public float windAngle;
    public int windAnglePrecision;

    //Enums
    public string weather;
    public string season;


    public EnvironmentData(EnvironmentController env)
    {
        //Time
        timeMultiplier = env.timeMultiplier;
        daysInMonth = env.daysInMonth;
        MonthsInYear = env.monthsInYear;
        clockWork = env.Clockwork;

        //Temp
        tempMultiplier = env.tempMultiplier;
        tempFahrenheit = env.bIsTempFahrenheit;
        temperature = env.temperature;
        tempPrecision = env.tempPrecision;

        //Wind
        windStrengthMultiplier = env.windStrengthMultiplier;
        windStrength = env.windStrength;
        windStrengthPrecision = env.windStrengthPrecision;

        windAngle = env.windAngle;
        windAnglePrecision = env.windAnglePrecision;

        season = env.seasonEnum.ToString();
        weather = env.weatherEnum.ToString();
    }
}
