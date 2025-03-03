using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Holds current data for UI writing per team
/// Used for master scene
/// Sends email replies to the users
/// </summary>
public class TeamDataHandler : MonoBehaviour
{
    [SerializeField] private int _teamId;
    [SerializeField] private GameObject _notifyIcon;
    [SerializeField] private GameObject _teamScreen;
    [Header("Email panel")]
    [SerializeField] private TextMeshProUGUI _emailText;
    [SerializeField] private TextMeshProUGUI _emailTitle;
    [SerializeField] private GameObject _emailPanel;
    [SerializeField] private TMP_InputField _replyInput;
    [SerializeField] private Transform _incomingEmailsParent;
    [SerializeField] private Button _incomingEmailPrefab;
    [Header("Archive")]
    [SerializeField] private EmailArchiveButton _emailArchiveButton;
    [SerializeField] private EmailUI _singleEmailUI;
    [SerializeField] private Transform _archiveButtonsParent;

    private UserData.TextData _currentEmailData;
    private Dictionary<UserData.TextData, Button> _instancedButtons = new Dictionary<UserData.TextData, Button>();

    public void DeActivateTeamScreen()
    {
        _teamScreen.SetActive(false);
    }
    public void ActivateTeamScreen()
    {
        _teamScreen.SetActive(true);
        _notifyIcon.SetActive(false);
    }
    public void ShowEmail()
    {
        _emailPanel.SetActive(true);
        _emailText.text = _currentEmailData.Text;
        _emailTitle.text = _currentEmailData.Title;
    }
    /// <summary>
    /// Create incoming email button and email TextData
    /// </summary>
    /// <param name="pText"></param>
    /// <param name="pSender"></param>
    public void InitializeIncomingEmail(string pText, string pSender, string pTitle)
    {
        InitializeIncomingEmail(new UserData.TextData(pText, pSender, pTitle));
    }
    public void InitializeIncomingEmail(UserData.TextData pTextData)
    {
        _notifyIcon.SetActive(true);

        // Creating button to call incoming email screen and adding it to the dictionary to keep track
        // of the instanced buttons and user data they are linked to
        Button incomingButton = Instantiate(_incomingEmailPrefab);
        incomingButton.transform.SetParent(_incomingEmailsParent);
        incomingButton.GetComponentInChildren<TextMeshProUGUI>().text = MakeEmailTitle(pTextData);
        _instancedButtons.Add(pTextData, incomingButton);

        incomingButton.onClick.AddListener(() =>
        {
            _currentEmailData = pTextData;
            ShowEmail();
        });
    }
    public void SendReply()
    {
        // Sending reply through the network to the client
        EmailingContainer cont = GetReplyMailContainer();

        _currentEmailData.Reply = _replyInput.text;
        ServerBehaviour.Instance.AddEmailToTeamArchive(_currentEmailData, _teamId);
        ServerBehaviour.Instance.ScheduleMessage(cont.PackObject(), _teamId);
        AddToArchive();
        // Destroying and removing button from the instanced dictionary
        Destroy(_instancedButtons[_currentEmailData].gameObject);
        _instancedButtons.Remove(_currentEmailData);

        AddPointsToTeam(10);
    }
    private void AddToArchive()
    {
        EmailArchiveButton button = Instantiate(_emailArchiveButton);
        button.transform.SetParent(_archiveButtonsParent);
        button.Initialize(_currentEmailData, _singleEmailUI);
    }
    private void AddPointsToTeam(int pScore)
    {
        ScoreContainer scoreContainer = new ScoreContainer(ScoreContainer.Instructions.AssignScore);
        scoreContainer.TargetTeam = _teamId;
        scoreContainer.ScoreToAdd = pScore;
        ServerBehaviour.Instance.ScheduleMessage(scoreContainer.PackObject());
    }

    private EmailingContainer GetReplyMailContainer()
    {
        EmailingContainer emailContainer = new EmailingContainer(EmailingContainer.Instructions.Email);
        emailContainer.Email = _currentEmailData.Text;
        emailContainer.Recipient = _currentEmailData.Recipient;
        emailContainer.Reply = _replyInput.text;
        emailContainer.Sender = _currentEmailData.Sender;
        emailContainer.Title = _currentEmailData.Title;
        return emailContainer;
    }
    public static string MakeEmailTitle(UserData.TextData pEmailData)
    {
        return $"Email to {pEmailData.Recipient} with title {pEmailData.Title}";
    }
}