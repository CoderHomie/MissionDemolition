using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SlingshotSFX : MonoBehaviour
{
    [Header("Inscribed")]
    public AudioClip snapClip;
    [Range(0f, 1f)]
    public float volume = 1f;

    private AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
        _src.playOnAwake = false;
    }

    public void PlaySnap()
    {
        if (_src == null || snapClip == null) return;
        _src.PlayOneShot(snapClip, volume);
    }
}

