using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(DestructionEffect))]
public class Destructable : Shootable
{
    [Header("Models")]
    [SerializeField] private GameObject intactModel;
    

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private bool respawn = true;

    [Header("Spawn animation")]
    [SerializeField] private UnityEvent onSpawned;

    private DestructionEffect destructionEffect;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isBroken;

    protected virtual void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        destructionEffect = GetComponent<DestructionEffect>();

        intactModel.SetActive(true);


        onSpawned?.Invoke();
    }

    protected override void HandleShot(Projectile projectile, Vector3 hitPoint)
    {
        Break(hitPoint);
    }

    public void Break(Vector3 hitPoint) 
    {
        if (isBroken) return;
        isBroken = true;

        intactModel.SetActive(false);

        destructionEffect.Spawn(
            transform.position,
            transform.rotation,
            hitPoint
        );

        if (respawn)
        {
            StartCoroutine(RespawnAfterDelay());
        }
        else
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);


        transform.SetPositionAndRotation(startPosition, startRotation);


        intactModel.SetActive(true);

        isBroken = false;

        onSpawned?.Invoke();
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Destroy(gameObject);
    }
}
