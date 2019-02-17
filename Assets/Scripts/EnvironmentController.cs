using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    [Header("References: ")]
    [SerializeField] private Light sun;

    //[Space(20)]
    [Header("Time: ")]
    [SerializeField] private bool bIsTimeEnabled = true;
    [SerializeField] private float timeMultiplier = 1f;
    [SerializeField] private int DaysInMonth = 30;
    [SerializeField] private int MonthsInYear = 4;

    private float sunInitialIntensity;
    [Range(0, 1)] [SerializeField] private float currentTimeOfDay = 0;

    [Space(10)]
    [SerializeField] private int Seconds = 0;
    [SerializeField] private int Minutes = 0;
    [SerializeField] private int Hours = 0;

    [Space(10)]
    [SerializeField] private int Day = 1;
    [SerializeField] private int Month = 1;
    [SerializeField] private int Year = 1;

    [SerializeField] private float Clockwork;
    private float DayTick;
    private float DayNightHours;
    private int dayTracker = 1;

    //[Space(20)]
    [Header("Temperature:")]
    [SerializeField] float tempMultiplier = 1;
    [SerializeField] bool bNewGenerationTemp = true;
    [SerializeField] bool bHasGeneratedTemp = false;
    [SerializeField] bool bIsTempFahrenheit = false;
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
        if (bIsTimeEnabled)
        {
            SetClockwork();
            Clock();
            Calendar();
            UpdateSun();
        }

        UpdateEnvironment();
    }

    void UpdateEnvironment()
    {
        print(Temperature());
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
                    //TODO Season Check
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
