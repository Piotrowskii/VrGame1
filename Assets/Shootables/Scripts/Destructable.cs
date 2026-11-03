using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Destructable : Shootable
{
    [Header("Models")]
    [SerializeField] private GameObject intactModel;
    [SerializeField] private GameObject brokenModel;

    [Header("Explosion")]
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardModifier = 0.5f;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private bool respawn = true;

    [Header("Spawn animation")]
    [SerializeField] private UnityEvent onSpawned;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isBroken;

    private Rigidbody[] shardBodies;

    private readonly Dictionary<Transform, Vector3> startLocalPositions = new();
    private readonly Dictionary<Transform, Quaternion> startLocalRotations = new();

    protected virtual void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        shardBodies = brokenModel.GetComponentsInChildren<Rigidbody>();

        foreach(var rb in shardBodies)
        {
            Transform t = rb.transform;

            startLocalPositions[t] = t.localPosition;
            startLocalRotations[t] = t.localRotation;

            rb.isKinematic = true;
        }

        intactModel.SetActive(true);
        brokenModel.SetActive(false);

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
        brokenModel.SetActive(true);

        foreach(var rb in shardBodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddExplosionForce(
                explosionForce,
                hitPoint,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

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

        ResetShards();

        brokenModel.SetActive(false);
        intactModel.SetActive(true);

        isBroken = false;

        onSpawned?.Invoke();
    }

    private void ResetShards()
    {
        foreach (var rb in shardBodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        foreach (var rb in shardBodies)
        {
            Transform t = rb.transform;
            t.localPosition = startLocalPositions[t];
            t.localRotation = startLocalRotations[t];
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Destroy(gameObject);
    }
}
