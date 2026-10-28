using System.Collections;
using System.Threading;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float scaleDelay = 0.1f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Particles")]
    [SerializeField] private ParticleSystem poofPrefab;
    [SerializeField] private float startSize = 8f;
    [SerializeField] private float startSpeed = 1f;
    [SerializeField] private float shapeRadius = 0.1f;
    [SerializeField] private float particleDuration = 0.1f;
    [SerializeField] private Vector3 particleRotation = new Vector3(-90f,0f,0f);


    private Vector3 originalScale;
    private Coroutine animationCoroutine;
    private ParticleSystem poofInstance;


    private void Awake()
    {
        originalScale = transform.localScale;

        if (poofPrefab != null)
        {
            poofInstance = Instantiate(
                poofPrefab,
                transform.position,
                Quaternion.Euler(particleRotation)
            );

            var main = poofInstance.main;
            main.startSize = startSize;
            main.duration = particleDuration;
            main.startSpeed = startSpeed;

            var shape = poofInstance.shape;
            shape.radius = shapeRadius;

            poofInstance.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    public void Play()
    {
        if (animationCoroutine != null) 
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        transform.localScale = Vector3.zero;

        PlayPoof();

        animationCoroutine = StartCoroutine(ScaleUp());
    }

    private void PlayPoof()
    {
        if (poofInstance == null) return;

        poofInstance.transform.position = transform.position;
        poofInstance.transform.rotation = Quaternion.Euler(particleRotation);

        poofInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        poofInstance.Play();
    }

    private IEnumerator ScaleUp()
    {
        if (scaleDelay > 0f) yield return new WaitForSeconds(scaleDelay);

        if (duration <= 0f)
        {
            transform.localScale = originalScale;
            animationCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float t = Mathf.Clamp(elapsed / duration, 0f, 1f);
            float cureValue = scaleCurve.Evaluate(t);

            transform.localScale = originalScale * cureValue;

            yield return null;
        }

        transform.localScale = originalScale;
        animationCoroutine = null;
    }

    private void OnDestroy()
    {
        if (poofInstance != null) Destroy(poofInstance.gameObject);
    }
}
