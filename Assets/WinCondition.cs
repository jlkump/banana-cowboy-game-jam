using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int total = other.gameObject.GetComponentInParent<UIManager>().starDustAmount;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            WinManager.totalStars = total;
            PlayerData.resetData();
            SceneManager.LoadScene(2);
        }
    }
}
