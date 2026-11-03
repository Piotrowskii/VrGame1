using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Arrow : Projectile
{
    [Header("Arrow")]
    [SerializeField] public Transform ArrowGrabTransform;
    public Transform GrabbingController { get; private set;  }

    public UnityEvent OnArrowReleased;

    private bool isAttachedToTheBow { get; set; }

    private XRGrabInteractable xrGrabInteractable;

    private void Awake()
    {
        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        if (ArrowGrabTransform != null)
        {
            xrGrabInteractable.attachTransform = ArrowGrabTransform;
        }
    }

    private void OnEnable()
    {
        xrGrabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        xrGrabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        OnArrowReleased.Invoke();
        if (!isAttachedToTheBow)
        {
            EnableProjectile();
        }
    }

    public void AttachToBow()
    {
        isAttachedToTheBow = true;

        xrGrabInteractable.trackPosition = false;
        xrGrabInteractable.trackRotation = false;
        xrGrabInteractable.trackScale = false;
        xrGrabInteractable.throwOnDetach = false;
        GrabbingController = xrGrabInteractable.firstInteractorSelecting.transform;
    }
}
