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
    [SerializeField] private int teamId;
    [SerializeField] private GameObject _notifyIcon;
    [SerializeField] private GameObject _teamScreen;
    [SerializeField] private TextMeshProUGUI _senderText;
    [SerializeField, Tooltip("Button to access sent email")] private GameObject _incomingEmail;
    [Header("Email panel")]
    [SerializeField] private TextMeshProUGUI _emailText;
    [SerializeField] private GameObject _emailPanel;
    [SerializeField] private TMP_InputField _replyInput;
    [Header("Archive")]
    [SerializeField] private EmailArchiveButton _emailArchiveButton;
    [SerializeField] private EmailUI _singleEmailUI;
    [SerializeField] private Transform _archiveButtonsParent;

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
        UpdateText(pTextData.Text, pTextData.Sender);
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
        _currentEmail = new UserData.TextData(pText, pSender);
        _senderText.text = _currentEmail.Sender;
    }
    public void SendReply()
    {
        EmailingContainer cont = new EmailingContainer(EmailingContainer.Instructions.Email);
        cont.Email = _replyInput.text;
        cont.Sender = _currentEmail.Sender;
        NetworkPacket packet = new NetworkPacket();
        packet.Write(cont);

        _currentEmail.Reply = _replyInput.text;
        ServerBehaviour.Instance.AddEmailToTeamArchive(_currentEmail, teamId);
        ServerBehaviour.Instance.ScheduleMessage(packet, teamId);
        AddToArchive();
        _incomingEmail.SetActive(false);
    }
    private void AddToArchive()
    {
        EmailArchiveButton button = Instantiate(_emailArchiveButton);
        button.transform.SetParent(_archiveButtonsParent);
        button.Initialize(ServerBehaviour.Instance.GetUserDataByID(teamId - 1).GetEmailsCount() - 1, teamId, _singleEmailUI);
    }
}