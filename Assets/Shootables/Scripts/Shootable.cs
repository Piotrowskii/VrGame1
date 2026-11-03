using UnityEngine;
using UnityEngine.Events;


public class Shootable : MonoBehaviour, IShootable
{
    [Header("Hit Sound")]
    [SerializeField] private AudioClip shotSound;

    [Range(0f, 1f)]
    [SerializeField] private float shotVolume = 1f;

    public UnityEvent<Shootable, Projectile, Vector3> OnShot;

    public virtual void RegisterHit(Projectile projectile, Vector3 hitPoint)
    {
        if(shotSound != null)
        {
            AudioSource.PlayClipAtPoint(shotSound, hitPoint, shotVolume);
        }

        HandleShot(projectile, hitPoint);
        OnShot?.Invoke(this, projectile, hitPoint);
    }

    void IShootable.OnShot(Projectile projectile, Vector3 hitPoint)
    {
        RegisterHit(projectile, hitPoint);
    }

    protected virtual void HandleShot(Projectile projectile, Vector3 hitPoint)
    {

    }
}
