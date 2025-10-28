using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activated)
        {
            collision.GetComponent<PlayerRespawn>().ReachedCheckPoint(transform.position);
            activated = true;
            Debug.Log("Checkpoint alcanzado en: " + transform.position);
        }
    }
}
