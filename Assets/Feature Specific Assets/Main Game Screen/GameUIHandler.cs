#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class GameUIHandler : MonoBehaviour
{

    [SerializeField] private ToggleSpriteSwap randomizationButton;

    void Start()
    {
        randomizationButton.onValueChanged += UpdateRandomizationSetting;
        randomizationButton.IsOn = GeneralSettingsManager.Instance.isRandomOn;
    }

    private void UpdateRandomizationSetting(bool isOn)
    {
        GeneralSettingsManager.Instance.isRandomOn = isOn;
    }

    private void UnSub()
    {
        randomizationButton.onValueChanged -= UpdateRandomizationSetting;
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
