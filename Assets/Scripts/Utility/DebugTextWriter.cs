using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTextWriter : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _text;
    private static DebugTextWriter instance;

    public static void WriteOnScreen(string pText)
    {
        if (instance == null)
        {
            instance = FindObjectOfType<DebugTextWriter>();
        }
        instance.UpdateText(pText);
    }
    public void UpdateText(string pText)
    {
        _text.text = pText;
    }
}
