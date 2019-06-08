using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region Singleton management
    // Static singleton instance
    private static GameManager _instance;

    // Static singleton property
    public static GameManager Instance
    {
        get
        {
            return _instance ?? (_instance = new GameObject("SingletonHolder").AddComponent<GameManager>());
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

    #region Events
    /// <summary>
    /// EndofGameEvent
    /// Event fired when the game is in its ending part.
    /// </summary>
    public UnityEvent EndOfGameEvent { get; private set; } = new UnityEvent();

    /// <summary>
    /// InvokeEndOfGameEvent
    /// Invokes the end of the game event.
    /// </summary>
    public void InvokeEndOfGameEvent()
    {
        EndOfGameEvent.Invoke();
    }
    #endregion

    #region Action Buttons
    /// <summary>
    /// _actionButtonsToDeactivate
    /// The action buttons to deactivate at the end of the game.
    /// </summary>
    [Header("Buttons Management")]
    [SerializeField]
    private List<Button> _actionButtonsToDeactivate = new List<Button>();

    /// <summary>
    /// _nextDialogueButton
    /// The button for the next dialogue.
    /// </summary>
    [SerializeField]
    private Button _nextDialogueButton;

    /// <summary>
    /// DeactivateButtons
    /// Deactivates all the buttons contained in the _actionButtonList.
    /// </summary>
    private void DeactivateButtons()
    {
        foreach (Button button in _actionButtonsToDeactivate)
        {
            button.interactable = false;
        }
    }
    #endregion

    #region Ending Sorting
    /// <summary>
    /// _heroEndingStartingDialogue
    /// The starting dialogue for the hero ending.
    /// </summary>
    [Header("Endings")]
    [SerializeField]
    private DialogueHolder _heroEndingStartingDialogue;

    /// <summary>
    /// _retributionEndingStartingDialogue
    /// The starting dialogue for the retribution ending.
    /// </summary>
    [SerializeField]
    private DialogueHolder _retributionEndingStartingDialogue;

    /// <summary>
    /// _losingEndingStartingDialogue
    /// The starting dialogue for the losing ending.
    /// </summary>
    [SerializeField]
    private DialogueHolder _losingEndingStartingDialogue;

    /// <summary>
    /// _losingPlayerLogHolder
    /// The player log to display when losing the game.
    /// </summary>
    [SerializeField]
    private PlayerLogHolder _losingPlayerLogHolder;

    /// <summary>
    /// SortEnding
    /// Redirects the game into the proper ending.
    /// </summary>
    private void SortEnding()
    {
        float struggleLevel = StruggleManager.Instance.struggleLevel;
        // Losing ending;
        if (struggleLevel <= 0)
        {
            DialogueManager.Instance.currentDialogue = _losingEndingStartingDialogue;
            PlayerLogsManager.Instance.currentPlayerLog = _losingPlayerLogHolder;
            PlayerLogsManager.Instance.DisplayCurrentLog();
        }
        // Retribution Ending;
        else if (struggleLevel < 20)
        {
            _audioSource.mute = true;
            DialogueManager.Instance.currentDialogue = _retributionEndingStartingDialogue;
        }
        // Hero Ending;
        else
        {
            _audioSource.mute = true;
            DialogueManager.Instance.currentDialogue = _heroEndingStartingDialogue;
        }
        _nextDialogueButton.gameObject.SetActive(true);
        DialogueManager.Instance.StopDialogueAuto();
        DialogueManager.Instance.DisplayCurrentDialogue();
    }
    #endregion

    #region Sound
    /// <summary>
    /// _audioSource
    /// The audio source of the scene.
    /// </summary>
    [Header("Sounds")]
    [SerializeField]
    private AudioSource _audioSource;

    /// <summary>
    /// _mainMenuClip
    /// The audio clip used in the main menu.
    /// </summary>
    [SerializeField]
    private AudioClip _mainMenuClip;

    /// <summary>
    /// _gameClip
    /// The audio clip used as the main music in the gameloop.
    /// </summary>
    [SerializeField]
    private AudioClip _gameClip;

    /// <summary>
    /// _changingMusicDurationEffect
    /// The duration of the fade in / fade out to change musics.
    /// </summary>
    [SerializeField, Range(0f, 5f)]
    private float _changingMusicDurationEffect;

    /// <summary>
    /// _changingMusicCoroutineHolder
    /// Holder of the changing music coroutine.
    /// </summary>
    private Coroutine _changingMusicCoroutineHolder;

    /// <summary>
    /// FadeOutCoroutine
    /// Makes a fade out effect on the given audio source
    /// </summary>
    /// <param name="audioSource">The audio source to apply the effect to</param>
    /// <param name="FadeTime">The duration of the effect</param>
    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    /// <summary>
    /// FadeInCoroutine
    /// Makes a fade in effect on the given audio source
    /// </summary>
    /// <param name="audioSource">The audio source to apply the effect to</param>
    /// <param name="FadeTime">The duration of the effect</param>
    private IEnumerator FadeInCoroutine(AudioSource audioSource, float FadeTime)
    {
        audioSource.Play();
        audioSource.volume = 0f;
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    /// <summary>
    /// SwitchMusicCoroutine
    /// Switched the music on the main audio source with a fade effect.
    /// </summary>
    /// <param name="clip">The new clip to play.</param>
    private IEnumerator SwitchMusicCoroutine(AudioClip clip)
    {
        StartCoroutine(FadeOutCoroutine(_audioSource, _changingMusicDurationEffect / 2));
        yield return new WaitUntil( () => !_audioSource.isPlaying );
        _audioSource.clip = clip;
        StartCoroutine(FadeInCoroutine(_audioSource, _changingMusicDurationEffect / 2));
        _changingMusicCoroutineHolder = null;
    }

    /// <summary>
    /// SwitchMusic
    /// Switched the music on the main audio source with a fade effect.
    /// </summary>
    /// <param name="clip">The new clip to play.</param>
    private void SwitchMusic(AudioClip clip)
    {
        if (_changingMusicCoroutineHolder == null)
            _changingMusicCoroutineHolder = StartCoroutine(SwitchMusicCoroutine(clip));
    }
    #endregion

    #region UI Management
    /// <summary>
    /// _cutScenePanel
    /// The panel holding the dialogues of the cut scene.
    /// </summary>
    [Header("UI")]
    [SerializeField]
    private GameObject _cutScenePanel;

    /// <summary>
    /// _mainMenuPanel
    /// The panel displaying the main menu.
    /// </summary>
    [SerializeField]
    private GameObject _mainMenuPanel;

    /// <summary>
    /// _mainGameLoopPanel
    /// The panel used to display the main gameloop UI.
    /// </summary>
    [SerializeField]
    private GameObject _mainGameLoopPanel;

    /// <summary>
    /// _endingScreen
    /// The ending screen of the game.
    /// </summary>
    [SerializeField]
    private GameObject _endingScreen;

    /// <summary>
    /// _fadingPanel
    /// This panel is used to make fade in and out between screens.
    /// </summary>
    [SerializeField]
    private Image _fadingPanel;

    /// <summary>
    /// _fadeDuration
    /// The duration between a complete fadeIn and fadeOut.
    /// </summary>
    [SerializeField, Range(0f, 5f)]
    private float _fadeDuration;

    /// <summary>
    /// _fadeCoroutineHolder
    /// Holds the coroutine making the fade effect.
    /// </summary>
    private Coroutine _fadeCoroutineHolder;

    /// <summary>
    /// FadeEffectCoroutine
    /// Coroutine taking care of the fade effect on the screen.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeEffectCoroutine()
    {
        Color startingColor = new Color(0f, 0f, 0f, 0f);
        Color endingColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        float timerEffect = 0f;
        float lerpValue = 0f;

        _fadingPanel.gameObject.SetActive(true);

        while (timerEffect < _fadeDuration)
        {
            yield return new WaitForEndOfFrame();
            timerEffect += Time.deltaTime;
            // This function f(x) = (T - abs(2x - T)) / T will be defined between 0 and T and get value between 0 and 1.
            lerpValue = (_fadeDuration - Mathf.Abs(2*timerEffect - _fadeDuration)) / _fadeDuration;
            _fadingPanel.color = Color.Lerp(startingColor, endingColor, lerpValue);
        }

        _fadingPanel.gameObject.SetActive(false);
        _fadeCoroutineHolder = null;
    }

    /// <summary>
    /// StartFadeEffect
    /// Start the fade effect if there's not one already made.
    /// </summary>
    public void StartFadeEffect()
    {
        if (_fadeCoroutineHolder == null)
            _fadeCoroutineHolder = StartCoroutine(FadeEffectCoroutine());
    }
    #endregion

    #region Game Section Management
    /// <summary>
    /// _startGameCoroutineHolder
    /// Holder of the start game coroutine
    /// </summary>
    private Coroutine _startGameCoroutineHolder;
    
    /// <summary>
    /// InitializeGame
    /// Starts all the needed settings for the core game.
    /// </summary>
    private IEnumerator InitializeGame()
    {
        StartFadeEffect();

        yield return new WaitForSeconds(_fadeDuration / 2);

        _cutScenePanel.SetActive(false);
        _mainGameLoopPanel.SetActive(true);

        StruggleManager.Instance.StartStruggleDecayCoroutine();
        DialogueManager.Instance.StartDialogueAuto();
    }

    /// <summary>
    /// StartGameCoroutine
    /// Launches the game starting by the cutscene.
    /// </summary>
    private IEnumerator StartGameCoroutine()
    {
        // Music Gestion
        SwitchMusic(_gameClip);

        //Fading Effect
        StartFadeEffect();
        yield return new WaitForSeconds(_fadeDuration / 2);

        // UI Gestion
        _mainMenuPanel.SetActive(false);

        // eventSubscription
        // EndOfGameEvent.AddListener(DeactivateButtons);
        EndOfGameEvent.AddListener(SortEnding);
        DialogueManager.Instance.CutSceneEnding.AddListener(delegate { StartCoroutine(InitializeGame()); } );

        // Disabling UI uneeded elements
        if (_nextDialogueButton != null)
            _nextDialogueButton.gameObject.SetActive(false);

        // Starting the cutscene.
        _cutScenePanel.SetActive(true);
        DialogueManager.Instance.StartCutScene();
    }

    /// <summary>
    /// StartGame
    /// Starts the main game with the cutscene.
    /// </summary>
    public void StartGame()
    {
        if (_startGameCoroutineHolder == null)
            _startGameCoroutineHolder = StartCoroutine(StartGameCoroutine());
    }

    /// <summary>
    /// EndOfGame
    /// Starts the end of the game process.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndOfGameCoroutine()
    {        
        StartFadeEffect();
        yield return new WaitForSeconds(_fadeDuration / 2);
        _mainGameLoopPanel.SetActive(false);
        _endingScreen.SetActive(true);
        yield return new WaitForSeconds(5f);
        StartFadeEffect();
        SwitchMusic(null);
        yield return new WaitForSeconds(_fadeDuration / 2);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// CheckForEndOfGame
    /// Checks for the end of the game based on dialogues.
    /// </summary>
    public void CheckForEndofGame()
    {
        if (DialogueManager.Instance.currentDialogue.NextDialogue == null)
        {
            StartCoroutine(EndOfGameCoroutine());
        }
    }
    #endregion
}