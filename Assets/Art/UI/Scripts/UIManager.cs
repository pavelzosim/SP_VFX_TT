using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteAlways]
public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class ProgressBarSlider
    {
        public int maxValue = 100;
        [Range(0, 1000)] public int currentValue = 0;
    }

    [System.Serializable]
    public class Sticker
    {
        [Range(0, 10)] public int stickerCount = 1;
    }

    [System.Serializable]
    public class TargetReferences
    {
        [Tooltip("Text component to display progress values")]
        public Text progressText;
        [Tooltip("Base material for the progress bar (will be instanced)")]
        public Material progressBarMaterial;
        [Tooltip("Image component representing the progress bar mask")]
        public Image targetMask;
        [Tooltip("Text component to display sticker count")]
        public Text stickerText;
        [Tooltip("Popup to show when progress reaches max")]
        public GameObject popupObject;
        [Tooltip("Container for debug elements")]
        public GameObject debugContainer;
        [Tooltip("Text component to display current time")]
        public Text timeText;
    }

    [Header("Slider Settings")]
    public ProgressBarSlider settings = new ProgressBarSlider();

    [Header("Sticker Settings")]
    public Sticker stickerSettings = new Sticker();

    [Header("Targets")]
    public TargetReferences targetReferences = new TargetReferences();

    [Header("Animation Settings")]
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    private bool _maxValueReached;

    [Header("Debug")]
    [Tooltip("Toggle visibility of progress bar elements")]
    public bool debugMode = true;

    [Header("Dynamic Object Disabler")]
    [Tooltip("List of objects to disable when maxValue is reached")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    private Material _instancedMaterial;
    private bool _lastDebugState;
    private float _lastFillAmount;
    private int _lastMaxValue;
    private Coroutine _timeCoroutine;

    void OnEnable()
    {
        InitializeProgressBar();
        _lastDebugState = debugMode;
        UpdateDebugVisibility();
        _lastMaxValue = settings.maxValue;

        if (Application.isPlaying)
        {
            _timeCoroutine = StartCoroutine(UpdateTimeCoroutine());
        }
    }

    void Update()
    {
        UpdateProgress();
        UpdateStickerText();
        CheckMaxValueReset();

        if (_lastDebugState != debugMode)
        {
            UpdateDebugVisibility();
            _lastDebugState = debugMode;
        }
    }

    void OnDisable()
    {
        CleanupMaterial();
        if (_timeCoroutine != null)
        {
            StopCoroutine(_timeCoroutine);
            _timeCoroutine = null;
        }
    }

    private void InitializeProgressBar()
    {
        if (targetReferences.targetMask == null)
        {
            Debug.LogWarning($"[{nameof(UIManager)}] No targetMask assigned.", this);
            return;
        }

        if (targetReferences.progressBarMaterial != null && _instancedMaterial == null)
        {
            _instancedMaterial = Instantiate(targetReferences.progressBarMaterial);
            _instancedMaterial.name = $"{targetReferences.progressBarMaterial.name} (Instance)";
            targetReferences.targetMask.material = _instancedMaterial;
        }
    }

    private void UpdateProgress()
    {
        if (targetReferences.targetMask == null || settings == null || settings.maxValue <= 0)
            return;

        settings.currentValue = Mathf.Clamp(settings.currentValue, 0, settings.maxValue);
        float fillAmount = (float)settings.currentValue / settings.maxValue;

        if (settings.currentValue == settings.maxValue)
        {
            if (targetReferences.popupObject != null && !targetReferences.popupObject.activeSelf)
                targetReferences.popupObject.SetActive(true);

            DisableObjectsOnMaxValue();
        }
        else
        {
            EnableObjectsOnBelowMaxValue();
        }

        if (settings.currentValue == settings.maxValue)
        {
            if (!_maxValueReached)
            {
                PanelFadeIn();
                _maxValueReached = true;
            }
        }
        else
        {
            if (_maxValueReached)
            {
                PanelFadeOut();
                _maxValueReached = false;
            }
        }

        if (!Mathf.Approximately(fillAmount, _lastFillAmount))
        {
            _lastFillAmount = fillAmount;
            targetReferences.targetMask.fillAmount = fillAmount;
            UpdateMaterial(fillAmount);
        }

        UpdateProgressText();
    }

    private void UpdateMaterial(float fill)
    {
        if (_instancedMaterial == null) return;

        var mat = targetReferences.targetMask.materialForRendering;
        if (mat != null)
        {
            mat.SetFloat("_Fill", fill);
        }
    }

    private void UpdateProgressText()
    {
        if (targetReferences.progressText != null)
            targetReferences.progressText.text = $"{settings.currentValue}/{settings.maxValue}";
    }

    private void UpdateStickerText()
    {
        if (targetReferences.stickerText != null)
            targetReferences.stickerText.text = $"{stickerSettings.stickerCount}";
    }

    private void UpdateDebugVisibility()
    {
        if (targetReferences.debugContainer != null)
        {
            targetReferences.debugContainer.SetActive(debugMode);
        }
    }

    private IEnumerator UpdateTimeCoroutine()
    {
        while (true)
        {
            if (targetReferences.timeText != null)
                targetReferences.timeText.text = DateTime.Now.ToString("HH:mm:ss");
            yield return new WaitForSeconds(1f);
        }
    }

    private void CheckMaxValueReset()
    {
        if (settings.maxValue != _lastMaxValue)
        {
            settings.currentValue = 0;
            _lastMaxValue = settings.maxValue;
            _maxValueReached = false;

            if (targetReferences.popupObject != null)
                targetReferences.popupObject.SetActive(false);

            EnableObjectsOnBelowMaxValue();
            UpdateProgress();
        }
    }

    private void CleanupMaterial()
    {
        if (_instancedMaterial != null)
            DestroyImmediate(_instancedMaterial);
    }

    private void DisableObjectsOnMaxValue()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    private void EnableObjectsOnBelowMaxValue()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    public void SetEmpty()
    {
        if (settings == null)
            return;

        settings.currentValue = 0;
        UpdateProgress();
        UpdateProgressText();
        UpdateStickerText();
    }

    public void SetFull()
    {
        if (settings == null || settings.maxValue <= 0)
            return;

        settings.currentValue = settings.maxValue;
        UpdateProgress();
        UpdateProgressText();
        UpdateStickerText();
    }
public void SetRandom()
{
    if (settings == null || settings.maxValue <= 0)
        return;

    // Explicitly use UnityEngine.Random
    settings.currentValue = UnityEngine.Random.Range(
        Mathf.RoundToInt(settings.maxValue * 0.1f),
        settings.maxValue + 1
    );
    UpdateProgress();
    UpdateProgressText();
    UpdateStickerText();
}

    public void PanelFadeIn()
    {
        if (canvasGroup != null) canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.OutElastic);
        canvasGroup.DOFade(1, fadeTime);
    }

    public void PanelFadeOut()
    {
        canvasGroup.alpha = 1f;
        rectTransform.localPosition = Vector3.zero;
        rectTransform.DOAnchorPos(new Vector2(0f, -1000f), fadeTime)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() =>
            {
                if (canvasGroup != null) canvasGroup.gameObject.SetActive(false);
            });
        canvasGroup.DOFade(0, fadeTime);
    }
}