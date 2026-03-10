using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Exemple d'un panneau de paramètres simple avec volume et qualité graphique.
/// À adapter selon vos besoins.
/// </summary>
public class SettingsPanelController : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeLabel;

    [Header("Qualité")]
    [SerializeField] private Dropdown qualityDropdown;

    void Start()
    {
        // Initialiser le slider de volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Initialiser le dropdown de qualité
        if (qualityDropdown != null)
        {
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        if (volumeLabel != null)
            volumeLabel.text = $"Volume: {Mathf.RoundToInt(value * 100)}%";
    }

    private void OnQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        Debug.Log($"Qualité graphique changée à: {QualitySettings.names[index]}");
    }
}
