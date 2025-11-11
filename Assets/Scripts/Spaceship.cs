using UnityEngine;
using UnityEngine.SceneManagement;

public class Spaceship : MonoBehaviour
{
    [SerializeField] private int requiredParts = 4;
    [SerializeField] private string sceneToLoad = "NextLevel";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
        if (inventory != null && inventory.HasEnoughKeys(requiredParts))
        {
            Debug.Log("¡Partes suficientes! Despegando...");
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("Te faltan partes de la nave para despegar.");
        }
    }
}
