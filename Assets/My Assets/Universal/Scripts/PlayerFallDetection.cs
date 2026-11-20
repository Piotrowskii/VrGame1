using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerFallDetection : MonoBehaviour
{
    private Vector3 startPlayerPosition;
    [SerializeField] private Vector3 startPlayerForward = Vector3.forward;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        startPlayerPosition = GameManager.Instance.startPlayerPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<XROrigin>(out var xrOrigin))
        {
            ResetPlayer(xrOrigin);
        }
    }

    private void ResetPlayer(XROrigin xrOrigin)
    {
        CharacterController cc = xrOrigin.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        xrOrigin.MatchOriginUpCameraForward(Vector3.up, startPlayerForward);

        Vector3 heightOffset = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
        xrOrigin.MoveCameraToWorldLocation(startPlayerPosition + heightOffset);

        if (cc != null) cc.enabled = true;
    }
}
