using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int total = ui.gameObject.GetComponent<UIManager>().starDustAmount;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            WinManager.totalStars = total;
            PlayerData.resetData();
            SceneManager.LoadScene(2);
        }
    }
}
