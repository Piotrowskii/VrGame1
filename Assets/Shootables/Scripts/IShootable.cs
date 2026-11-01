using UnityEngine;

public interface IShootable
{
    void OnShot(Bullet bullet, Vector3 hitPoint);
}
