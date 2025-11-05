using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    void Start() {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused)
                Reanudar();
            else
                Pausar();
        }
    }

    // --- FUNCIONES DE CONTROL DE PAUSA ---
    public void Pausar() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Reanudar() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // --- FUNCIONES EXTRA DEL MENÚ ---
    public void Reiniciar() {
        Debug.Log("Reiniciando...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public void VolverAlMenuPrincipal() {
        Debug.Log("Volviendo al menú principal...");
        Time.timeScale = 1f; // Reanuda el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu"); // Cambia a la escena del menú principal
    }

    public void CerrarJuego() {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}