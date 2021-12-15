using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Actions/Play Sound")]
public class ActionPlaySound : ScriptableObject
{
    [SerializeField, HideInInspector]
    private static AudioSource fxAudioSource;
    [SerializeField]
    private AudioMixerGroup fxAudioMixer;

    private bool m_bInitialized = false;

    public void Initialize()
    {
		if (m_bInitialized) { return; }
        m_bInitialized = true;
        /*
        musicAudioSource = CreateAudioSource();
        musicAudioSource.outputAudioMixerGroup = musicAudioMixer;
        */

        fxAudioSource = CreateAudioSource();
        fxAudioSource.outputAudioMixerGroup = fxAudioMixer;
        /*

        musicEnabled = save.musicEnabled;
        fxEnabled = save.fxEnabled;

        hideFlags = HideFlags.DontUnloadUnusedAsset;
        */
    }

    public void PlaySound(AudioClip clip)
    {
        Initialize();
        if (fxAudioSource.enabled)
        {
            fxAudioSource.PlayOneShot(clip);
        }
    }
    private AudioSource CreateAudioSource()
    {
        AudioSource newSource = new GameObject().AddComponent<AudioSource>();
        GameObject.DontDestroyOnLoad(newSource);

#if UNITY_EDITOR
        newSource.name = "Audio Source";
#endif
        return newSource;
    }

}

