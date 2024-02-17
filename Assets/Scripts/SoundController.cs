using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // public singleton manager object
    public static SoundController instance;

    public SoundMap[] soundMapping;

    private static Dictionary<SoundType, AudioSource> soundDict;

    public enum SoundType
    {
        SwordAttack,
        AreaAttack
    }

    // key-value pair for sound mapping
    [Serializable]
    public struct SoundMap
    {
        public SoundType   type;
        public AudioSource file;
    }

    public void PlaySound (SoundType sound)
    {
        if (soundDict.ContainsKey(sound))
            soundDict[sound].Play();
    }
        
    public void PlayDelayed(SoundType sound, float delay)
    {
        if (soundDict.ContainsKey(sound))
            if (soundDict[sound] != null)
                soundDict[sound].PlayDelayed(delay);
    }

    public void StopSound (SoundType sound)
    {
        if (soundDict.ContainsKey(sound))
            if (soundDict[sound] != null)
                soundDict[sound].Stop();
    }

    private void Awake()
    {
        // there should be only one
        if (instance == null)
            instance = this;
        else
            Debug.Log("Duplicate Sound Controller detected, ignoring this one.");
    }

    // Start is called before the first frame update
    void Start()
    {
        soundDict = new Dictionary<SoundType, AudioSource>();

        // build the dictionary for fast run-time lookup
        foreach (SoundMap sound in soundMapping)
        {
            if (sound.file != null)
                soundDict.Add(sound.type, sound.file);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
