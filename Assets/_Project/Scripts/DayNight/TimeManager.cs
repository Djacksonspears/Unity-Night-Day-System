using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Scepter;

public class TimeManager : MonoBehaviour {

    [Header("UI")]
    [SerializeField] TextMeshProUGUI timeText;

    [Header("Lights")]
    [SerializeField] Light sun;
    [SerializeField] Light moon;

    [Header("Scene")]
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;

    [Header("Settings")]
    [SerializeField] TimeSettings timeSettings;
    
    //this is for testing keys
    const float MINUTES_BEFORE = 2f;


    ColorAdjustments colorAdjustments;
    TimeService service;

    void Start() {
        service = new TimeService(timeSettings);
        volume.profile.TryGet(out colorAdjustments);

        MessageWorld.Receive<SunriseMsg>(_ => Debug.Log("Sunrise"));
        MessageWorld.Receive<SunsetMsg>(_ => Debug.Log("Sunset"));
        MessageWorld.Receive<HourChangedMsg>(msg => Debug.Log($"Hour changed to: {msg.NewHour}"));
    }

    void Update() {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();

        if (Input.GetKeyDown(KeyCode.Space))     timeSettings.timeMultiplier *= 2;
        if (Input.GetKeyDown(KeyCode.LeftShift)) timeSettings.timeMultiplier /= 2;

        // Jump to just before sunset (dusk)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            service.SetTime(timeSettings.sunsetHour - (MINUTES_BEFORE / 60f));

        // Jump to just before sunrise (dawn)
        if (Input.GetKeyDown(KeyCode.Alpha2))
            service.SetTime(timeSettings.sunriseHour - (MINUTES_BEFORE / 60f));
    }

    void UpdateSkyBlend() {
        float dot   = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, timeSettings.lightIntensityCurve.Evaluate(dot));
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    void UpdateLightSettings() {
        float dot           = Vector3.Dot(sun.transform.forward, Vector3.down);
        float lightIntensity = timeSettings.lightIntensityCurve.Evaluate(dot);

        sun.intensity        = Mathf.Lerp(0,                           timeSettings.maxSunIntensity,  lightIntensity);
        sun.shadowStrength   = Mathf.Lerp(0,                           timeSettings.maxSunShadow,     lightIntensity);
        moon.intensity       = Mathf.Lerp(timeSettings.maxMoonIntensity, 0,                           lightIntensity);
        moon.shadowStrength  = Mathf.Lerp(timeSettings.maxMoonShadow,    0,                           lightIntensity);

        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(
            timeSettings.nightAmbientLight,
            timeSettings.dayAmbientLight,
            lightIntensity
        );
    }

    void RotateSun() {
        sun.transform.rotation = Quaternion.AngleAxis(service.CalculateSunAngle(), Vector3.right);
    }

    void UpdateTimeOfDay() {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null)
            timeText.text = service.CurrentTime.ToString("hh:mm");
    }
}