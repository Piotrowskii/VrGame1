using UnityEngine;

public class VRHelper : MonoBehaviour
{
    public static VRHelper Instance;

    [Header("controllers")]
    public GameObject leftControllerVisuals;
    public GameObject rightControllerVisuals;

    [Header("pitch pointers")]
    public GameObject leftPitchPointerVisuals;
    public GameObject rightPitchPointerVisuals;

    private void Start()
    {
        Instance = this;
    }

    public void HideLeftController()
    {
        leftControllerVisuals.SetActive(false);
        leftPitchPointerVisuals.SetActive(false);
    }

    public void ShowLeftController()
    {
        leftControllerVisuals.SetActive(true);
        leftPitchPointerVisuals.SetActive(true);
    }

    public void HideRightController()
    {
        rightControllerVisuals.SetActive(false);
        rightPitchPointerVisuals.SetActive(false);
    }

    public void ShowRightController()
    {
        rightControllerVisuals.SetActive(true);
        rightPitchPointerVisuals.SetActive(true);
    }
}
