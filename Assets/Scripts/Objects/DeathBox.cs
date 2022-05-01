using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterController>().enabled = false;
            Vector3 spawn = respawnPoint.transform.position;
            spawn.y += 3;
            other.transform.position = spawn;
            other.GetComponent<CharacterController>().enabled = true;

        }
    }
}
