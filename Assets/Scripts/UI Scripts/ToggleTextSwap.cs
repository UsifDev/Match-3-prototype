using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ToggleTextSwap : MonoBehaviour
{
    [SerializeField] private string onText;
    [SerializeField] private string offText;
    [SerializeField] private ToggleSpriteSwap toggleButton;
    [SerializeField] private TextMeshProUGUI targetText;

    private Button button;

    void Start()
    {
        Debug.Log(targetText.text);
        button = GetComponent<Button>();
        button.transition = Selectable.Transition.None;

        toggleButton.onValueChanged += UpdateValue;
        UpdateValue(toggleButton.IsOn);
    }

    private void UpdateValue(bool isOn)
    {
        if (targetText == null) return;
        targetText.text = isOn ? onText : offText;
    }

    private void OnEnable()
    {
        toggleButton.onValueChanged += UpdateValue;
    }

    private void OnDisable()
    {
        toggleButton.onValueChanged -= UpdateValue;
    }
}
