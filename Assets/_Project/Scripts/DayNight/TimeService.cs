using System;
using Scepter;

public class TimeService {
    readonly TimeSettings settings;
    DateTime currentTime;
    readonly TimeSpan sunriseTime;
    readonly TimeSpan sunsetTime;

    public DateTime CurrentTime => currentTime;
    
    bool wasDayTime;
    int lastHour;

    public TimeService(TimeSettings settings) {
        this.settings = settings;
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
        sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
        sunsetTime = TimeSpan.FromHours(settings.sunsetHour);
        
        wasDayTime = IsDayTime();
        lastHour = currentTime.Hour;
    }

    public void UpdateTime(float deltaTime) {
        currentTime = currentTime.AddSeconds(deltaTime * settings.timeMultiplier);
        
        // State change detection
        bool isDay = IsDayTime();
        if (isDay != wasDayTime) {
            if (isDay) new SunriseMsg().Send();
            else new SunsetMsg().Send();
            wasDayTime = isDay;
        }

        if (currentTime.Hour != lastHour) {
            lastHour = currentTime.Hour;
            new HourChangedMsg { NewHour = lastHour }.Send();
        }
    }
    
    public float CalculateSunAngle() {
        bool isDay = IsDayTime();
        float startDegree = isDay ? 0 : 180;
        TimeSpan start = isDay ? sunriseTime : sunsetTime;
        TimeSpan end = isDay ? sunsetTime : sunriseTime;
        
        TimeSpan totalTime = CalculateDifference(start, end);
        TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        return UnityEngine.Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
    }

    bool IsDayTime() => currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

    TimeSpan CalculateDifference(TimeSpan from, TimeSpan to) {
        TimeSpan difference = to - from;
        return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
    }
}