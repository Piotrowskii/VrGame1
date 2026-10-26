using UnityEngine;

public class VRHelper : MonoBehaviour
{
    public static VRHelper instance;

    [Header("controllers")]
    public GameObject leftControllerVisuals;
    public GameObject rightControllerVisuals;

    private void Start()
    {
        instance = this;
    }

    public void HideLeftController()
    {
        leftControllerVisuals.SetActive(false);
    }

    public void ShowLeftController()
    {
        leftControllerVisuals.SetActive(true);
    }

    public void HideRightController()
    {
        rightControllerVisuals.SetActive(false);
    }

    public void ShowRightController()
    {
        rightControllerVisuals.SetActive(true);
    }
}
