using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    [Header("References: ")]
    [SerializeField] private Light sun;

    [Header("Time: ")]
    [SerializeField] private bool bIsTimeEnabled = true;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private int DaysInMonth = 30;
    [SerializeField] private int MonthsInYear = 12;

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
    [SerializeField] ESeasonEnum SeasonEnum;

    private float DayTick;
    private float sunInitialIntensity;
    private float DayNightHours;

    [Header("Temperature:")]
    [SerializeField] float tempMultiplier = 1;
    [SerializeField] bool bIsTempFahrenheit = false;
    bool bNewGenerationTemp = true;
    bool bHasGeneratedTemp = false;
    float lastTemp = 0;
    float minGenTemp = 0;
    float maxGenTemp = 7;
    float generatedTemp = 5.0f;
    float[] gameTemp;
    float averageTemp;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        gameTemp = new float[3];
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

        UpdateSun();
        Temperature();
        Season();
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

        if(currentTimeOfDay >= 1)
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
        if (Day > DaysInMonth)
        {
            Day = 1;
            Month++;
        }
        if (Month > MonthsInYear)
        {
            Month = 1;
            Year++;
        }
    }

    ESeasonEnum Season()
    {
        if (Month == 12 || Month == 1 || Month == 2)
        {
            return ESeasonEnum.EWinter;
        }
        else if (Month == 3 || Month == 4 || Month == 5)
        {
            return ESeasonEnum.ESpring;
        }
        else if (Month == 6 || Month == 7 || Month == 8)
        {
            return ESeasonEnum.ESummer;
        }
        else if (Month == 9 || Month == 10 || Month == 11)
        {
            return ESeasonEnum.EAutumn;
        }
        else
        {
            print("Season::Setting season failed!");
            return ESeasonEnum.ENone;
        }
    }

    float Temperature()
    {
        if (bNewGenerationTemp)
        {
            lastTemp = Random.Range(0, 7);
            bNewGenerationTemp = false;
        }

        if(Minutes == 0)
        {
            if (!bHasGeneratedTemp)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (SeasonEnum == ESeasonEnum.EWinter)
                    {
                        maxGenTemp = 8.0f * tempMultiplier;
                        minGenTemp = -7.0f * tempMultiplier;
                    }
                    else if (SeasonEnum == ESeasonEnum.ESpring)
                    {
                        maxGenTemp = 12.0f * tempMultiplier;
                        minGenTemp = 2.0f * tempMultiplier;
                    }
                    else if (SeasonEnum == ESeasonEnum.ESummer)
                    {
                        maxGenTemp = 20.0f * tempMultiplier;
                        minGenTemp = 7.0f * tempMultiplier;
                    }
                    else if (SeasonEnum == ESeasonEnum.EAutumn)
                    {
                        maxGenTemp = 13.0f * tempMultiplier;
                        minGenTemp = -3.0f * tempMultiplier;
                    }
                    else
                    {
                        print("Temperature::Check for season failed!");
                    }

                    generatedTemp = Random.Range(minGenTemp, maxGenTemp);

                    if((generatedTemp - lastTemp) > 4)
                    {
                        generatedTemp = lastTemp + Random.Range(2.0f, 3.5f);
                    }else if ((lastTemp - generatedTemp) > 4)
                    {
                        generatedTemp = lastTemp - Random.Range(0.0f, 2.5f);
                    }

                    if(Hours <= 13 && Hours > 1)
                    {
                        generatedTemp = generatedTemp + Random.Range(1.5f, 3.0f);
                    }else if(Hours > 13 && Hours < 24)
                    {
                        generatedTemp = generatedTemp - Random.Range(0.2f, 1.5f);
                    }
                    gameTemp[i] = generatedTemp;
                }
                averageTemp = (gameTemp[0] + gameTemp[1] + gameTemp[2]) / 3;

                //averageTemp = averageTemp - windFloat / 5;

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
        return averageTemp;
    }
}
