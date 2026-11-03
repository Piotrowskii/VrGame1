using UnityEngine;

public interface IShootable
{
    void OnShot(Projectile projectile, Vector3 hitPoint);
}
