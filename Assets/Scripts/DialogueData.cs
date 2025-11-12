using UnityEngine;

[System.Serializable]
public class DialogueData {
    public string npcName;
    [TextArea(2, 6)]
    public string[] lines;
}