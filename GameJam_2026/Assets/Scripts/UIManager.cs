using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;
    static public UIManager Instance { get { return instance; } }

    public event Action<string, Transform> DisplayClueTextEvent;
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private TMP_Text displayText = null;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DisplayClueTextEvent += DisplayClueText;
    }

    private void OnDisable()
    {
        DisplayClueTextEvent -= DisplayClueText;
    }

    public void RequestDisplayClueText(string clueString, Transform clue)
    {
        DisplayClueTextEvent?.Invoke(clueString, clue);
    }

    public void DisplayClueText(string clueString, Transform clue)
    {
        TMP_Text textToDisplay = Instantiate(displayText, canvasPrefab.transform);
        textToDisplay.transform.position = Camera.main.WorldToScreenPoint(clue.position) + new Vector3(50.0f, 50.0f, 0f);
        textToDisplay.text = clueString;
    }
}