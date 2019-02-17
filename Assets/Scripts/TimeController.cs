using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Light sun;
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

    void Start()
    {
        sunInitialIntensity = sun.intensity;
    }

    void Update()
    {
        SetClockwork();
        Clock();
        Calendar();

        UpdateSun();
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
}
