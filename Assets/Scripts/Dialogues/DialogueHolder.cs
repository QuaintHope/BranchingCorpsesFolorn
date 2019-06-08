using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "BranchingCorpses/Dialogue")]
public class DialogueHolder : ScriptableObject
{
    /// <summary>
    /// dialogueSpeaker
    /// The identity of the speaker person.
    /// </summary>
    [SerializeField]
    public string dialogueSpeaker;

    /// <summary>
    /// _dialogue
    /// The piece of dialogue to display on screen.
    /// </summary>
    [SerializeField, Multiline(5)]
    private string _dialogue;

    /// <summary>
    /// Dialogue
    /// Returns the dialogue.
    /// </summary>
    public string Dialogue
    {
        get { return _dialogue; }
        private set { _dialogue = value; }
    }

    /// <summary>
    /// SwordSprite
    /// The sprite that will be used as the sword portrait.
    /// </summary>
    [SerializeField]
    private Sprite _swordSprite;

    /// <summary>
    /// SwordSprite
    /// The sprite that will be used as the sword portrait.
    /// </summary>
    public Sprite SwordSprite
    {
        get { return _swordSprite; }
        set { _swordSprite = value; }
    }

    /// <summary>
    /// _nextDialogue
    /// The dialogue following the current dialogue.
    /// </summary>
    [SerializeField]
    private DialogueHolder _nextDialogue;

    /// <summary>
    /// NextDialogue
    /// Returns the next dialogue.
    /// </summary>
    public DialogueHolder NextDialogue
    {
        get { return _nextDialogue; }
        private set { _nextDialogue = value; }
    }
}
