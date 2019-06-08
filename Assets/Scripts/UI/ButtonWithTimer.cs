using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ButtonWithTimer
/// Monobehaviour to give buttons a sliding background linked to a timer.
/// </summary>
public class ButtonWithTimer : MonoBehaviour
{
    /// <summary>
    /// _cooldownDuration
    /// The duration of the button's cooldown.
    /// </summary>
    [SerializeField]
    private float _cooldownDuration;

    /// <summary>
    /// _lastActivationTimestamp
    /// The last timestamp when the button has been used.
    /// </summary>
    private float _lastActivationTimestamp;

    /// <summary>
    /// _waitingBar
    /// The background of the button working as a timer displayer.
    /// </summary>
    [SerializeField]
    private Slider _waitingBar;

    /// <summary>
    /// _actionButton
    /// The actual button of the action
    /// </summary>
    [SerializeField]
    private Button _actionButton;

    /// <summary>
    /// _buttonCooldownUpdateCoroutineHolder
    /// Holder of the coroutine for the button slider updater.
    /// </summary>
    private Coroutine _buttonCooldownUpdateCoroutineHolder;

    /// <summary>
    /// ButtonCooldownUpdateCoroutine
    /// This coroutine will make the button usable only after the cooldown has passed.
    /// </summary>
    private IEnumerator ButtonCooldownUpdateCoroutine()
    {
        yield return new WaitUntil(() =>
            {
                float passedTime = Time.time - _lastActivationTimestamp;
                _waitingBar.value = passedTime;
                return passedTime> _cooldownDuration;
            }
        );
        _actionButton.interactable = true;
        _buttonCooldownUpdateCoroutineHolder = null;
    }

    /// <summary>
    /// OnButtonWithTimerClick
    /// This function should be called when the action button is clicked on.
    /// </summary>
    private void OnButtonWithTimerClick()
    {
        if(_buttonCooldownUpdateCoroutineHolder == null)
        {
            _lastActivationTimestamp = Time.time;
            _actionButton.interactable = false;
            _waitingBar.value = _waitingBar.minValue;
            StartCoroutine(ButtonCooldownUpdateCoroutine());
        }
    }

    /// <summary>
    /// Start
    /// Called shortly after the loading of the scene.
    /// </summary>
    private void Start()
    {
        // Putting the cooldown bar's standard values.
        if (_waitingBar != null)
        {
            _waitingBar.minValue = 0f;
            _waitingBar.maxValue = _cooldownDuration;
            _waitingBar.value = _cooldownDuration;
        }
        // Adding the button cooldown on the action.
        if (_actionButton)
        {
            _actionButton.onClick.AddListener(OnButtonWithTimerClick);
            GameManager.Instance.EndOfGameEvent.AddListener(delegate { _actionButton.interactable = false; });
        }
    }
}
