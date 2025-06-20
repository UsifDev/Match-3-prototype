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

    [SerializeField] private AudioClip buttonPressClip;
    private AudioSource menuSoundsPlayer;

    void Start()
    {
        musicButton.onValueChanged += UpdateMusicSetting;
        soundButton.onValueChanged += UpdateSoundSetting;
        menuSoundsPlayer = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if(GeneralSettingsManager.Instance.isSoundOn) menuSoundsPlayer.PlayOneShot(buttonPressClip, 0.2f);
    }

    private void UpdateSoundSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isSoundOn = isOn;
    }
    private void UpdateMusicSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isMusicOn = isOn;
        PersistBackground.Instance.OnUpdateMusic(isOn);
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
