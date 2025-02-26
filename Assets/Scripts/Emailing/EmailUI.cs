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
    [SerializeField] private Button _backButton;

    public void SetEmailText(UserData.TextData pEmailText)
    {
        _origianalText.text = pEmailText.Text;
        _sender.text = pEmailText.Sender;
        _reply.text = pEmailText.Reply;
    }
}
