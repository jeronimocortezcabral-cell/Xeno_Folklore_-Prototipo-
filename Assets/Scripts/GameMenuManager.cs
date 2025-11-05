using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour {
    void Start() {
      
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Jugar() {
        Debug.Log("Se hizo clic en 'Jugar'");
        SceneManager.LoadScene("MainScene");
    }

    public void SalirDelJuego() {
        Debug.Log("Saliendo del juego...");
#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si estás en una build, cierra la aplicación
        Application.Quit();
#endif
    }

    void Update() {
        
    }
}

