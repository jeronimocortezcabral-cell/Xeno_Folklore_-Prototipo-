using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    [Header("Datos del diálogo")]
    public DialogueData dialogue; // o DialogueSO si usás ScriptableObject

    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;

        Debug.Log("DialogueTrigger: Player entered range of " + gameObject.name);
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;

        Debug.Log("DialogueTrigger: Player left range of " + gameObject.name);
        playerInRange = false;
    }

    private void Update() {
        if (!playerInRange) return;

        // Si ya hay un diálogo abierto, no abrir otro
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen)
            return;

        // Presionar B para iniciar diálogo
        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("DialogueTrigger: B pressed near " + gameObject.name);

            if (DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(dialogue);
            else
                Debug.LogWarning("DialogueTrigger: No DialogueManager found in scene!");
        }
    }
}
