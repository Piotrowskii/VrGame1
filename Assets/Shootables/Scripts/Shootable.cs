using UnityEngine;
using UnityEngine.Events;


public class Shootable : MonoBehaviour, IShootable
{
    [Header("Hit Sound")]
    [SerializeField] private AudioClip shotSound;

    [Range(0f, 1f)]
    [SerializeField] private float shotVolume = 1f;

    public UnityEvent<Shootable, Bullet, Vector3> OnShot;

    public virtual void RegisterHit(Bullet bullet, Vector3 hitPoint)
    {
        if(shotSound != null)
        {
            AudioSource.PlayClipAtPoint(shotSound, hitPoint, shotVolume);
        }

        HandleShot(bullet, hitPoint);
        OnShot?.Invoke(this, bullet, hitPoint);
    }

    void IShootable.OnShot(Bullet bullet, Vector3 hitPoint)
    {
        RegisterHit(bullet, hitPoint);
    }

    protected virtual void HandleShot(Bullet bullet, Vector3 hitPoint)
    {

    }
}
