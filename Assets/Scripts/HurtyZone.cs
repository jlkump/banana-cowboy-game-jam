using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Worked on by: Joaquin 
   A basic script for damaging the player upon entering the
   "hurty zone".

   Modify this later when hazards are fully designed. */
public class HurtyZone : MonoBehaviour
{
    
    [field: SerializeField]
    public bool kill { get; private set; }
    private void OnTriggerEnter(Collider other)
    {
        // check if the collided entity is the player
        if (kill)
        {
            // damage the player
            UIManager.ChangeHealth(-3);
        } else {
            if (other.CompareTag("Player"))
            {
                // damage the player
                UIManager.ChangeHealth(-1);
            }
        }
       
    }
}
