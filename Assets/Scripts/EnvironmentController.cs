using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    [Header("References:")]
    [SerializeField] private Light sun;

    [Header("Time:")]
    [SerializeField] private bool bIsTimeEnabled = true;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private int daysInMonth = 30;
    [SerializeField] private int monthsInYear = 12;

    [Range(0, 1)] [SerializeField] private float currentTimeOfDay = 0;

    [Space(10)]
    [SerializeField] private int Seconds = 0;
    [SerializeField] private int Minutes = 0;
    [SerializeField] private int Hours = 0;
    [SerializeField] private int Day = 1;
    [SerializeField] private int Month = 1;
    [SerializeField] private int Year = 1;
    [SerializeField] private float Clockwork;

    enum ESeasonEnum { ENone, ESpring, ESummer, EAutumn, EWinter };
    [SerializeField] ESeasonEnum seasonEnum;


    private float DayTick;
    private float sunInitialIntensity;
    private float DayNightHours;

    [Header("Temperature:")]
    [SerializeField] float tempMultiplier = 1;
    [SerializeField] bool bIsTempFahrenheit = false;
    [SerializeField] float temperature = 0;
    bool bNewGenerationTemp = true;
    bool bHasGeneratedTemp = false;
    float lastTemp = 0;
    float minGenTemp = 0;
    float maxGenTemp = 7;
    float generatedTemp = 5.0f;
    float[] gameTemp;
    float averageTemp;

    [Header("Wind:")]
    [SerializeField] float windStrengthMultiplier = 1;
    [SerializeField] float windStrength = 0;
    bool bNewGenerationWind = true;
    bool bHasGeneratedWind = false;
    float lastWind;
    float generatedWind;
    float[] gameWind;
    float averageWind;

    [SerializeField] float windAngle = 0;
    bool bNewGenerationWindAngle = true;
    bool bHasGeneratedWindAngle = false;
    float lastWindAngle;
    float generatedWindAngle;
    float[] gameWindAngle;
    float averageWindAngle;

    [Header("Weather:")]
    [SerializeField] EWeatherEnum weatherEnum;
    bool bNewGenerationWeather = true;
    bool bHasGeneratedWeather = false;
    int counter;
    enum EWeatherEnum { ENone, ESunny, EOvercast, ECloudy, ERain, ESnow, EThunder, EFog };
    EWeatherEnum lastWeather;
    EWeatherEnum weather;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        gameTemp = new float[3];
        gameWind = new float[3];
        gameWindAngle = new float[3];
    }

    void Update()
    {
        if (!bIsTimeEnabled)
        {
            return;
        }
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

        float DeltaTimeUnit = (Time.deltaTime / 0.25f * 0.24f) * timeMultiplier;
        float AddedClockwork = DeltaTimeUnit + Clockwork;
        float NewDayTick = AddedClockwork / (60 * 24);
        float Remainder = AddedClockwork % (60.0f * 24.0f);
        Clockwork = Remainder;
        DayTick = NewDayTick;
        currentTimeOfDay = NewDayTick;

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }
    }

    void Clock()
    {
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
        if (bNewGenerationTemp)
        {
            lastTemp = Random.Range(0, 7);
            bNewGenerationTemp = false;
        }

        if (Minutes == 0)
        {
            if (!bHasGeneratedTemp)
            {
                for (int i = 0; i < 3; i++)
                {
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

                    generatedTemp = Random.Range(minGenTemp, maxGenTemp);

                    if ((generatedTemp - lastTemp) > 4)
                    {
                        generatedTemp = lastTemp + Random.Range(2.0f, 3.5f);
                    } else if ((lastTemp - generatedTemp) > 4)
                    {
                        generatedTemp = lastTemp - Random.Range(0.0f, 2.5f);
                    }

                    if (Hours <= 13 && Hours > 1)
                    {
                        generatedTemp = generatedTemp + Random.Range(1.5f, 3.0f);
                    } else if (Hours > 13 && Hours < 24)
                    {
                        generatedTemp = generatedTemp - Random.Range(0.2f, 1.5f);
                    }
                    gameTemp[i] = generatedTemp;
                }
                averageTemp = (gameTemp[0] + gameTemp[1] + gameTemp[2]) / 3;

                averageTemp = averageTemp - (windStrength / 5);

                if (averageTemp < -273.0f)
                {
                    averageTemp = -273.0f;
                }
                lastTemp = averageTemp;

                if (bIsTempFahrenheit)
                {
                    averageTemp = (averageTemp * (9 / 5)) + 32;
                }

                bHasGeneratedTemp = true;
            }
        }
        else
        {
            bHasGeneratedTemp = false;
        }
        temperature = averageTemp;
    }

    void WindStrength()
    {
        if (bNewGenerationWind)
        {
            lastWind = Random.Range(0, 7);
            bNewGenerationWind = false;
        }

        if ((Hours == 0 && Minutes == 0) ||
            (Hours == 6 && Minutes == 0) ||
            (Hours == 12 && Minutes == 0) ||
            (Hours == 18 && Minutes == 0))
        {
            if (!bHasGeneratedWind)
            {
                for (int i = 0; i < 3; i++)
                {
                    generatedWind = Random.Range(-3.0f, 3.0f);

                    if ((generatedWind - lastWind) > 3)
                    {
                        generatedWind = lastWind + Random.Range(0.0f, 3.0f);
                    } else if ((lastWind - generatedWind) < 3)
                    {
                        generatedWind = lastWind - Random.Range(0.0f, 3.0f);
                    }

                    gameWind[i] = generatedWind;
                }

                averageWind = (gameWind[0] + gameWind[1] + gameWind[2]) / 3;

                if (averageWind > 2.5f)
                {
                    averageWind = averageWind - Random.Range(0.3f, 0.9f);
                } else if (averageWind < 0)
                {
                    averageWind = Random.Range(0.3f, 1.3f);
                }
                lastWind = averageWind;
                averageWind = averageWind * windStrengthMultiplier;
                bHasGeneratedWind = true;
            }
        }
        else
        {
            bHasGeneratedWind = false;
        }
        windStrength = averageWind;
    }

    void WindAngle() {
        if (bNewGenerationWindAngle) {
            lastWindAngle = Random.Range(0.0f, 360.0f);
            bNewGenerationWindAngle = false;
        }

        if (Minutes == 0) { 
            if (!bHasGeneratedWindAngle) {
                for (int i = 0; i < 3; i++) { 
                    generatedWindAngle = Random.Range(0.0f, 360.0f);

                    
                    if ((generatedWindAngle - lastWindAngle) > 15) {
                        generatedWindAngle = lastWindAngle + Random.Range(0.0f, 10.0f);
                    }
                    else if ((lastWindAngle - generatedWindAngle) < 15) {
                        generatedWindAngle = lastWindAngle - Random.Range(0.0f, 10.0f);
                    }
                    gameWindAngle[i] = generatedWindAngle;
                }

                
                averageWindAngle = (gameWindAngle[0] + gameWindAngle[1] + gameWindAngle[2]) / 3;

                if (averageWindAngle < 0) {
                    averageWindAngle = 340 - averageWindAngle;
                }
                else if (averageWindAngle > 360) {
                    averageWindAngle = averageWindAngle - 340;
                }
                lastWindAngle = averageWindAngle;
                bHasGeneratedWindAngle = true; 
            }
        }
        else {
            bHasGeneratedWindAngle = false; 
        }

        windAngle = averageWindAngle; 
    }

    void Weather()
    {
        if (Minutes == 0) { 
            if (!bHasGeneratedWeather) {
                if (bNewGenerationWeather == true) {

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
                    bNewGenerationWeather = false;
                }
                else if (bNewGenerationWeather == false) {
                    if (Hours == 0) {
                    }
                    else if (lastWeather == EWeatherEnum.ESunny) {
                        if (seasonEnum == ESeasonEnum.ESpring) {
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

                lastWeather = weather;
                counter++;
                if (counter >= 1) {
                    bNewGenerationWeather = false;
                }
                bHasGeneratedWeather = true; //Makes sure generation only happens once
            }

        }
        else {
            bHasGeneratedWeather = false; //Resets the variable for the next hour
        }
        weatherEnum = weather;
    }
}