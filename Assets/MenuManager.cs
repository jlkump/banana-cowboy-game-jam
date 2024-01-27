using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject creditScreen;

    public void ButtonsPressed(Button button)
    {
        transform.GetComponent<AudioSource>().Play()
            ;
        switch (button.name)
        {
            case "Play":
                SceneManager.LoadScene(1);
                break;
            case "Credits":
                creditScreen.SetActive(true);
                break;
            case "Back":
                // Make everything false here
                creditScreen.SetActive(false);
                break;
        }
    }
}
