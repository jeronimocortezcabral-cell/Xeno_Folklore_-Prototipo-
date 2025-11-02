using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerHealth playerHealth;  // referencia al jugador
    [SerializeField] private Image healthFill;           // la imagen del relleno (fill)

    [Header("Colores opcionales")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;

    private void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (playerHealth == null || healthFill == null) return;

        // Progreso de vida (0 a 1)
        float fillValue = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        healthFill.fillAmount = fillValue;

        // Cambiar color dinámicamente
        healthFill.color = Color.Lerp(lowHealthColor, fullHealthColor, fillValue);
    }
}
