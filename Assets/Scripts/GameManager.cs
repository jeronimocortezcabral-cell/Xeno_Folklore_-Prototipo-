using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Reiniciar() 
        {
        Debug.Log("se iso clik");
        SceneManager.LoadScene("MainScene");
    }
    public void CerrarJuego() {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        // Si estás en el editor, detiene el modo de juego
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // Si estás en una build, cierra la aplicación
    Application.Quit();
#endif
    }

  

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))

        {
           pauseMenu.SetActive(true);

        }
        
    }
}
