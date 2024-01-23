using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider item)
    {
        if (item.CompareTag("Player"))
        {
            UIManager.ChangeStarDustAmount(1);
            Destroy(gameObject);
        }
    }
}
