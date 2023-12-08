using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    [SerializeField]
    private AudioSource blockSound;

    public static SoundHandler instance;

    public static void PlayBlockSound()
    {
        instance.blockSound.PlayOneShot(instance.blockSound.clip);
    }

    void Awake()
    {
        instance = this;
    }
}
