using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerLogsManager : MonoBehaviour
{
    #region Singleton Management
    // Static singleton instance
    private static PlayerLogsManager _instance;

    // Static singleton property
    public static PlayerLogsManager Instance
    {
        get
        {
            return _instance ?? (_instance = new GameObject("PlayerLogsManager").AddComponent<PlayerLogsManager>());
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

    #region Player Log Management
    /// <summary>
    /// _logText
    /// The text where we're displaying the log.
    /// </summary>
    [SerializeField]
    private Text _logText;

    /// <summary>
    /// _actionButtonText
    /// The text display on the action button.
    /// </summary>
    [SerializeField]
    private Text _actionButtonText;

    /// <summary>
    /// currentPlayerLog
    /// Tue current log used by the player.
    /// </summary>
    public PlayerLogHolder currentPlayerLog;

    /// <summary>
    /// DisplayCurrentLog
    /// Display the current dialogue in the log box.
    /// </summary>
    public void DisplayCurrentLog()
    {
        if (_logText == null || currentPlayerLog == null || _actionButtonText == null)
            return;

        _logText.text = currentPlayerLog.log;
        _actionButtonText.text = currentPlayerLog.nextButtonText;
        PlaySoundEffect();
    }

    /// <summary>
    /// MoveToNextLog
    /// Moves to the next log, but does not display it.
    /// </summary>
    public void MoveToNextLog()
    {
        if (currentPlayerLog == null || currentPlayerLog.nextLog == null)
            return;

        currentPlayerLog = currentPlayerLog.nextLog;

        if (currentPlayerLog.nextLog == null)
            GameManager.Instance.InvokeEndOfGameEvent();
    }

    public void Start()
    {
        DisplayCurrentLog();
    }
    #endregion

    #region Sound Effects
    /// <summary>
    /// _soundEffectSource
    /// The source used to play sound effect.
    /// </summary>
    [SerializeField]
    private AudioSource _soundEffectSource;

    /// <summary>
    /// PlaySoundeffect
    /// If there's a sound effect associated with the log action, plays it.
    /// </summary>
    private void PlaySoundEffect()
    {
        if (_soundEffectSource == null || currentPlayerLog.soundEffectButtonOnClick == null)
            return;

        _soundEffectSource.clip = currentPlayerLog.soundEffectButtonOnClick;
        _soundEffectSource.pitch = currentPlayerLog.pitchValue;
        _soundEffectSource.Play();
    }
    #endregion
}
