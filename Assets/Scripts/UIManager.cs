using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // THIS SCRIPT SHOULD BE LOADED IN ONCE
    public static int starDustAmount = 0;
    public TMP_Text starDustUI;
    public static UIManager instance;

    private void Start()
    {
        instance = this;
        instance.starDustUI.text = "X " + starDustAmount.ToString();
    }

    public static void ChangeStarDustAmount(int change)
    {
        starDustAmount += change;
        instance.starDustUI.text = "X "+starDustAmount.ToString();
    }
}
