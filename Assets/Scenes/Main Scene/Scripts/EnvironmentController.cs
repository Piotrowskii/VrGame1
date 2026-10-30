using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [Header("Skybox")]
    [SerializeField] private Material skyboxMaterial;

    [Header("Night Skybox Settings")]
    [SerializeField] private bool nightSunEnabled = false;
    [SerializeField] private float nightSunSize = 0f;
    [SerializeField] private float nightAtmosphereThickness = 0.41f;
    [SerializeField] private Color nightSkyTint = Color.black;
    [SerializeField] private Color nightGroundColor = new Color32(72, 72, 72, 255);
    [SerializeField] private float nightExposure = 0.03f;

    [Header("Day Skybox Settings")]
    [SerializeField] private bool daySunEnabled = true;
    [SerializeField] private float daySunSize = 0.1f;
    [SerializeField] private float daySunSizeConvergence = 5f;
    [SerializeField] private float dayAtmosphereThickness = 0.59f;
    [SerializeField] private Color daySkyTint = new Color32(0, 136, 255, 255);
    [SerializeField] private Color dayGroundColor = new Color32(0, 98, 170, 255);
    [SerializeField] private float dayExposure = 8f;

    [Header("Environment Lighting Intensity")]
    [SerializeField] private float dayLightingIntensity = 1f;
    [SerializeField] private float nightLightingIntensity = 0.1f;

    [Header("Environment Reflections Intensity")]
    [SerializeField] private float dayReflectionIntensity = 1f;
    [SerializeField] private float nightReflectionIntensity = 0.25f;

    [Header("Lighting")]
    [SerializeField] private Light dayLight;
    [SerializeField] private Light nightLight;

    private static readonly int SunSizeID = Shader.PropertyToID("_SunSize");

    private static readonly int SunSizeConvergenceID = Shader.PropertyToID("_SunSizeConvergence");

    private static readonly int AtmosphereThicknessID = Shader.PropertyToID("_AtmosphereThickness");

    private static readonly int SkyTintID = Shader.PropertyToID("_SkyTint");

    private static readonly int GroundColorID = Shader.PropertyToID("_GroundColor");

    private static readonly int ExposureID = Shader.PropertyToID("_Exposure");

    private void OnEnable()
    {
        GameManager.instance.OnTimeOfDayChange.AddListener(HandleTimeOfDayChange);
    }

    private void OnDisable()
    {
        GameManager.instance.OnTimeOfDayChange.RemoveListener(HandleTimeOfDayChange);
    }

    private void Start()
    {
        if (skyboxMaterial != null) RenderSettings.skybox = skyboxMaterial;
        SetEnvironment(GameManager.instance.CurrentTimeOfDay);
    }

    private void HandleTimeOfDayChange(TimeOfDay time)
    {
        SetEnvironment(time);
    }

    private void SetEnvironment(TimeOfDay time)
    {
        bool isDay = time == TimeOfDay.Day;

        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat(SunSizeID, isDay ? daySunSize : nightSunSize);

            skyboxMaterial.SetFloat(SunSizeConvergenceID, isDay ? daySunSizeConvergence : 5f);

            skyboxMaterial.SetFloat(AtmosphereThicknessID, isDay ? dayAtmosphereThickness : nightAtmosphereThickness);

            skyboxMaterial.SetColor(SkyTintID, isDay ? daySkyTint : nightSkyTint);

            skyboxMaterial.SetColor(GroundColorID, isDay ? dayGroundColor : nightGroundColor);

            skyboxMaterial.SetFloat(ExposureID, isDay ? dayExposure : nightExposure);
        }

        RenderSettings.skybox = skyboxMaterial;

        RenderSettings.ambientIntensity = isDay ? dayLightingIntensity: nightLightingIntensity;

        RenderSettings.reflectionIntensity = isDay ? dayReflectionIntensity : nightReflectionIntensity;

        if (dayLight != null) dayLight.enabled = isDay;

        if (nightLight != null) nightLight.enabled = !isDay;

        DynamicGI.UpdateEnvironment();
    }
}
