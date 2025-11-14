using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour {
    // Este método será llamado desde un Animation Event
    public void OnEndingFinished() {
        Debug.Log("[EndingController] Animación finalizada. Volviendo al menú...");
        SceneManager.LoadScene("MainMenu");
    }
}