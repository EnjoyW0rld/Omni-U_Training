using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Holds current data for UI writing per team
/// Used for master scene
/// </summary>
public class TeamDataHandler : MonoBehaviour
{
    [SerializeField] private GameObject _notifyIcon;
    [SerializeField] private GameObject _teamScreen;
    [SerializeField] private TextMeshProUGUI _senderText;
    [SerializeField] private GameObject _incomingEmail;
    [Header("Email panel")]
    [SerializeField] private TextMeshProUGUI _emailText;
    [SerializeField] private GameObject _emailPanel;

    private UserData.TextData _currentEmail;

    public void DeActivateTeamScreen()
    {
        _teamScreen.SetActive(false);
    }
    public void ActivateTeamScreen()
    {
        _teamScreen.SetActive(true);
        _notifyIcon.SetActive(false);
    }
    public void UpdateText(UserData.TextData pTextData)
    {
        UpdateText(pTextData.Text,pTextData.Sender);
    }
    public void ShowEmail()
    {
        _emailPanel.SetActive(true);
        _emailText.text = _currentEmail.Text;
    }
    public void UpdateText(string pText, string pSender)
    {
        _notifyIcon.SetActive(true);
        _incomingEmail.SetActive(true);
        _currentEmail = new UserData.TextData(pText,pSender);
        _senderText.text = _currentEmail.Sender;
    }
}
