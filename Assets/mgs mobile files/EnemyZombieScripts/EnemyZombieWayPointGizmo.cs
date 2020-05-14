using UnityEngine;
using System.Collections;

public class EnemyZombieWayPointGizmo : MonoBehaviour {

    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "wayPoint.png", true);
    }
}
