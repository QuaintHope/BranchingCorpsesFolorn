using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

public class StruggleManager : MonoBehaviour {

    #region Singleton Management
    // Static singleton instance
    private static StruggleManager _instance;

    // Static singleton property
    public static StruggleManager Instance
    {
        get
        {
            return _instance ?? (_instance = new GameObject("StruggleManager").AddComponent<StruggleManager>());
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

    #region GameLogic
    /// <summary>
    /// struggleLevel
    /// The current struggle level of the player. 
    /// When it falls at 0, the end of the game should be raised.
    /// </summary>
    [Range(0, 100)]
    public float struggleLevel;

    /// <summary>
    /// struggleDecayPerSecondValue
    /// The amount of struggle the player every second.
    /// </summary>
    [SerializeField]
    [Range(0, 25)]
    private int _struggleDecayPerSecondValue;

    /// <summary>
    /// _struggleDecayingCoroutineHolder
    /// Holds the coroutine that decays the struggle level.
    /// </summary>
    private Coroutine _struggleDecayingCoroutineHolder;

    /// <summary>
    /// _struggleGainedPerClick
    /// The amount of struggle the player will win per click.
    /// </summary>
    [SerializeField, Range(0f, 8f)]
    private float _struggleGainedPerClick;

    /// <summary>
    /// StruggleDecayingCoroutine
    /// This coroutine is used to decay the struggle level every second. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator StruggleDecayingCoroutine()
    {
        for (; ; )
        {
            struggleLevel = Mathf.Max(struggleLevel - _struggleDecayPerSecondValue, 0);
            UpdateStruggleSlider();

            // Checks for the end of the game.
            if (struggleLevel <= 0)
            {
                GameManager.Instance.InvokeEndOfGameEvent();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// StartStruggleDecayCoroutine
    /// Staert the decaying of the struggle meters.
    /// </summary>
    public void StartStruggleDecayCoroutine()
    {
        if (_struggleDecayingCoroutineHolder == null)
            _struggleDecayingCoroutineHolder = StartCoroutine(StruggleDecayingCoroutine());
    }

    /// <summary>
    /// StopStruggleDecayCoroutine
    /// Stops the decaying of the struggle meters.
    /// </summary>
    public void StopStruggleDecayCoroutine()
    {
        if(_struggleDecayingCoroutineHolder != null)
        {
            StopCoroutine(_struggleDecayingCoroutineHolder);
            _struggleDecayingCoroutineHolder = null;
        }
    }

    /// <summary>
    /// GainStruggle
    /// Adds struggle to the player according to the amount given in the StruggleManager.
    /// </summary>
    public void GainStruggle()
    {
        struggleLevel = Mathf.Min(struggleLevel + _struggleGainedPerClick, 100);
        UpdateStruggleSlider();
    }

    #endregion

    #region UI
    /// <summary>
    /// _struggleSlider
    /// The slider that is used to represent the struggle level.
    /// </summary>
    [SerializeField]
    private Slider _struggleSlider;

    /// <summary>
    /// _lowStruggleColorLevel
    /// The color used on the slider when the player has a low struggle level.
    /// </summary>
    [SerializeField]
    private Color _lowStruggleLevelColor = Color.red;

    /// <summary>
    /// _hightStruggleLevelColor
    /// The color used on the slider when the player has a high struggle level.
    /// </summary>
    [SerializeField]
    private Color _highStruggleLevelColor = Color.blue;

    /// <summary>
    /// UpdateStruggleSlider
    /// Updates the value of the struggle slider and its color.
    /// </summary>
    private void UpdateStruggleSlider()
    {
        if(_struggleSlider != null)
        {
            _struggleSlider.value = struggleLevel;
            _struggleSlider.fillRect.GetComponent<Image>().color = Color.Lerp(_lowStruggleLevelColor, _highStruggleLevelColor, struggleLevel / _struggleSlider.maxValue);
        }
    }
    #endregion

    public void Start()
    {
        if(_struggleSlider != null)
        {
            _struggleSlider.maxValue = 100;
            _struggleSlider.minValue = 0;
        }
        GameManager.Instance.EndOfGameEvent.AddListener(StopStruggleDecayCoroutine);
    }
}
