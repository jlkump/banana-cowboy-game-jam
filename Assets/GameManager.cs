using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // serialized fields
    [SerializeField] Vector3 defRespawnCoords;
    [SerializeField] Vector3 defCheckpoint;
    [SerializeField] Vector3 defSecondCheckpoint;
    [SerializeField] ThirdPersonController player;

    public GameObject pauseMenu;

    private void Awake()
    {
        // set respawn coords, too little time to make something intuitive
        PlayerData.levelCoords = defRespawnCoords;
        PlayerData.checkpt1Coords = defCheckpoint;
        PlayerData.checkpt2Coords = defSecondCheckpoint;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1.0f;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
        }
    }

    // resetScene with the players most recent respawn coordinates
    public void resetSceneWithRespawnCoords()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ButtonsPressed(Button button)
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        switch (button.name)
        {
            case "Resume":
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                break;
            case "Restart":
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case "Quit":
                SceneManager.LoadScene(0);
                PlayerData.resetData();
                break;

        }
    }
}

