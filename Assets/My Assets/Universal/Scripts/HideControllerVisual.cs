using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static UnityEngine.XR.OpenXR.Features.Interactions.HTCViveControllerProfile;

[RequireComponent(typeof(XRGrabInteractable))]
public class HideControllerVisual : MonoBehaviour
{

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(HideController);
        grabInteractable.selectExited.AddListener(ShowController);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(HideController);
            grabInteractable.selectExited.RemoveListener(ShowController);
        }

    }

    private void HideController(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;
        var controller = interactor.transform.gameObject;
        var parent = interactor.transform.parent;

        if (parent.name.Contains("Left"))
        {
            VRHelper.Instance.HideLeftController();
        }
        else
        {
            VRHelper.Instance.HideRightController();
        }
    }

    private void ShowController(SelectExitEventArgs args)
    {
        var interactor = args.interactorObject;
        var controller = interactor.transform.gameObject;
        var parent = interactor.transform.parent;

        if (parent.name.Contains("Left"))
        {
            VRHelper.Instance.ShowLeftController();
        }
        else
        {
            VRHelper.Instance.ShowRightController();
        }
    }
}
