using UnityEngine;

public class PersistBackground : MonoBehaviour
{
    private AudioSource musicPlayer;

    public static PersistBackground Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
    }

    public void OnUpdateMusic(bool isOn)
    {
        if (isOn) musicPlayer.UnPause();
        else musicPlayer.Pause();
    }
}
