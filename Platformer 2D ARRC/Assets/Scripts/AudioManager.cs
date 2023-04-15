using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    } //Para el singleton del AudioManager instance
    private AudioSource audioSrc;

    private void Start() => audioSrc = GetComponent<AudioSource>();
    public void PlaySound(AudioClip clip) => audioSrc.PlayOneShot(clip);
}
