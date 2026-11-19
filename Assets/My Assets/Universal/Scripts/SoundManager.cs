using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    PointSound,
}

[Serializable]
public class SoundEntry
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Sound Library")]
    [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }

    public void PlaySound(SoundType type)
    {
        SoundEntry entry = sounds.Find(s => s.type == type);

        if (entry != null)
        {
            if (entry.clip != null)
            {
                audioSource.PlayOneShot(entry.clip, entry.volume);
            }
            else
            {
                Debug.LogWarning($"SoundManager: AudioClip is missing for {type}");
            }
        }
        else
        {
            Debug.LogWarning($"SoundManager: SoundType {type} not found in the sound list");
        }
    }
}