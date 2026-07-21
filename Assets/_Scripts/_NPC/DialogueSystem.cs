using UnityEngine;

[CreateAssetMenu(fileName = "NewNpcDialogue", menuName ="NPC Dialogue")]
public class DialogueSystem : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
}
