using UnityEngine;

public class BridgeLayerZone : MonoBehaviour
{
    public bool isOnBridge = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.layer = LayerMask.NameToLayer(isOnBridge ? "BridgePass" : "Player");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}
