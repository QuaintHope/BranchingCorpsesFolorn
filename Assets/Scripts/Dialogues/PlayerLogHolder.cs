using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerLogHolder
/// Holds the player log.
/// </summary>
[CreateAssetMenu(fileName = "PlayerLog", menuName = "BranchingCorpses/Player Log")]
public class PlayerLogHolder : ScriptableObject
{
    /// <summary>
    /// log
    /// The text of the log.
    /// </summary>
    [SerializeField, Multiline(2)]
    public string log;

    /// <summary>
    /// nextLog
    /// The log coming after this log;
    /// </summary>
    [SerializeField]
    public PlayerLogHolder nextLog;

    /// <summary>
    /// nextButtonText
    /// The text that will display, describing the next action.
    /// </summary>
    [SerializeField]
    public string nextButtonText;

    /// <summary>
    /// soundEffectButtonOnClick
    /// The sound effect to play when clicking on the next action.
    /// </summary>
    [SerializeField]
    public AudioClip soundEffectButtonOnClick;

    /// <summary>
    /// pitchValue
    /// The value of the pitch with which the sound should be played.
    /// </summary>
    [SerializeField, Range(-3f, 3f)]
    public float pitchValue = 1;
}
