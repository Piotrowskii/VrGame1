using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    BulletImpact
}

[System.Serializable]
public class ParticleEntry
{
    public ParticleType type;
    public ParticleSystem prefab;
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [SerializeField] private List<ParticleEntry> particles;
    private Dictionary<ParticleType, ParticleSystem> particleDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        particleDictionary = new Dictionary<ParticleType, ParticleSystem>();

        foreach (var entry in particles) 
        {
            particleDictionary[entry.type] = entry.prefab;
        }
    }

    public void PlayEffect(ParticleType type, Vector3 position, Vector3? scale = null)
    {
        if (!particleDictionary.TryGetValue(type, out var prefab)) return;

        prefab.transform.localScale = scale.Value;
        ParticleSystem effect = Instantiate(prefab, position, Quaternion.identity);

        effect.transform.localScale = scale ?? Vector3.one;

        effect.Play();

        Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
    }
}
