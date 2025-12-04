using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip audioClip2;
    public AudioClip audioClip3;
    public AudioClip audioClip4;

    void Animation()
    {
        audioSource.PlayOneShot(audioClip);
    }
    void Animation2()
    {
        audioSource.PlayOneShot(audioClip2);
    }
    void Animation3()
    {
        audioSource.PlayOneShot(audioClip3);
    }
    void Animation4()
    {
        audioSource.PlayOneShot(audioClip4);
    }
}
