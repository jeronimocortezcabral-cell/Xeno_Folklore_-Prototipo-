using UnityEngine;

public class MusicZoneTrigger : MonoBehaviour
{
    [SerializeField] private bool isCityZone = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isCityZone)
                MusicManager.instance.PlayCityMusic();
            else
                MusicManager.instance.PlayOutsideMusic();
        }
    }
}
