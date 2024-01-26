using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private GameObject dialogueHolder;
    private TMP_Text textDisplay;
    private float typingSpeed = 0.05f;
    private static string text;

    private static DialogueSystem instance;
    private Coroutine typingCoroutine;

    private void Start()
    {
        instance = this;
        dialogueHolder = transform.GetChild(0).gameObject;
        textDisplay = dialogueHolder.transform.GetChild(1).GetComponent<TMP_Text>();
    }
    IEnumerator Type(string sentence)
    {
        textDisplay.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public static void StartText(string sentence)
    {
        text = sentence;
        instance.dialogueHolder.SetActive(true);
        instance.typingCoroutine = instance.StartCoroutine(instance.Type(sentence));
    }

    public static void StopText()
    {
        if (instance.typingCoroutine != null)
        {
            instance.StopCoroutine(instance.typingCoroutine);
        }
        instance.dialogueHolder.SetActive(false);
    }
}
