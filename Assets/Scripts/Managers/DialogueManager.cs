using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    #region Singleton management
    // Static singleton instance
    private static DialogueManager _instance;

    // Static singleton property
    public static DialogueManager Instance
    {
        get
        {
            return _instance ?? (_instance = new GameObject("DialogueManager").AddComponent<DialogueManager>());
        }
        private set
        {
            _instance = value;
        }
    }

    /// <summary>
    /// Awake
    /// Called when the gameobject is awaken. 
    /// </summary>
    void Awake()
    {
        // Setting up the Singleton.
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
            Destroy(this);
    }
    #endregion

    #region UIElement
    /// <summary>
    /// _startingCutSceneText
    /// The text holder for the cutscene. 
    /// </summary>
    [Header("UI Elements")]
    [SerializeField]
    private Text _startingCutSceneText;

    /// <summary>
    /// DialogueTextBox
    /// The Ui Element holding the dialogues.
    /// </summary>
    [SerializeField]
    private Text _dialogueTextBoxGameplay;

    /// <summary>
    /// EnemyNameTextBox
    /// </summary>
    [SerializeField]
    private Text _enemyNameTextBox;

    /// <summary>
    /// _enemyImage
    /// The image of the enemy currently talking.
    /// </summary>
    [SerializeField]
    private Image _enemyImage;
    #endregion

    #region Dialogues
    /// <summary>
    /// CurrentDialogue
    /// The current dialogue being display.
    /// Don't change it outside the Dialogue Manager lightly.
    /// </summary>
    public DialogueHolder currentDialogue;

    /// <summary>
    /// _firstDialogueGameplay
    /// The first dialogue to be displayed in the game loop.
    /// </summary>
    [Header("Game Dialogues")]
    [SerializeField]
    private DialogueHolder _firstDialogueGameplay;

    /// <summary>
    /// timedDialogues
    /// Tells if the dialogues should be timed or if we're moving
    /// according to the player's action.
    /// </summary>
    [HideInInspector]
    public bool timedDialogues;

    /// <summary>
    /// _dialogueDuration
    /// The duration of each dialogue when on timedDialogues.
    /// </summary>
    [SerializeField]
    private float _dialogueDuration;
    
    /// <summary>
    /// _lastDialogueTimestamp
    /// The timestamp of the last displayed dialogue.
    /// </summary>
    private float _lastDialogueTimestamp;

    /// <summary>
    /// _dialogueAutoCoroutineHolder
    /// 
    /// </summary>
    private Coroutine _dialogueAutoCoroutineHolder;

    /// <summary>
    /// DisplayCurrentDialogue
    /// Display the current dialogue in the dialogue box.
    /// </summary>
    public void DisplayCurrentDialogue()
    {
        if (currentDialogue == null || _dialogueTextBoxGameplay == null || _enemyImage == null || _enemyNameTextBox == null)
            return;

        _dialogueTextBoxGameplay.text = currentDialogue.Dialogue;
        _enemyNameTextBox.text = currentDialogue.dialogueSpeaker;

        _enemyImage.sprite = currentDialogue.SwordSprite;
        if(_enemyImage.sprite == null)
            _enemyImage.color = new Color(1.0f, 1.0f, 1.0f, 0f);
        else
            _enemyImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// MoveToNextDialogue
    /// Moves to the next dialogue, but does not display it.
    /// </summary>
    public void MoveToNextDialogue()
    {
        if (currentDialogue == null || currentDialogue.NextDialogue == null)
            return;

        currentDialogue = currentDialogue.NextDialogue;
    }

    /// <summary>
    /// DialogueChangingAutoCoroutine
    /// Starts the auto changing dialogue coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator DialogueChangingAutoCoroutine()
    {
        for(; ;)
        {
            _lastDialogueTimestamp = Time.time;
            DisplayCurrentDialogue();
            yield return new WaitUntil(() => Time.time - _lastDialogueTimestamp > _dialogueDuration);
            MoveToNextDialogue();
        }
    }

    /// <summary>
    /// StartDialogueAuto
    /// If the coroutine of dialogue auto has not been casted, starts it.
    /// </summary>
    public void StartDialogueAuto()
    {
        timedDialogues = true;
        if (_dialogueAutoCoroutineHolder == null)
            _dialogueAutoCoroutineHolder = StartCoroutine(DialogueChangingAutoCoroutine());
    }

    /// <summary>
    /// StopDialogueAuto
    /// If the coroutine of dialogue auto has been casted, stops it.
    /// </summary>
    public void StopDialogueAuto()
    {
        timedDialogues = false;
        if (_dialogueAutoCoroutineHolder != null)
        {
            StopCoroutine(_dialogueAutoCoroutineHolder);
            _dialogueAutoCoroutineHolder = null;
        }
    }
    #endregion

    #region Starting Cutscene
    /// <summary>
    /// _cutSceneFirstDialogue
    /// The first dialogue to display in the cut scene.
    /// </summary>
    [Header("Cutscene")]
    [SerializeField]
    private DialogueHolder _cutSceneFirstDialogue;

    /// <summary>
    /// _cutsceneDialogueDuration
    /// The duration of the dialogues in the cutscene.
    /// </summary>
    [SerializeField]
    private float _cutsceneDialogueDuration;

    /// <summary>
    /// _cutScenePlayed
    /// Has the starting cutscene been played?
    /// </summary>
    private bool _cutscenePlayed = false;

    /// <summary>
    /// CutSceneEnding
    /// The event that will be fired when the cut scene ends.
    /// </summary>
    public UnityEvent CutSceneEnding { get; set; } = new UnityEvent();

    /// <summary>
    /// _cutSceneCoroutine
    /// The coroutine that contains the cutscene coroutine.
    /// </summary>
    private Coroutine _cutSceneCoroutine;

    /// <summary>
    /// StartCutSceneCoroutine
    /// Start the coroutine that take care of the cutscene.
    /// </summary>
    private IEnumerator StartCutSceneCoroutine()
    {
        currentDialogue = _cutSceneFirstDialogue;

        while(currentDialogue != null)
        {
            _lastDialogueTimestamp = Time.time;
            _startingCutSceneText.text = currentDialogue.Dialogue;
            currentDialogue = currentDialogue.NextDialogue;
            yield return new WaitUntil(() => Time.time - _lastDialogueTimestamp > _cutsceneDialogueDuration);
        }

        EndCutScene();
    }

    /// <summary>
    /// EndCutScene
    /// Ends the cutscene at the start of the game.
    /// </summary>
    private void EndCutScene()
    {
        if(!_cutscenePlayed)
        {
            if (_cutSceneCoroutine != null)
            {
                StopCoroutine(_cutSceneCoroutine);
                _cutSceneCoroutine = null;
            }
            currentDialogue = _firstDialogueGameplay;
            _cutscenePlayed = true;
            CutSceneEnding.Invoke();
        }
    }

    /// <summary>
    /// StartCutScene
    /// Starts the cut scene of the beginning.
    /// </summary>
    public void StartCutScene()
    {
        if (_cutSceneCoroutine == null)
            _cutSceneCoroutine = StartCoroutine(StartCutSceneCoroutine());
    }
    #endregion

    /// <summary>
    /// Update
    /// Used here to manage the premature ending of the cutscene.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !_cutscenePlayed)
            EndCutScene();
    }
}
