using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static int totalStars;
    public static WinManager instance;
    public TMP_Text starResult;
    // Start is called before the first frame update
    void Start()
    {
        if (totalStars == 1)
        {
            starResult.text = "You got " + totalStars + " star sparkle!";
        }
        else
        {
            starResult.text = "You got " + totalStars + " star sparkles!";
        }
    }
}
