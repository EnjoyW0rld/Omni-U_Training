using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmailUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _origianalText;
    [SerializeField] private TextMeshProUGUI _sender;
    [SerializeField] private TextMeshProUGUI _reply;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _recipient;
    [SerializeField] private Button _backButton;

    public void SetEmailText(UserData.TextData pEmailText)
    {
        _origianalText.text = pEmailText.Text;
        _sender.text = pEmailText.Recipient;
        _reply.text = pEmailText.Reply;
        Debug.Log("Title is " + pEmailText.Title);
        _title.text = pEmailText.Title;
        _recipient.text = pEmailText.Recipient;
    }
}
