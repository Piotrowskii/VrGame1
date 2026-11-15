using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour
{
    [Header("Weapon settings")]
    [Min(1)]
    public int magazineSize = 12;
    public bool infiniteAmmo = false;

    [Min(0)]
    public int bulletsLeft = 12;

    [Header("Firing Settings")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Min(0f)]
    public float projectileSpeed = 30f;

    [Min(0)]
    public int shootDelayMs = 250;

    [Header("AudioSources")]
    public AudioSource shootAudioSource;
    public AudioSource emptyAudioSource;

    [Header("Animation")]
    public Animator weaponAnimator;
    public AnimationClip recoilClip;

    [Header("VrSettings")]
    public Transform attachPoint;

    [Header("Optional")]
    public bool destroyProjectileOnDisable = false;

    private XRGrabInteractable grabInteractable;
    private float nextShootTime;

    public int MagazineSize => magazineSize;
    public int BulletsLeft => bulletsLeft;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.attachTransform = attachPoint;

        magazineSize = Mathf.Max(1, magazineSize);
        bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
    }

    private void OnEnable()
    {
        if(grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.activated.AddListener(OnActivated);
    }

    private void OnDisable()
    {
        if (grabInteractable != null) 
        {
            grabInteractable.activated.RemoveListener(OnActivated);
        }
        
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Fire();
    }

    public void Fire()
    {
        if(Time.time < nextShootTime)
        {
            return;
        }

        if (bulletsLeft <= 0 && infiniteAmmo == false)
        {
            emptyAudioSource.Play();
            nextShootTime = Time.time + shootDelayMs / 1000f;
            return;
        }

        if (firePoint == null) return;

        if (projectilePrefab == null) return;

        var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        var rigidBody = projectile.GetComponent<Rigidbody>();
        var projectileScript = projectile.GetComponent<Projectile>();

        if(projectileScript != null)
        {
            projectileScript.EnableProjectile();
        }

        if (rigidBody != null)
        {
            rigidBody.useGravity = false;
            rigidBody.linearVelocity = firePoint.forward * projectileSpeed;
        }

        shootAudioSource.Play();
        PlayRecoilAnimation();

        if(infiniteAmmo == false) bulletsLeft--;

        nextShootTime = Time.time + shootDelayMs / 1000f;
    }

    public void Reload()
    {
        bulletsLeft = magazineSize;
    }

    private void PlayRecoilAnimation()
    {
        if (weaponAnimator == null || recoilClip == null) return;
        weaponAnimator.Play(recoilClip.name);
    }

    private void OnValidate()
    {
        magazineSize = Mathf.Max(1, magazineSize);
        bulletsLeft = Mathf.Clamp(bulletsLeft, 0, magazineSize);
        projectileSpeed = Mathf.Max(0f, projectileSpeed);

        if (firePoint == null)
        {
            Debug.LogWarning( $"{name}: firePoint is not assigned!",this);
        }

        if (projectilePrefab == null)
        {
            Debug.LogWarning(  $"{name}: projectilePrefab is not assigned!", this);
        }

        if (shootAudioSource == null)
        {
            Debug.LogWarning($"{name}: shootAudioSource is not assigned!", this);
        }

        if(emptyAudioSource == null)
        {
            Debug.LogWarning($"{name}: emptyAudioSource is not assigned!", this);
        }

        if (weaponAnimator == null)
        {
            Debug.LogWarning($"{name}: weaponAnimator is not assigned!", this);
        }

        if (recoilClip == null)
        {
            Debug.LogWarning($"{name}: recoilClip is not assigned!", this);
        }
    }
}
