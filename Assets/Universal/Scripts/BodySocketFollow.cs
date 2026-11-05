using UnityEngine;

public class BodySocketFollow : MonoBehaviour
{
    [SerializeField] private Transform headCamera;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0f);

    private void LateUpdate()
    {
        if (headCamera == null) return;

        Vector3 flatForward = Vector3.ProjectOnPlane(headCamera.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.001f) return;

        Quaternion bodyYaw = Quaternion.LookRotation(flatForward, Vector3.up);

        transform.position = headCamera.position + (bodyYaw * localOffset);
        transform.rotation = bodyYaw;
    }
}
