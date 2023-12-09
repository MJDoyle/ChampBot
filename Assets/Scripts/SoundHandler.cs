using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class SoundHandler : MonoBehaviour
{
    [SerializeField]
    private AudioSource blockSound;

    [SerializeField]
    private AudioSource faSound;

    [SerializeField]
    private AudioSource fallSound;

    [SerializeField]
    private AudioSource injurySound;

    [SerializeField]
    private AudioSource organSound;

    [SerializeField]
    private AudioSource ripSound;

    [SerializeField]
    private AudioSource koSound;

    public static SoundHandler instance;

    public static void PlayBlockSound()
    {
        instance.blockSound.PlayOneShot(instance.blockSound.clip);
    }

    public static async void PlayFASound()
    {
        await Task.Delay(1000);

        instance.faSound.PlayOneShot(instance.faSound.clip);
    }

    public static void PlayKnockDownSound()
    {
        instance.fallSound.PlayOneShot(instance.fallSound.clip);
    }

    public static void PlayKOSound()
    {
        instance.koSound.PlayOneShot(instance.koSound.clip);
    }

    public static void PlayInjurySound()
    {
        instance.injurySound.PlayOneShot(instance.injurySound.clip);
    }

    public static void PlayDeathSound()
    {
        if (Random.Range(0f, 1f) > 0.5f)
            instance.organSound.PlayOneShot(instance.organSound.clip);

        else
            instance.ripSound.PlayOneShot(instance.ripSound.clip);
    }

    void Awake()
    {
        instance = this;
    }
}
