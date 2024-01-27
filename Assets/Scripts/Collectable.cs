using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] public UIManager ui;
    public SoundManager soundManager;
    public AudioClip collectSFX;

    private void Start()
    {
        soundManager = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
    }

    private void OnTriggerEnter(Collider item)
    {
        if (item.CompareTag("Player"))
        {
            soundManager.PlaySFX(collectSFX, 1);
            ui.ChangeStarDustAmount(1);
            Destroy(gameObject);
        }
    }
}
