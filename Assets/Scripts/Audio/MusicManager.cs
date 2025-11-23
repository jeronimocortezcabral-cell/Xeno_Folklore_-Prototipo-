using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Música de Escenario")]
    public AudioSource cityMusic;
    public AudioSource outsideMusic;

    // --- MÚSICAS DE COMBATE ESPECÍFICAS ---
    [Header("Música de Combate")]
    [Tooltip("Música específica para la pelea contra el Chonchón.")]
    public AudioSource chonchonMusic;
    [Tooltip("Música específica para la pelea contra el Lobizón.")]
    public AudioSource werewolfMusic;
    // ----------------------------------------

    [Header("Música de Victoria")]
    [Tooltip("Música al derrotar a ambos jefes.")]
    public AudioSource finalVictoryMusic;

    [Header("Crossfade")]
    public float fadeDuration = 2f;
    public float volumen = 1f;

    [Header("Progreso del Juego")]
    private int defeatedBosses = 0;
    private const int BOSSES_REQUIRED_FOR_VICTORY = 2; // Chonchón + Lobizón

    private AudioSource currentMusic;
    private AudioSource nextMusic;

    private void Awake()
    {
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
        // Inicialización de la música de escenario
        currentMusic = cityMusic;
        if (currentMusic != null)
        {
            currentMusic.volume = volumen;
            currentMusic.Play();
            currentMusic.loop = true; // Asegura que las músicas de escenario hacen loop
        }
    }

    // ---------------------------
    // LLAMADAS PÚBLICAS
    // ---------------------------

    public void PlayCityMusic() => TryFade(cityMusic);
    public void PlayOutsideMusic() => TryFade(outsideMusic);

    // NUEVAS LLAMADAS ESPECÍFICAS PARA CADA JEFE
    public void PlayChonchonMusic() => TryFade(chonchonMusic);
    public void PlayWerewolfMusic() => TryFade(werewolfMusic);
    // ----------------------------------------------------

    /// <summary>
    /// Llamado al derrotar un jefe. Transiciona a la música de Victoria.
    /// </summary>
    public void DefeatedBoss()
    {
        // Detiene cualquier crossfade anterior para dar prioridad a la victoria
        StopAllCoroutines();

        defeatedBosses++;

        Debug.Log($"Jefes derrotados: {defeatedBosses} de {BOSSES_REQUIRED_FOR_VICTORY}. Iniciando secuencia de victoria.");
        StartCoroutine(PlayVictorySequence(finalVictoryMusic));
    }

    // ******************************************************************
    // CORRUTINA: MANEJA LA SECUENCIA DE MÚSICA DE VICTORIA (MODIFICADA)
    // ******************************************************************
    private IEnumerator PlayVictorySequence(AudioSource victoryTrack)
    {
        // 1. Apagar la música de combate rápidamente (Fade out)
        if (currentMusic != null)
        {
            float startVolume = currentMusic.volume;
            float timer = 0f;
            while (timer < 0.5f) // Fade out rápido
            {
                timer += Time.deltaTime;
                currentMusic.volume = Mathf.Lerp(startVolume, 0f, timer / 0.5f);
                yield return null;
            }
            currentMusic.Stop();
        }

        // 2. Reproducir la pista de victoria (SIN LOOP)
        if (victoryTrack != null && victoryTrack.clip != null)
        {
            currentMusic = victoryTrack;
            currentMusic.volume = volumen;
            currentMusic.loop = false;
            currentMusic.Play();

            // 3. Esperar la duración de la pista de victoria MENOS la duración del fade final.
            float victoryDuration = currentMusic.clip.length;
            float fadeOutStartDelay = Mathf.Max(0f, victoryDuration - fadeDuration);

            yield return new WaitForSeconds(fadeOutStartDelay);

            // **********************************************
            // NUEVO: FADE OUT SUAVE DE LA MÚSICA DE VICTORIA
            // **********************************************

            float timer = 0f;
            // Reducir el volumen mientras el tiempo restante (fadeDuration) lo permita
            while (timer < fadeDuration && currentMusic.isPlaying)
            {
                timer += Time.deltaTime;
                currentMusic.volume = Mathf.Lerp(volumen, 0f, timer / fadeDuration);
                yield return null;
            }

            // Asegurar que se detiene y reinicia volumen para el siguiente uso
            currentMusic.Stop();
            currentMusic.volume = 0f;
            // **********************************************
        }

        // 4. Volver a la música de nivel/exterior con Crossfade
        if (defeatedBosses >= BOSSES_REQUIRED_FOR_VICTORY)
        {
            Debug.Log("Volviendo a la música exterior después de la victoria final.");
            PlayOutsideMusic();
        }
        else
        {
            Debug.Log("Volviendo a la música exterior después de la victoria parcial.");
            PlayOutsideMusic();
        }
    }
    // ******************************************************************


    private void TryFade(AudioSource target)
    {
        if (target == null)
        {
            Debug.LogWarning("Intentaste reproducir una música que no está asignada en el Inspector.");
            return;
        }

        // Si la música actual es la misma o la de victoria está sonando, abortar
        if (currentMusic == target || currentMusic == finalVictoryMusic && currentMusic.isPlaying) return;

        // Detener la corrutina de Crossfade anterior si existe para evitar conflictos
        // NOTA: Esto detiene la corrutina PlayVictorySequence() si está en marcha.
        StopAllCoroutines();

        // Si ya está sonando la misma pista pero se detuvo por salir de rango, reinicia el volumen
        if (currentMusic == target && !currentMusic.isPlaying)
        {
            currentMusic.Play();
            currentMusic.volume = volumen;
            return;
        }

        StartCoroutine(Crossfade(target));
    }

    // ---------------------------
    // CROSSFADE GENÉRICO (Ligeramente ajustado para la limpieza)
    // ---------------------------
    private IEnumerator Crossfade(AudioSource newMusic)
    {
        nextMusic = newMusic;
        nextMusic.volume = 0f;

        // Asegurar que la nueva música siempre está en loop (la de victoria ya no pasa por aquí)
        nextMusic.loop = true;

        nextMusic.Play();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (currentMusic != null)
            {
                currentMusic.volume = Mathf.Lerp(volumen, 0f, t);
            }
            nextMusic.volume = Mathf.Lerp(0f, volumen, t);
            yield return null;
        }

        // Finalizar la transición
        if (currentMusic != null)
        {
            currentMusic.Stop();
            currentMusic.volume = 0f;
        }

        currentMusic = nextMusic;
        nextMusic = null;

        // Asegurarse de que el loop se activa en la nueva música si es necesario (ej: música de jefe/nivel)
        currentMusic.loop = true;
    }
}