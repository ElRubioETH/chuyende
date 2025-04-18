using UnityEngine;
using UnityEngine.AI;
using Fusion;


public class EnemyAI_Fusion : NetworkBehaviour
{
    public float speed = 3.5f;
    public float detectionRadius = 30f;
    public float stoppingDistance = 1f;

    private Vector3 defaultPosition;
    private NavMeshPath path;
    private int pathIndex = 0;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        defaultPosition = transform.position;
        path = new NavMeshPath();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        Vector3 targetPos = GetTargetPosition();
        MoveAlongPath(targetPos);
    }

    Vector3 GetTargetPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var player in players)
        {
            float dist = Vector3.Distance(defaultPosition, player.transform.position);
            if (dist <= detectionRadius && dist < minDist)
            {
                minDist = dist;
                closest = player;
            }
        }

        return closest ? closest.transform.position : defaultPosition;
    }

    void MoveAlongPath(Vector3 destination)
    {
        if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path))
        {
            if (path.corners.Length < 2) return;

            // Nếu đã gần điểm hiện tại thì chuyển sang điểm kế tiếp
            Vector3 nextPoint = path.corners[1];
            float distToNext = Vector3.Distance(transform.position, nextPoint);

            if (distToNext < stoppingDistance)
                return;

            Vector3 dir = (nextPoint - transform.position).normalized;
            transform.position += dir * speed * Time.fixedDeltaTime;
            transform.forward = dir;
        }
    }
}
