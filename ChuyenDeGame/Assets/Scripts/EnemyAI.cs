using UnityEngine;
using UnityEngine.AI;
using Fusion;
public class PigAI : NetworkBehaviour
{
    public NavMeshAgent agent;
    public GameObject[] targets;

    // Update is called once per frame
    void Update()
    {
        // tìm các game object có tag là "Player"
        targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length == 0) return;

        // tìm target gần nhất
        GameObject target = null;
        float minDistance = Mathf.Infinity;
        foreach (var t in targets)
        {
            var distance = Vector3.Distance(t.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = t;
            }
        }

        if (target != null) agent.SetDestination(target.transform.position);
    }
}
