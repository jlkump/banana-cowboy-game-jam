using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] public UIManager ui;
    private void OnTriggerEnter(Collider item)
    {
        if (item.CompareTag("Player"))
        {
            ui.ChangeStarDustAmount(1);
            Destroy(gameObject);
        }
    }
}
