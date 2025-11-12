using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour {
    public DialogueData dialogue;
    private bool playerInRange = false;

    private void Reset() {
        // asegurar que el collider sea trigger por defecto al añadir el script
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playerInRange = false;
        }
    }

    private void Update() {
        if (playerInRange && Input.GetKeyDown(KeyCode.B)) {
            // Si ya hay un diálogo abierto y es este NPC quien lo tiene, avanço se hace en DialogueManager.
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsOpen) {
                DialogueManager.Instance.StartDialogue(dialogue);
            }
            // Si DialogueManager ya está abierto, la misma tecla B hará ShowNextLine (controlado por DialogueManager).
        }
    }
}