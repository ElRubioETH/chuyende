using UnityEngine;
using Fusion;

public class Bullet : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage = 5;

    public Player owner; // Reference to shooter

    public override void Spawned()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        transform.position += transform.forward * speed * Runner.DeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;

        Player hitPlayer = other.GetComponent<Player>();

        // 👉 Hit a player
        if (hitPlayer != null)
        {
            if (hitPlayer == owner)
            {
                // 🍃 Ignore hitting the owner
                return;
            }

            // 💥 Damage other players
            hitPlayer.RpcUpdateHealth(hitPlayer.health - damage);
            DestroySelf();
        }
        else
        {
            // 💣 Hit something else (wall, enemy, whatever)
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
    }
}
