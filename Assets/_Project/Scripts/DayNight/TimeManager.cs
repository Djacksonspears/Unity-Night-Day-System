using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Scepter;

public class TimeManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timeText;
    
    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;
    
    [SerializeField] Color dayAmbientLight;
    [SerializeField] Color nightAmbientLight;
    [SerializeField] Volume volume;
    [SerializeField] Material skyboxMaterial;
    
    [SerializeField] TimeSettings timeSettings;
    
    ColorAdjustments colorAdjustments;
    TimeService service;

    void Start() {
        service = new TimeService(timeSettings);
        volume.profile.TryGet(out colorAdjustments);
        
        // Scepter handles the internal subscriptions and scene-load cleanup
        MessageWorld.Receive<SunriseMsg>(_ => Debug.Log("Sunrise"));
        MessageWorld.Receive<SunsetMsg>(_ => Debug.Log("Sunset"));
        MessageWorld.Receive<HourChangedMsg>(msg => Debug.Log($"Hour changed to: {msg.NewHour}"));
    }

    void Update() {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();
        
        // Debug Controls for Time Multiplier
        if (Input.GetKeyDown(KeyCode.Space)) {
            timeSettings.timeMultiplier *= 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            timeSettings.timeMultiplier /= 2;
        }
    }

    void UpdateSkyBlend() {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }
    
    void UpdateLightSettings() {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        float lightIntensity = lightIntensityCurve.Evaluate(dotProduct);
        
        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightIntensity);
        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightIntensity);
        
        if (colorAdjustments == null) return;
        colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensity);
    }

    void RotateSun() {
        float rotation = service.CalculateSunAngle();
        // Only rotates the Sun light object now; dial logic removed.
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);
    }

    void UpdateTimeOfDay() {
        service.UpdateTime(Time.deltaTime);
        if (timeText != null) {
            timeText.text = service.CurrentTime.ToString("hh:mm");
        }
    }
}