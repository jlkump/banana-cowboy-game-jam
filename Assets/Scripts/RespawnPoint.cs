using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [field: SerializeField] public Vector3 respawnPos { get; private set; }
    [SerializeField] public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // set game managers respawn coords to set coordinates
            gameManager.setRespawn(respawnPos);
        }
    }
}
