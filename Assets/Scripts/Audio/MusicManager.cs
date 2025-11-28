using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    // --- MÚSICAS DE ESCENARIO Y COMBATE ---
    [Header("Música de Escenario")]
    public AudioSource cityMusic;
    public AudioSource outsideMusic;

    [Header("Música de Combate")]
    [Tooltip("Música específica para la pelea contra el Chonchón.")]
    public AudioSource chonchonMusic;

    // --- LOBIZÓN: CONFIGURACIÓN ACTUALIZADA ---
    [Tooltip("Música específica para la pelea contra el Lobizón.")]
    public AudioSource werewolfMusic;
    [Tooltip("Volumen específico para la música del Lobizón (solución punto B).")]
    [SerializeField] private float werewolfMusicVolume = 0.7f; // Valor por defecto ajustado para que se escuche mejor
    // ----------------------------------------

    [Header("Música de Victoria")]
    [Tooltip("Música al derrotar a ambos jefes.")]
    public AudioSource finalVictoryMusic;

    [Header("Crossfade")]
    public float fadeDuration = 2f;
    public float volumen = 1f; // Volumen base para la mayoría de las pistas

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

    public void PlayCityMusic() => TryFade(cityMusic, volumen);
    public void PlayOutsideMusic() => TryFade(outsideMusic, volumen);

    public void PlayChonchonMusic() => TryFade(chonchonMusic, volumen);
    // MODIFICADO: Usa el volumen específico del Lobizón
    public void PlayWerewolfMusic() => TryFade(werewolfMusic, werewolfMusicVolume);
    // ----------------------------------------------------

    /// <summary>
    /// Llamado al derrotar un jefe. Transiciona a la música de Victoria.
    /// (SOLUCIÓN C)
    /// </summary>
    public void DefeatedBoss()
    {
        // Detiene la música actual de inmediato (Lobizón, Chonchón o Nivel)
        if (currentMusic != null)
        {
            currentMusic.Stop();
            currentMusic.volume = 0f;
        }

        // Detiene cualquier crossfade anterior para dar prioridad a la victoria
        StopAllCoroutines();

        defeatedBosses++;

        Debug.Log($"Jefes derrotados: {defeatedBosses} de {BOSSES_REQUIRED_FOR_VICTORY}. Iniciando secuencia de victoria.");

        // La corrutina PlayVictorySequence() se encargará de reproducir la música de victoria y volver al nivel
        StartCoroutine(PlayVictorySequence(finalVictoryMusic));
    }

    // ******************************************************************
    // CORRUTINA: MANEJA LA SECUENCIA DE MÚSICA DE VICTORIA (SIN CAMBIOS)
    // ******************************************************************
    private IEnumerator PlayVictorySequence(AudioSource victoryTrack)
    {
        // 1. Apagar la música de combate rápidamente (ya se hizo en DefeatedBoss para asegurar la parada del Lobizón)
        // Eliminado el fade out rápido de 0.5s porque ya se detuvo en DefeatedBoss()

        // 2. Reproducir la pista de victoria (SIN LOOP)
        if (victoryTrack != null && victoryTrack.clip != null)
        {
            currentMusic = victoryTrack;
            // Usa el volumen base para la música de victoria
            currentMusic.volume = volumen;
            currentMusic.loop = false;
            currentMusic.Play();

            // 3. Esperar la duración de la pista de victoria MENOS la duración del fade final.
            float victoryDuration = currentMusic.clip.length;
            float fadeOutStartDelay = Mathf.Max(0f, victoryDuration - fadeDuration);

            yield return new WaitForSeconds(fadeOutStartDelay);

            // **********************************************
            // FADE OUT SUAVE DE LA MÚSICA DE VICTORIA
            // **********************************************

            float timer = 0f;
            // Usar el volumen base para el inicio del fade
            float startVolume = volumen;

            while (timer < fadeDuration && currentMusic.isPlaying)
            {
                timer += Time.deltaTime;
                currentMusic.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
                yield return null;
            }

            // Asegurar que se detiene y reinicia volumen para el siguiente uso
            currentMusic.Stop();
            currentMusic.volume = 0f;
        }

        // 4. Volver a la música de nivel/exterior con Crossfade
        Debug.Log("Volviendo a la música exterior después de la secuencia de victoria.");
        PlayOutsideMusic(); // Ya sea victoria final o parcial, volvemos al Exterior.
    }
    // ******************************************************************

    // MODIFICADO: Añadido parámetro 'targetVolume'
    private void TryFade(AudioSource target, float targetVolume)
    {
        if (target == null)
        {
            Debug.LogWarning("Intentaste reproducir una música que no está asignada en el Inspector.");
            return;
        }

        // Si la música actual es la misma o la de victoria está sonando, abortar
        if (currentMusic == target || currentMusic == finalVictoryMusic && currentMusic.isPlaying) return;

        // Detener la corrutina de Crossfade anterior si existe para evitar conflictos
        StopAllCoroutines();

        // Si ya está sonando la misma pista pero se detuvo por salir de rango, reinicia el volumen
        if (currentMusic == target && !currentMusic.isPlaying)
        {
            currentMusic.Play();
            currentMusic.volume = targetVolume; // Usa el volumen específico
            return;
        }

        StartCoroutine(Crossfade(target, targetVolume));
    }

    // ---------------------------
    // CROSSFADE GENÉRICO (MODIFICADO para usar targetVolume)
    // ---------------------------
    private IEnumerator Crossfade(AudioSource newMusic, float targetVolume)
    {
        nextMusic = newMusic;
        nextMusic.volume = 0f;

        // Asegurar que la nueva música siempre está en loop (la de victoria ya no pasa por aquí)
        nextMusic.loop = true;

        nextMusic.Play();

        float timer = 0f;
        float startVolumeCurrent = (currentMusic != null) ? currentMusic.volume : 0f; // Volumen inicial de la música actual

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (currentMusic != null)
            {
                currentMusic.volume = Mathf.Lerp(startVolumeCurrent, 0f, t);
            }
            // MODIFICADO: Usa targetVolume en lugar de la variable 'volumen' global
            nextMusic.volume = Mathf.Lerp(0f, targetVolume, t);
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

        // Asegurar que el volumen final es el correcto
        currentMusic.volume = targetVolume;
        currentMusic.loop = true;
    }
}