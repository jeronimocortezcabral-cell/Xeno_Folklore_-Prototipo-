using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {
    public void OnIntroFinished() {
        SceneManager.LoadScene("MainScene");
    }
}