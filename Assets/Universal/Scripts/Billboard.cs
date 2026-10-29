using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Billboard Settings")]
    public Vector3 rotationOffset = new Vector3(0f, 180f, 0f);

    private void LateUpdate()
    {
        var camera = Camera.main;
        if (camera == null) return;

        Vector3 direction = camera.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset); ;
    }
}
