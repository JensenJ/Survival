// Copyright (c) 2019 JensenJ
// NAME: EnvironmentController
// PURPOSE: Controls the environmental settings, such as time, weather and temperature.

using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    //References for drag/drop in inspector.
    [Header("References:")]
    [SerializeField] private Light sun;

    //Time settings
    [Header("Time:")]
    [SerializeField] [Range(1, 8)] public float timeMultiplier = 1f;
    [SerializeField] [Range(1, 120)] public int daysInMonth = 30;
    [SerializeField] [Range(1, 40)] public int monthsInYear = 12;
    //Slider for percentage of full day done
    [Range(0, 1)] [SerializeField] private float currentTimeOfDay = 0;

    //Time variables
    [Space(10)]
    private int Seconds = 0;
    private int Minutes = 0;
    private int Hours = 0;
    private int Day = 1;
    private int Month = 1;
    private int Year = 1;
    [SerializeField] public float Clockwork = 360; // 6 o'clock

    //Seasons
    public enum ESeasonEnum { ENone, ESpring, ESummer, EAutumn, EWinter };
    [SerializeField] public ESeasonEnum seasonEnum;

    //Variables for calculations later.
    private float DayTick;
    private float sunInitialIntensity;
    private float DayNightHours;

    //Temperature
    [Header("Temperature:")]
    //Temperature settings
    [SerializeField] [Range(0, 5)] public float tempMultiplier = 1;
    [SerializeField] public bool bIsTempFahrenheit = false;
    [SerializeField] public float temperature = 0;
    [SerializeField] [Range(0, 4)] public int tempPrecision = 2;

    //Temperature variables
    bool bNewGenerationTemp = true;
    bool bHasGeneratedTemp = false;
    float lastTemp = 0;
    float minGenTemp = 0;
    float maxGenTemp = 7;
    float generatedTemp = 5.0f;
    float[] gameTemp;
    float averageTemp;

    //Wind
    [Header("Wind:")]
    //Wind settings
    [SerializeField] [Range(0, 5)] public float windStrengthMultiplier = 1;
    [SerializeField] public float windStrength = 0;
    [SerializeField] [Range(0, 4)] public int windStrengthPrecision = 2;

    //Wind variable
    bool bNewGenerationWind = true;
    bool bHasGeneratedWind = false;
    float lastWind;
    float generatedWind;
    float[] gameWind;
    float averageWind;

    //Wind angle settings
    [SerializeField] public float windAngle = 0;
    [SerializeField] [Range(0, 4)] public int windAnglePrecision = 2;

    //Wind angle variables
    bool bNewGenerationWindAngle = true;
    bool bHasGeneratedWindAngle = false;
    float lastWindAngle;
    float generatedWindAngle;
    float[] gameWindAngle;
    float averageWindAngle;

    //Weather
    [Header("Weather:")]
    //Current weather
    [SerializeField] public EWeatherEnum weatherEnum;

    //Weather variables
    bool bNewGenerationWeather = true;
    bool bHasGeneratedWeather = false;
    public enum EWeatherEnum { ENone, ESunny, EOvercast, ECloudy, ERain, ESnow, EThunder, EFog };
    EWeatherEnum lastWeather;
    EWeatherEnum weather;

    void Start()
    {
        //Setting defaults
        sun = transform.root.GetChild(0).GetComponent<Light>();
        sunInitialIntensity = sun.intensity;
        gameTemp = new float[3];
        gameWind = new float[3];
        gameWindAngle = new float[3];
    }

    void Update()
    {
        //Functions to run every tick.
        SetClockwork();
        Clock();
        Calendar();

        Season();
        UpdateSun();
        Temperature();
        WindStrength();
        WindAngle();
        Weather();
    }

    void SetClockwork()
    {
        //Calculations for working out gamespeed and time.
        float DeltaTimeUnit = (Time.deltaTime / 0.25f * 0.24f) * timeMultiplier;
        float AddedClockwork = DeltaTimeUnit + Clockwork;
        float NewDayTick = AddedClockwork / (60 * 24);
        float Remainder = AddedClockwork % (60.0f * 24.0f);
        Clockwork = Remainder;
        DayTick = NewDayTick;
        currentTimeOfDay = NewDayTick;

        //Restart time of day when day is finished.
        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }
    }

    void Clock()
    {
        //Time calculations.
        // Seconds
        float ClockworkSeconds = Clockwork * 3600;

        float NewSeconds = ClockworkSeconds / 60;
        float RemainderSeconds = NewSeconds % 60.0f;
        Seconds = Mathf.FloorToInt(RemainderSeconds);

        // Minutes
        float NewMinutes = NewSeconds / 60;
        float RemainderMinutes = NewMinutes % 60.0f;
        Minutes = Mathf.FloorToInt(RemainderMinutes);

        // Hours
        float NewHours = NewMinutes / 60;
        DayNightHours = NewHours % 60.0f;
        Hours = Mathf.FloorToInt(DayNightHours);
    }

    void UpdateSun()
    {
        //Updates sun position based on currentTimeOfDay
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;

        if (currentTimeOfDay <= 0.25f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }

    void Calendar()
    {
        //Calendar function
        Day = (int)DayTick + Day;
        if (Day > daysInMonth)
        {
            Day = 1;
            Month++;
        }
        if (Month > monthsInYear)
        {
            Month = 1;
            Year++;
        }
    }

    void Season()
    {
        //Get season based on month
        if (Month == 12 || Month == 1 || Month == 2)
        {
            seasonEnum = ESeasonEnum.EWinter;
        }
        else if (Month == 3 || Month == 4 || Month == 5)
        {
            seasonEnum = ESeasonEnum.ESpring;
        }
        else if (Month == 6 || Month == 7 || Month == 8)
        {
            seasonEnum = ESeasonEnum.ESummer;
        }
        else if (Month == 9 || Month == 10 || Month == 11)
        {
            seasonEnum = ESeasonEnum.EAutumn;
        }
        else
        {
            print("Season::Setting season failed!");
            seasonEnum = ESeasonEnum.ENone;
        }
    }

    void Temperature()
    {
        //Check whether its a new game for generation
        if (bNewGenerationTemp)
        {
            lastTemp = Random.Range(0, 7);
            bNewGenerationTemp = false;
        }

        //Only run this at the start of every hour.
        if (Minutes == 0)
        {
            if (!bHasGeneratedTemp) //Stops repeats of code within that minute.
            {
                for (int i = 0; i < 3; i++) //Repeats three times to be used in average later.
                {
                    //Sets min and max temps based on season.
                    if (seasonEnum == ESeasonEnum.EWinter)
                    {
                        maxGenTemp = 8.0f * tempMultiplier;
                        minGenTemp = -7.0f * tempMultiplier;
                    }
                    else if (seasonEnum == ESeasonEnum.ESpring)
                    {
                        maxGenTemp = 12.0f * tempMultiplier;
                        minGenTemp = 2.0f * tempMultiplier;
                    }
                    else if (seasonEnum == ESeasonEnum.ESummer)
                    {
                        maxGenTemp = 20.0f * tempMultiplier;
                        minGenTemp = 7.0f * tempMultiplier;
                    }
                    else if (seasonEnum == ESeasonEnum.EAutumn)
                    {
                        maxGenTemp = 13.0f * tempMultiplier;
                        minGenTemp = -3.0f * tempMultiplier;
                    }
                    else
                    {
                        print("Temperature::Check for season failed!");
                    }

                    //Random generation between min and max generated.
                    generatedTemp = Random.Range(minGenTemp, maxGenTemp);

                    //Makes sure temperature gap is not too large/unreasonable.
                    if ((generatedTemp - lastTemp) > 4)
                    {
                        generatedTemp = lastTemp + Random.Range(2.0f, 3.5f);
                    } else if ((lastTemp - generatedTemp) > 4)
                    {
                        generatedTemp = lastTemp - Random.Range(0.0f, 2.5f);
                    }

                    //Gradual increase towards midday and decrease away from midday.
                    if (Hours <= 13 && Hours > 1)
                    {
                        generatedTemp = generatedTemp + Random.Range(1.5f, 3.0f);
                    } else if (Hours > 13 && Hours < 24)
                    {
                        generatedTemp = generatedTemp - Random.Range(0.2f, 1.5f);
                    }
                    //Set temperature into array for average.
                    gameTemp[i] = generatedTemp;
                }
                //calculate avergae.
                averageTemp = (gameTemp[0] + gameTemp[1] + gameTemp[2]) / 3;

                //apply wind strength to temperature
                averageTemp = averageTemp - (windStrength / 5);

                //Makes sure temperature is not below absolute zero.
                if (averageTemp < -273.0f)
                {
                    averageTemp = -273.0f;
                }
                //sets last temp = new temp
                lastTemp = averageTemp;

                //converts to fahrenheit if needed.
                if (bIsTempFahrenheit)
                {
                    averageTemp = (averageTemp * (9 / 5)) + 32;
                }

                //Rounds to chosen number of dp
                averageTemp = (float)System.Math.Round(averageTemp, tempPrecision);

                bHasGeneratedTemp = true; //Stops repetition of code.
            }
        }
        else
        {
            bHasGeneratedTemp = false; //Makes sure code does not happen when minute is not 0
        }
        temperature = averageTemp; //sets temperature of world to generated temp.
    }

    void WindStrength()
    {
        //check for new game
        if (bNewGenerationWind)
        {
            lastWind = Random.Range(0, 7);
            bNewGenerationWind = false;
        }

        //wind only changes every 6 hours at 0 minutes
        if ((Hours == 0 && Minutes == 0) ||
            (Hours == 6 && Minutes == 0) ||
            (Hours == 12 && Minutes == 0) ||
            (Hours == 18 && Minutes == 0))
        {
            if (!bHasGeneratedWind)
            {
                for (int i = 0; i < 3; i++) //repeats for average
                {
                    generatedWind = Random.Range(-3.0f, 3.0f); //generate base wind

                    //makes sure wind difference is not too large
                    if ((generatedWind - lastWind) > 3)
                    {
                        generatedWind = lastWind + Random.Range(0.0f, 3.0f);
                    } else if ((lastWind - generatedWind) < 3)
                    {
                        generatedWind = lastWind - Random.Range(0.0f, 3.0f);
                    }

                    //sets into array for average
                    gameWind[i] = generatedWind;
                }

                //calculates average
                averageWind = (gameWind[0] + gameWind[1] + gameWind[2]) / 3;

                //makes sure wind is not too extreme and wind is not less than 0
                if (averageWind > 2.5f)
                {
                    averageWind = averageWind - Random.Range(0.3f, 0.9f);
                } else if (averageWind < 0)
                {
                    averageWind = Random.Range(0.3f, 1.3f);
                }
                //sets last wind = generated wind
                lastWind = averageWind;
                //applies multiplier.
                averageWind = averageWind * windStrengthMultiplier;

                //rounds to chosen number of dp
                averageWind = (float)System.Math.Round(averageWind, windStrengthPrecision);

                bHasGeneratedWind = true; //stops repetition
            }
        }
        else
        {
            bHasGeneratedWind = false;
        }
        windStrength = averageWind; //sets actual world wind = generated wind.
    }

    void WindAngle() {
        //Checks for new generation
        if (bNewGenerationWindAngle) {
            lastWindAngle = Random.Range(0.0f, 360.0f);
            bNewGenerationWindAngle = false;
        }

        //only generate on new hour.
        if (Minutes == 0) { 
            //stops repetition
            if (!bHasGeneratedWindAngle) {
                for (int i = 0; i < 3; i++) {  //repeats for average
                    //base wind angle
                    generatedWindAngle = Random.Range(0.0f, 360.0f);

                    //makes sure difference is not too great.
                    if ((generatedWindAngle - lastWindAngle) > 15) {
                        generatedWindAngle = lastWindAngle + Random.Range(0.0f, 10.0f);
                    }
                    else if ((lastWindAngle - generatedWindAngle) < 15) {
                        generatedWindAngle = lastWindAngle - Random.Range(0.0f, 10.0f);
                    }
                    //puts into array for average calculation
                    gameWindAngle[i] = generatedWindAngle;
                }

                //calculates average.
                averageWindAngle = (gameWindAngle[0] + gameWindAngle[1] + gameWindAngle[2]) / 3;

                //used for bridging gap between 0 and 360 as you cannot go above 360 in angle and cannot go below 0.
                if (averageWindAngle < 0) {
                    averageWindAngle = 340 - averageWindAngle;
                }
                else if (averageWindAngle > 360) {
                    averageWindAngle = averageWindAngle - 340;
                }
                //sets last angle = generated angle
                lastWindAngle = averageWindAngle;

                //rounds to chosen number of dp
                averageWindAngle = (float)System.Math.Round(averageTemp, windAnglePrecision);

                bHasGeneratedWindAngle = true; //stops repetitions.
            }
        }
        else {
            bHasGeneratedWindAngle = false; 
        }

        windAngle = averageWindAngle; //sets actual wind angle to generated one.
    }

    void Weather()
    {
        //only change weather when minute = 0 (new hour)
        if (Minutes == 0) { 
            //stops repetitions
            if (!bHasGeneratedWeather) {
                //checks for new generation
                if (bNewGenerationWeather == true) {
                    //randomly chooses number which is used as base weather
                    int GeneratedWeather = Random.Range(1, 4);

                    if (GeneratedWeather == 1) {
                        weather = EWeatherEnum.ESunny;
                    }
                    else if (GeneratedWeather == 2) {
                        weather = EWeatherEnum.ECloudy;
                    }
                    else if (GeneratedWeather == 3) {
                        weather = EWeatherEnum.EOvercast;
                    }
                    else if (GeneratedWeather == 4) {
                        weather = EWeatherEnum.ERain;
                    }
                    else {
                        weather = EWeatherEnum.ENone;
                    }
                    bNewGenerationWeather = false; //makes sure algorithm including last weather is used.
                }
                
                else if (bNewGenerationWeather == false) {
                    //use last weather as a base for the next weather that can happen, stops sudden changes from sunny to thunderstorm etc.
                    if (lastWeather == EWeatherEnum.ESunny) {
                        if (seasonEnum == ESeasonEnum.ESpring) {
                            //generate random number to use as basis for probability to generate weather, i.e. 66% to be sunny if it has just been sunny if in spring season.
                            int Sun = Random.Range(0, 2);
                            if (Sun == 0 || Sun == 1) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Sun == 2) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        //This code repeats for all different last weathers and what season it is.
                        
                        else if (seasonEnum == ESeasonEnum.ESummer) {
                            int Sun = Random.Range(0, 4);
                            if (Sun == 0 || Sun == 2 || Sun == 3) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Sun == 4) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EAutumn) {
                            int Sun = Random.Range(0, 1);
                            if (Sun == 0) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Sun == 1) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EWinter) {
                            int Sun = Random.Range(0, 2);
                            if (Sun == 0) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Sun == 1 || Sun == 2) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                    }
                    else if (lastWeather == EWeatherEnum.ECloudy) {
                        if (seasonEnum == ESeasonEnum.ESpring) {
                            int Cloudy = Random.Range(0, 7);
                            if (Cloudy == 0 || Cloudy == 1) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Cloudy == 2 || Cloudy == 3 || Cloudy == 4) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Cloudy == 5 || Cloudy == 6) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Cloudy == 7) {
                                weather = EWeatherEnum.EFog;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.ESummer) {
                            int Cloudy = Random.Range(0, 7);
                            if (Cloudy == 0 || Cloudy == 1 || Cloudy == 2) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Cloudy == 3 || Cloudy == 4 || Cloudy == 5 || Cloudy == 6) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Cloudy == 7) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Cloudy == 8) {
                                weather = EWeatherEnum.EFog;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EAutumn) {
                            int Cloudy = Random.Range(0, 9);
                            if (Cloudy == 0 || Cloudy == 1 || Cloudy == 2 || Cloudy == 3 || Cloudy == 4) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Cloudy == 5 || Cloudy == 6) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Cloudy == 7 || Cloudy == 8) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Cloudy == 9) {
                                weather = EWeatherEnum.EFog;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EWinter) {
                            int Cloudy = Random.Range(0, 8);
                            if (Cloudy == 0 || Cloudy == 1 || Cloudy == 2 || Cloudy == 3) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Cloudy == 4) {
                                weather = EWeatherEnum.ESunny;
                            }
                            else if (Cloudy == 5 || Cloudy == 6 || Cloudy == 7) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Cloudy == 8) {
                                weather = EWeatherEnum.EFog;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                    }
                    else if (lastWeather == EWeatherEnum.EOvercast) {
                        if (seasonEnum == ESeasonEnum.ESpring) {
                            int Overcast = Random.Range(0, 9);
                            if (Overcast == 0 || Overcast == 1 || Overcast == 2) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Overcast == 3 || Overcast == 4 || Overcast == 5 || Overcast == 6) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Overcast == 7 || Overcast == 8) {
                                weather = EWeatherEnum.ERain;
                            }
                            else if (Overcast == 9) {
                                weather = EWeatherEnum.EThunder;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.ESummer) {
                            int Overcast = Random.Range(0, 5);
                            if (Overcast == 0 || Overcast == 1) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Overcast == 2 || Overcast == 3 || Overcast == 4) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Overcast == 5) {
                                weather = EWeatherEnum.ERain;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EAutumn) {
                            int Overcast = Random.Range(0, 8);
                            if (Overcast == 0 || Overcast == 1 || Overcast == 2) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Overcast == 3 || Overcast == 4 || Overcast == 5) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Overcast == 6 || Overcast == 7) {
                                weather = EWeatherEnum.ERain;
                            }
                            else if (Overcast == 8) {
                                weather = EWeatherEnum.EThunder;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                        else if (seasonEnum == ESeasonEnum.EWinter) {
                            int Overcast = Random.Range(0, 9);
                            if (Overcast == 0 || Overcast == 1 || Overcast == 2) {
                                weather = EWeatherEnum.EOvercast;
                            }
                            else if (Overcast == 3 || Overcast == 4) {
                                weather = EWeatherEnum.ECloudy;
                            }
                            else if (Overcast == 5) {
                                weather = EWeatherEnum.ESnow;
                            }
                            else if (Overcast == 6 || Overcast == 7 || Overcast == 8) {
                                weather = EWeatherEnum.ERain;
                            }
                            else if (Overcast == 9) {
                                weather = EWeatherEnum.EThunder;
                            }
                            else {
                                weather = EWeatherEnum.ENone;
                            }
                        }
                    }
                    else if (lastWeather == EWeatherEnum.ERain) {
                        int Rain = Random.Range(0, 8);
                        if (Rain == 0 || Rain == 1 || Rain == 2 || Rain == 3) {
                            weather = EWeatherEnum.ERain;
                        }
                        else if (Rain == 4 || Rain == 5 || Rain == 6) {
                            weather = EWeatherEnum.EOvercast;
                        }
                        else if (Rain == 7) {
                            weather = EWeatherEnum.EFog;
                        }
                        else if (Rain == 8) {
                            weather = EWeatherEnum.EThunder;
                        }
                        else {
                            weather = EWeatherEnum.ENone;
                        }
                    }
                    else if (lastWeather == EWeatherEnum.EThunder) {
                        int Thunder = Random.Range(0, 6);
                        if (Thunder == 0 || Thunder == 1) {
                            weather = EWeatherEnum.EThunder;
                        }
                        else if (Thunder == 2 || Thunder == 3 || Thunder == 4) {
                            weather = EWeatherEnum.ERain;
                        }
                        else if (Thunder == 5) {
                            weather = EWeatherEnum.EOvercast;
                        }
                        else if (Thunder == 6) {
                            weather = EWeatherEnum.ECloudy;
                        }
                        else {
                            weather = EWeatherEnum.ENone;
                        }
                    }
                    else if (lastWeather == EWeatherEnum.EFog) {
                        int Fog = Random.Range(0, 3);
                        if (Fog == 0) {
                            weather = EWeatherEnum.EFog;
                        }
                        else if (Fog == 1 || Fog == 2) {
                            weather = EWeatherEnum.ESunny;
                        }
                        else if (Fog == 3) {
                            weather = EWeatherEnum.ECloudy;
                        }
                        else {
                            weather = EWeatherEnum.ENone;
                        }
                    }
                    else if (lastWeather == EWeatherEnum.ESnow) {
                        int Snow = Random.Range(0, 3);
                        if (Snow == 0 || Snow == 1) {
                            weather = EWeatherEnum.ESnow;
                        }
                        else if (Snow == 2) {
                            weather = EWeatherEnum.ECloudy;
                        }
                        else if (Snow == 3) {
                            weather = EWeatherEnum.EOvercast;
                        }
                        else {
                            weather = EWeatherEnum.ENone;
                        }
                    }
                }
                else {
                    weather = EWeatherEnum.ENone;
                }

                lastWeather = weather; //Sets last weather = new generated weather
                bNewGenerationWeather = false; //Stops repetitions
                
                bHasGeneratedWeather = true; //Makes sure generation only happens once
            }

        }
        else {
            bHasGeneratedWeather = false; //Resets the variable for the next hour
        }
        weatherEnum = weather; //Sets actual world weather = generated weather.
    }
}