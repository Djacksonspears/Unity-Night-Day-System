using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "TimeSettings", menuName = "TimeSettings")]
public class TimeSettings : ScriptableObject {

    [Header("Time")]
    public float timeMultiplier = 2000;
    public float startHour     = 12;
    public float sunriseHour   = 6;
    public float sunsetHour    = 18;

    [Header("Sun")]
    public float maxSunIntensity   = 1f;
    public float maxSunShadow      = 1f;

    [Header("Moon")]
    public float maxMoonIntensity  = 0.5f;
    public float maxMoonShadow     = 0.5f;

    [Header("Ambient / Post-Processing")]
    public Color dayAmbientLight   = Color.white;
    public Color nightAmbientLight = new Color(0.1f, 0.1f, 0.2f);

    [Header("Curves")]
    public AnimationCurve lightIntensityCurve = AnimationCurve.Linear(0, 0, 1, 1);
}
