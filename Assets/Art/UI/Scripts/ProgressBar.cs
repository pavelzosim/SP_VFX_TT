using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ProgressBar : MonoBehaviour
{
    [System.Serializable]
    public class ProgressBarSettings
    {
        public int maxValue = 100;
        public int currentValue = 0;
    }

    [Header("Data")]
    public ProgressBarSettings settings = new ProgressBarSettings();

    [Header("UI References")]
    [Tooltip("The Image that uses the LinearProgressBar shader.")]
    public Image targetImage;

    [Tooltip("Optional Text to display current/max values.")]
    public Text displayText;

    [Header("Shader")]
    [Tooltip("Assign your UI/LinearProgressBar material here.")]
    public Material baseProgressBarMaterial;

    // Internal
    private Material _instancedMaterial;

    void OnEnable()
    {
        if (targetImage == null)
        {
            Debug.LogWarning($"[{nameof(ProgressBar)}] No targetImage assigned on '{name}'.", this);
            return;
        }

        if (baseProgressBarMaterial != null)
        {
            // Instantiate a unique copy
            _instancedMaterial = Instantiate(baseProgressBarMaterial);
            _instancedMaterial.name = baseProgressBarMaterial.name + " (Instanced)";
            // Assign to the target Image
            targetImage.material = _instancedMaterial;
        }
    }

    void Update()
    {
        if (targetImage == null || settings == null || settings.maxValue <= 0)
            return;

        // 1) Compute normalized fill [0..1]
        float fillNorm = Mathf.Clamp01((float)settings.currentValue / settings.maxValue);

        // 2) (Optional) drive Unity's built-in fillAmount
        targetImage.fillAmount = fillNorm;

        // 3) Set the shader's _Fill on the *rendering* material
        if (_instancedMaterial != null)
        {
            var mat = targetImage.materialForRendering;
            if (mat != null)
            {
                mat.SetFloat("_Fill", fillNorm);
                targetImage.SetVerticesDirty();
                targetImage.SetMaterialDirty();
            }
        }

        // 4) Update text if provided
        if (displayText != null)
            displayText.text = $"{settings.currentValue}/{settings.maxValue}";
    }

    void OnDisable()
    {
        if (_instancedMaterial != null)
            DestroyImmediate(_instancedMaterial);
    }
}
