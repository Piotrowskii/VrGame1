using UnityEngine;

public class AttachFpsDisplay : MonoBehaviour
{
    [Header("XR Setup")]
    [SerializeField] private Transform xrRig;
    [SerializeField] private Transform controller;

    [Header("Display Position")]
    [SerializeField]
    private Vector3 localPosition = new Vector3(0f, 0.08f, 0.15f);

    [SerializeField]
    private Vector3 localRotation =  Vector3.zero;

    void Start()
    {
        Transform target = controller;

        if (target == null && xrRig != null)
        {
            target = xrRig.Find("Camera Offset/Right Controller");
        }

        if (target == null)
        {
            Debug.LogWarning( "FPS Display: Assign the controller or check its hierarchy." );
            return;
        }

        transform.SetParent(target, false);
        transform.localPosition = localPosition;
        transform.localEulerAngles = localRotation;
    }
}
