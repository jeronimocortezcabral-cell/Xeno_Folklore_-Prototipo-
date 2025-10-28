using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 checkPointPosition;
    private PlayerHealth playerHealth;

    private void Start()
    {
        checkPointPosition = transform.position;
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void ReachedCheckPoint(Vector3 position)
    {
        checkPointPosition = position;
        Debug.Log("Checkpoint alcanzado en: " + position);
    }

    public void Respawn()
    {
        transform.position = checkPointPosition;
        playerHealth.ResetHealth();
        Debug.Log("Jugador respawneado en último checkpoint.");
    }
}
