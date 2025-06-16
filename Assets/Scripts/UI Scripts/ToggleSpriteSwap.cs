// Credits to https://coremission.net/author/snezhok_13/
// Modified the class to get the image from the Button game object instead of passing the image to it and deleted the initialize method as i dont plan to utilize it

using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ToggleSpriteSwap : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private bool isOn;

    public bool IsOn { get { return isOn; } set { isOn = value; UpdateValue(); } }
    public event Action<bool> onValueChanged;

    private Button button;
    private Image targetImage;

    void Start () 
    {
        targetImage = GetComponent<Image>();
        button = GetComponent<Button>(); 
        button.transition = Selectable.Transition.None; 
        button.onClick.AddListener(OnClick); 
    } 

    void OnClick() 
    { 
        isOn = !isOn;
        UpdateValue();
    }

    private void UpdateValue(bool notifySubscribers = true) 
    { 
        if (notifySubscribers && onValueChanged != null) 
            onValueChanged(isOn); 
        if (targetImage == null) return; 
        targetImage.sprite = isOn ? onSprite : offSprite; 
    } 
}
