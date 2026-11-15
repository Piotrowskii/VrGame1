using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(XRGrabInteractable))]
public class Bow : MonoBehaviour
{
    [Header("Pull positions")]
    [SerializeField] private Transform pullStart;
    [SerializeField] private Transform pullEnd;
    [SerializeField] private Transform pullTransform;


    //[Header("Input")]
    //[SerializeField] private InputActionReference holdAction;
    //[SerializeField] private float inputThreshold = 0.5f;

    [Header("Shooting")]
    [SerializeField] private float shootForce = 30f;

    [Header("String")]
    [SerializeField] private LineRenderer bowString;
    [SerializeField] private Transform stringTop;
    [SerializeField] private Transform stringBottom;

    private XRGrabInteractable xrGrabInteractable;

    private float pullAmount;
    private Arrow loadedArrow;
    private Transform arrowGrabController;

    private void Awake()
    {
        xrGrabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        xrGrabInteractable.selectEntered.AddListener(OnBowGrabbed);
        xrGrabInteractable.selectExited.AddListener(OnBowDropped);
    }

    private void OnDisable()
    {
        xrGrabInteractable.selectEntered.RemoveListener(OnBowGrabbed);
        xrGrabInteractable.selectExited.RemoveListener(OnBowDropped);
    }

    private void Update()
    {
        if (loadedArrow != null && xrGrabInteractable.isSelected && arrowGrabController != null)
        {
            Transform bowHand = xrGrabInteractable.firstInteractorSelecting.GetAttachTransform(xrGrabInteractable);
            Vector3 aimDirection = bowHand.position - arrowGrabController.position;

            if (aimDirection.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(aimDirection, bowHand.up);
            }

            pullAmount = CalculatePosition(arrowGrabController.position);

            pullTransform.position = Vector3.Lerp(
                pullStart.position,
                pullEnd.position,
                pullAmount
            );
        }

        UpdateBowString();
    }

    private void OnBowGrabbed(SelectEnterEventArgs args)
    {
        xrGrabInteractable.trackRotation = true;
    }

    private void OnBowDropped(SelectExitEventArgs args)
    {
        if(loadedArrow != null)
        {
            Shoot();
        }
        xrGrabInteractable.trackRotation = true;
    }

    public void TryLoadArrow(Collider other)
    {
        if (!xrGrabInteractable.isSelected) return;

        Arrow arrow = other.GetComponentInParent<Arrow>();

        if (arrow == null || arrow.isEnabled || arrow.isAttachedToTheBow) return;
        if (loadedArrow != null) return;

        LoadArrow(arrow);
        
    }

    private void UpdateBowString()
    {
        bowString.positionCount = 3;

        bowString.SetPosition(0, stringTop.position);
        bowString.SetPosition(1, pullTransform.position);
        bowString.SetPosition(2, stringBottom.position);
    }

    private void LoadArrow(Arrow arrow)
    {
        loadedArrow = arrow;
        xrGrabInteractable.trackRotation = false;

        arrow.AttachToBow();
        arrow.OnArrowReleased.AddListener(Shoot);

        arrowGrabController = arrow.GrabbingController;

        Rigidbody rb = loadedArrow.GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        loadedArrow.transform.SetParent(pullTransform, false);

        Transform grabTransform = loadedArrow.ArrowGrabTransform;
        loadedArrow.transform.localPosition = -grabTransform.localPosition;
        loadedArrow.transform.localRotation = Quaternion.Inverse(grabTransform.localRotation);

        pullTransform.position = pullStart.position;

        pullAmount = 0f;
    }

    private void Shoot()
    {
        if (loadedArrow == null) return;
        xrGrabInteractable.trackRotation = true;

        Arrow arrow = loadedArrow;
        arrow.OnArrowReleased.RemoveListener(Shoot);

        loadedArrow = null;
        arrowGrabController = null;

        arrow.transform.SetParent(null);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        arrow.EnableProjectile();

        if(rb != null)
        {
            rb.isKinematic = false;

            Vector3 shootDirection = pullTransform.forward;

            rb.AddForce(shootDirection * shootForce * pullAmount, ForceMode.Impulse);
        }

        pullTransform.position = pullStart.position;
        pullAmount = 0f;
    }

    private float CalculatePosition(Vector3 pullPosition)
    {
        Vector3 pullDirection = pullPosition - pullStart.position;
        Vector3 targetDirection = pullEnd.position - pullStart.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 1);
    }
}
