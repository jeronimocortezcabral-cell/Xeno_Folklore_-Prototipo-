using UnityEngine;
// IMPORTANTE: Esta librería es necesaria para las Luces 2D
using UnityEngine.Rendering.Universal;

public class LightingDayNightCycle : MonoBehaviour
{
    [Header("Referencias")]
    public Light2D globalLight; // Arrastra aquí tu "Global Light 2D"

    [Header("Duración (Minutos)")]
    public float dayDurationMinutes = 2f;
    public float nightDurationMinutes = 2f;

    [Header("Configuración del Ciclo")]
    [Tooltip("El color de la luz a lo largo del ciclo")]
    public Gradient lightGradient;

    [Tooltip("La intensidad de la luz a lo largo del ciclo")]
    public AnimationCurve intensityCurve;

    private float totalCycleDuration;
    private float currentTime;

    private void Start()
    {
        // Convertir minutos a segundos
        totalCycleDuration = (dayDurationMinutes + nightDurationMinutes) * 60f;
        currentTime = 0f;

        // Si no asignaste la luz manualmente, intenta buscarla
        if (globalLight == null)
        {
            globalLight = GameObject.FindObjectOfType<Light2D>();
            if (globalLight == null)
                Debug.LogError("No se encontró una Global Light 2D en la escena.");
        }
    }

    private void Update()
    {
        if (globalLight == null) return;

        // Actualizar tiempo
        currentTime += Time.deltaTime;
        if (currentTime >= totalCycleDuration)
        {
            currentTime = 0f; // Reiniciar ciclo
        }

        // Porcentaje del ciclo (0.0 a 1.0)
        float timePercent = currentTime / totalCycleDuration;

        // 1. Cambiar el COLOR de la luz según el gradiente
        globalLight.color = lightGradient.Evaluate(timePercent);

        // 2. Cambiar la INTENSIDAD de la luz según la curva
        globalLight.intensity = intensityCurve.Evaluate(timePercent);
    }
}