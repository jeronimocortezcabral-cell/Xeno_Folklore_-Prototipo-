using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Música de escenario")]
    public AudioSource cityMusic;
    public AudioSource outsideMusic;

    [Header("Música de combate")]
    public AudioSource battleMusic;

    [Header("Música de victoria")]
    public AudioSource victoryMusic;

    [Header("Crossfade")]
    public float fadeDuration = 2f;
    public float volumen = 1f;

    private AudioSource currentMusic;
    private AudioSource nextMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentMusic = cityMusic;
        currentMusic.volume = volumen;
        currentMusic.Play();
    }

    // ---------------------------
    // LLAMADAS PÚBLICAS
    // ---------------------------
    public void PlayCityMusic() => TryFade(cityMusic);
    public void PlayOutsideMusic() => TryFade(outsideMusic);
    public void PlayBattleMusic() => TryFade(battleMusic);
    public void PlayVictoryMusic() => TryFade(victoryMusic);

    private void TryFade(AudioSource target)
    {
        if (currentMusic == target) return;
        StartCoroutine(Crossfade(target));
    }

    // ---------------------------
    // CROSSFADE GENÉRICO
    // ---------------------------
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
            currentMusic.volume = Mathf.Lerp(volumen, 0f, t);
            nextMusic.volume = Mathf.Lerp(0f, volumen, t);
            yield return null;
        }

        currentMusic.Stop();
        currentMusic.volume = 0f;

        currentMusic = nextMusic;
        nextMusic = null;
    }
}
