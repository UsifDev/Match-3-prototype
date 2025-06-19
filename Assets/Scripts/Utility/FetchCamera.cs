using UnityEngine;

public class FetchCamera : MonoBehaviour
{
    Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if (canvas.worldCamera == null) canvas.worldCamera = Camera.main;
    }
}
