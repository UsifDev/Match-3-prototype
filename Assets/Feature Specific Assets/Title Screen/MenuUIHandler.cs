#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] private ToggleSpriteSwap musicButton;
    [SerializeField] private ToggleSpriteSwap soundButton;

    void Start()
    {
        musicButton.onValueChanged += UpdateMusicSetting;
        soundButton.onValueChanged += UpdateSoundSetting;
    }

    private void UpdateSoundSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isSoundOn = isOn;
    }
    private void UpdateMusicSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isMusicOn = isOn;
    }

    private void UnSub()
    {
        musicButton.onValueChanged -= UpdateMusicSetting;
        soundButton.onValueChanged -= UpdateSoundSetting;
    }

    public void LoadNext(string nextSceneName)
    {
        UnSub();
        SceneManager.LoadScene(nextSceneName);
    }

    public void Exit()
    {
        UnSub();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
