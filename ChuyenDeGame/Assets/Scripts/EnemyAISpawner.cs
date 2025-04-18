using UnityEngine;
using Fusion;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef enemyPrefab;
    [SerializeField] private Transform spawnPoint;

    public override void Spawned()
    {
        Debug.Log("[Spawner] Spawning enemy on server.");
        Runner.Spawn(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity,
            inputAuthority: null // Server giữ quyền hoàn toàn
        );
    }
}
