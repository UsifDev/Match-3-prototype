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

    private void OnDestroy()
    {
        toggleButton.onValueChanged -= UpdateValue;
    }
}
