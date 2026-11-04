
using UnityEngine;

public class EnemyGroupMember : MonoBehaviour
{
    private EnemySpawner spawner;
    private int groupId;
    private bool initialized;
    private bool reported;

    public void Initialize(EnemySpawner owner, int id)
    {
        spawner = owner;
        groupId = id;
        initialized = true;
    }

    public void ReportDeath()
    {
        if (reported || !initialized)
            return;

        reported = true;

        if (spawner != null)
            spawner.RegisterEnemyKill(groupId);
    }
}
