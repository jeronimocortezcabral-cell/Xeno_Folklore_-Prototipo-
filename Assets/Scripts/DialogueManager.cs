using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject continueIcon; // icono 'presiona B'

    private Queue<string> linesQueue;
    private bool isOpen = false;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        linesQueue = new Queue<string>();
        dialogPanel.SetActive(false);
    }

    private void Update()
    {
        // Avanza con B si el diálogo está abierto
        if (isOpen && Input.GetKeyDown(KeyCode.B))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0) return;

        // Pausar/inhabilitar controles del jugador
        PlayerMovement Player = Object.FindFirstObjectByType<PlayerMovement>();
        if (Player) Player.SetInputEnabled(false);

        isOpen = true;
        dialogPanel.SetActive(true);
        nameText.text = data.npcName ?? "";

        linesQueue.Clear();
        foreach (var line in data.lines) linesQueue.Enqueue(line);

        continueIcon.SetActive(linesQueue.Count > 1);
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = linesQueue.Dequeue();
        StopAllCoroutines();
        // Si quieres efecto typing, sustituir por StartCoroutine(TypeLine(line));
        dialogText.text = line;
        continueIcon.SetActive(linesQueue.Count > 0);
    }

    private void EndDialogue()
    {
        dialogPanel.SetActive(false);
        isOpen = false;

        // Reactivar controles del jugador
        PlayerMovement Player = Object.FindFirstObjectByType<PlayerMovement>();
        if (Player) Player.SetInputEnabled(true);
    }

    // EJ: typing effect (opcional)
    private IEnumerator TypeLine(string line)
    {
        dialogText.text = "";
        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.01f);
        }
    }
}