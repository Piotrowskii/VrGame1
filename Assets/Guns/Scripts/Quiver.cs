using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class Quiver : MonoBehaviour
{
    [Header("Spawning prefab")]
    [SerializeField] private GameObject spawnPrefab;

    private XRGrabInteractable xrGrabInteractable;

    private void Awake()
    {
        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        xrGrabInteractable.trackPosition = false;
        xrGrabInteractable.trackRotation = false;
        xrGrabInteractable.trackScale = false;
        xrGrabInteractable.throwOnDetach = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        xrGrabInteractable.selectEntered.AddListener(SpawnAndGrabPrefab);
    }

    private void OnDisable()
    {
        xrGrabInteractable.selectEntered.RemoveListener(SpawnAndGrabPrefab);
    }

    private void SpawnAndGrabPrefab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;
        if (interactor == null || spawnPrefab == null) return;

        Transform handTransform = interactor.GetAttachTransform(xrGrabInteractable);
        GameObject prefabObj = Instantiate(spawnPrefab, handTransform.position, handTransform.rotation);

        if (!prefabObj.TryGetComponent(out XRGrabInteractable prefabInteratable))
        {
            Debug.LogError($"{name}: Spawned prefab doesn't have XRGrabInteractable!");
            Destroy(prefabObj);
            return;
        }

        xrGrabInteractable.interactionManager.SelectEnter(interactor, prefabInteratable);
    }
}
