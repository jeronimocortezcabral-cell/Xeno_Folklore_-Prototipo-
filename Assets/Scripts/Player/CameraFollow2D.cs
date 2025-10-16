using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;   // el jugador
    public float smoothSpeed = 5f;  // velocidad de suavizado
    public Vector3 offset;     // desplazamiento opcional

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
