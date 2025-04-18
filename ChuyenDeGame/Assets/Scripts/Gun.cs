using UnityEngine;
using Fusion;

public class PlayerGun : NetworkBehaviour
{
    public Transform firePoint; // Vị trí bắn
    public NetworkPrefabRef bulletPrefab;

    public float fireRate = 0.3f;
    private float nextFireTime = 0f;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            RpcShoot();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcShoot()
    {
        if (firePoint == null) return;

        Runner.Spawn(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation,
            Object.InputAuthority
        );
    }
}
