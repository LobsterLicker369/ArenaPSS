using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip musicClip;
    public float musicVolume = 0.5f;

    private AudioSource _musicSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = musicClip;
        _musicSource.loop = true;
        _musicSource.volume = musicVolume;
        _musicSource.playOnAwake = false;
    }

    void Start()
    {
        if (musicClip != null)
            _musicSource.Play();
    }
}