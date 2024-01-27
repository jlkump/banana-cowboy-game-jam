using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] public GameManager gameManager;

    // THIS SCRIPT SHOULD BE LOADED IN ONCE
    public int starDustAmount = 0;
    public TMP_Text starDustUI;
    public static UIManager instance;

    public int health;
    // The icon that is displayed for health
    public GameObject bananaIcon;
    // Stores all of the icons
    public Sprite[] bananaIcons;

    public Animator animator;

    private void Start()
    {
        instance = this;
        instance.starDustUI.text = "X " + starDustAmount.ToString();
        health = 3;
        instance.bananaIcon.GetComponent<Image>().sprite = instance.bananaIcons[health];
    }

    public void ChangeStarDustAmount(int change)
    {
        starDustAmount += change;
        instance.starDustUI.text = "X "+starDustAmount.ToString();
    }

    public void ChangeHealth(int change)
    {
        health += change;
        if (change < 0)
        {
            instance.animator.SetTrigger("Shake");
        }
        if (health >= 0 || health < 3)
        {
            instance.bananaIcon.GetComponent<Image>().sprite = instance.bananaIcons[health];
        }
        if(health == 0)
        {
            // TODO add death screen here
            gameManager.resetSceneWithRespawnCoords();
        }
    }
}
