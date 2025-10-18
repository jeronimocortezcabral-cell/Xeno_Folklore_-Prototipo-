using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Audio Sources")]
    public AudioSource cityMusic;
    public AudioSource outsideMusic;

    [Header("Configuración de crossfade")]
    public float fadeDuration = 2f; // segundos para transicionar

    private AudioSource currentMusic;
    private AudioSource nextMusic;
    public float Volumen;


    private void Awake()
    {
        // Singleton para que persista entre escenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Arranca con música de ciudad
        currentMusic = cityMusic;
        currentMusic.volume = Volumen;
        currentMusic.Play();
    }

    public void PlayCityMusic()
    {
        if (currentMusic == cityMusic) return;
        StartCoroutine(Crossfade(cityMusic));
    }

    public void PlayOutsideMusic()
    {
        if (currentMusic == outsideMusic) return;
        StartCoroutine(Crossfade(outsideMusic));
    }

    private IEnumerator Crossfade(AudioSource newMusic)
    {
        nextMusic = newMusic;
        nextMusic.volume = 0f;
        nextMusic.Play();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            currentMusic.volume = Mathf.Lerp(Volumen, 0f, t);
            nextMusic.volume = Mathf.Lerp(0f, Volumen, t);

            yield return null;
        }

        currentMusic.Stop();
        currentMusic.volume = 0f;

        currentMusic = nextMusic;
        nextMusic = null;
    }
}
