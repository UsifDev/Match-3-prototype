using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private ToggleSpriteSwap randomizationButton;

    [SerializeField] private AudioClip buttonPressClip;
    private AudioSource gameUIsoundsPlayer;
    void Start()
    {
        randomizationButton.onValueChanged += UpdateRandomizationSetting;
        randomizationButton.IsOn = GeneralSettingsManager.Instance.isRandomOn;
        gameUIsoundsPlayer = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (GeneralSettingsManager.Instance.isSoundOn) gameUIsoundsPlayer.PlayOneShot(buttonPressClip, 0.2f);
    }

    private void UpdateRandomizationSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isRandomOn = isOn;
    }

    public void LoadNext(string nextSceneName)
    {
        randomizationButton.onValueChanged -= UpdateRandomizationSetting;
        SceneManager.LoadScene(nextSceneName);
    }
}
