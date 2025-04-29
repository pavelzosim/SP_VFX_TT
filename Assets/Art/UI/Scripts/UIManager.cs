using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        public GameObject popupObject; // Added popup reference
        [Tooltip("Container for debug elements")]
        public GameObject debugContainer;
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

    private Material _instancedMaterial;
    private bool _lastDebugState;
    private float _lastFillAmount;

    void OnEnable()
    {
        InitializeProgressBar();
        _lastDebugState = debugMode;
        UpdateDebugVisibility();
    }

    void Update()
    {
        UpdateProgress();
        UpdateStickerText();

        // Check for debug mode changes only if necessary
        if (_lastDebugState != debugMode)
        {
            UpdateDebugVisibility();
            _lastDebugState = debugMode;
        }
    }

    void OnDisable()
    {
        CleanupMaterial();
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

        if (settings.currentValue == settings.maxValue && targetReferences.popupObject != null)
        {
            if (!targetReferences.popupObject.activeSelf)
                targetReferences.popupObject.SetActive(true);
        }

        // Animation trigger logic
        if (settings.currentValue == settings.maxValue)
        {
            if (!_maxValueReached)
            {
                // Start fade-in animation only once when reaching max
                PanelFadeIn();
                _maxValueReached = true;
            }
        }
        else
        {
            if (_maxValueReached)
            {
                // Start fade-out animation when dropping below max
                PanelFadeOut();
                _maxValueReached = false;
            }
        }

        // Update visuals only if fill amount changes
        if (!Mathf.Approximately(fillAmount, _lastFillAmount))
        {
            _lastFillAmount = fillAmount;
            targetReferences.targetMask.fillAmount = fillAmount;
            UpdateMaterial(fillAmount);
        }

        // Update text regardless of debug mode
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
        // Use debugContainer to toggle all debug elements
        if (targetReferences.debugContainer != null)
        {
            targetReferences.debugContainer.SetActive(debugMode);
        }
    }

    private void CleanupMaterial()
    {
        if (_instancedMaterial != null)
            DestroyImmediate(_instancedMaterial);
    }

    // Button Functionality

    // Sets the progress bar to 0% (empty bar)
    public void SetEmpty()
    {
        if (settings == null)
            return;

        settings.currentValue = 0; // Set progress to 0%
        UpdateProgress();
        UpdateProgressText();
        UpdateStickerText();
    }

    // Sets the progress bar to 100% (full bar)
    public void SetFull()
    {
        if (settings == null || settings.maxValue <= 0)
            return;

        settings.currentValue = settings.maxValue; // Set progress to 100%
        UpdateProgress();
        UpdateProgressText();
        UpdateStickerText();
    }

    // Sets the progress bar to a random value between 10% and 100%
    public void SetRandom()
    {
        if (settings == null || settings.maxValue <= 0)
            return;

        settings.currentValue = Random.Range(
            Mathf.RoundToInt(settings.maxValue * 0.1f), // 10%
            settings.maxValue + 1                      // 100% (inclusive)
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
            .OnComplete(() => {
                if (canvasGroup != null) canvasGroup.gameObject.SetActive(false);
            });
        canvasGroup.DOFade(0, fadeTime); // Fixed fade-out to 0
    }
}