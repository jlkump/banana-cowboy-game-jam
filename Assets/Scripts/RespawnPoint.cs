using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [field: SerializeField] public int checkpointNum { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            // set game managers respawn coords to set coordinates
            PlayerData.checkpointsReached = checkpointNum;
        }
    }
}
