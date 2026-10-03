using System;
using UnityEngine;

public class StunEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyMovement>();
            enemy.stunned = true;
        }
    }
}
