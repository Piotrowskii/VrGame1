using UnityEngine;

public class DestructionEffect : MonoBehaviour
{
    [Header("Destruction prefab")]
    [SerializeField] private GameObject brokenPrefab;
    [SerializeField] private Vector3 scaleMultiplier = Vector3.one;


    [Header("Explosion")]
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardModifier = 0f;

    [Header("Physics")]
    [SerializeField] private bool useGravity = true;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float angularDrag = 2f;

    [Header("Cleanup")]
    [SerializeField] private float destroyDelay = 5f;

    public float DestroyDelay => destroyDelay;

    public void Spawn(Vector3 position, Quaternion rotation, Vector3 explosionPoint)
    {
        if(brokenPrefab == null)
        {
            Debug.LogWarning("Broken prefab is empty");
            return;
        }

        GameObject brokenInstance = Instantiate(brokenPrefab, position, rotation);

        brokenInstance.transform.localScale = Vector3.Scale(brokenInstance.transform.localScale,scaleMultiplier);

        Rigidbody[] shardBodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in shardBodies)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.useGravity = useGravity;
            rb.linearDamping = drag;
            rb.angularDamping = angularDrag;

            rb.AddExplosionForce(
                explosionForce,
                explosionPoint,
                explosionRadius,
                upwardModifier,
                ForceMode.Impulse
            );
        }

        Destroy(brokenInstance, destroyDelay);
    }
}
