using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {
    [Header("Nombres de escena (configurables)")]
    [SerializeField] private string introSceneName = "IntroScene";
    [SerializeField] private string mainSceneName = "MainScene";

    [Header("Referencia opcional al botón 'Jugar' (para desactivar tras clic)")]
    [SerializeField] private Button playButton;

    private bool clicked = false;

    void Start() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Si el botón está asignado, aseguramos que sea interactable al inicio
        if (playButton != null) playButton.interactable = true;
    }

    // Llamar desde el OnClick del botón en el Inspector
    public void Jugar() {
        if (clicked) {
            Debug.Log("[GameMenuManager] Jugar() ignorado: ya se hizo clic.");
            return;
        }
        clicked = true;

        Debug.Log($"[GameMenuManager] Se hizo clic en 'Jugar' -> cargando '{introSceneName}'");
        Time.timeScale = 1f;

        if (playButton != null) playButton.interactable = false;

        SceneManager.LoadScene(introSceneName);
    }

    public void SalirDelJuego() {
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update() {
        // vacio por ahora
    }
}
