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

    // Cambiado: ahora carga IntroScene
    public void Jugar() {
        Debug.Log("Cargando Intro...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("IntroScene");
    }

    public void Reiniciar() {
        Debug.Log("Reiniciando...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    public void VolverAlMenuPrincipal() {
        Debug.Log("Volviendo al menú principal...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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