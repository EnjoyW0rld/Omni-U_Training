using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Holds current data for UI writing per team
/// Used for master scene
/// </summary>
public class TeamDataHandler : MonoBehaviour
{
    [SerializeField] private int teamId;
    [SerializeField] private GameObject _notifyIcon;
    [SerializeField] private GameObject _teamScreen;
    //[SerializeField] private TextMeshProUGUI _senderText;
    //[SerializeField, Tooltip("Button to access sent email")] private GameObject _incomingEmail;
    [Header("Email panel")]
    [SerializeField] private TextMeshProUGUI _emailText;
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

    private void Start()
    {
        //_instancedButtons = new Dictionary<UserData.TextData, Button>();
    }

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
    }
    public void UpdateText(string pText, string pSender)
    {
        _notifyIcon.SetActive(true);

        // Creating button to call incoming email screen and adding it to the dictionary to keep track
        // of the instanced buttons and user data they are linked to
        UserData.TextData recievedData = new UserData.TextData(pText, pSender);
        Button incomingButton = Instantiate(_incomingEmailPrefab);
        incomingButton.transform.SetParent(_incomingEmailsParent);
        incomingButton.GetComponentInChildren<TextMeshProUGUI>().text = MakeEmailTitle(recievedData);

        _instancedButtons.Add(recievedData, incomingButton);
        incomingButton.onClick.AddListener(() =>
        {
            _currentEmailData = recievedData;
            ShowEmail();
        });
    }
    public void SendReply()
    {
        // Sending reply through the network to the client
        EmailingContainer cont = new EmailingContainer(EmailingContainer.Instructions.Email);
        cont.Email = _replyInput.text;
        cont.Sender = _currentEmailData.Sender;
        cont.Reply = _replyInput.text;
        NetworkPacket packet = new NetworkPacket();
        packet.Write(cont);

        _currentEmailData.Reply = _replyInput.text;
        ServerBehaviour.Instance.AddEmailToTeamArchive(_currentEmailData, teamId);
        ServerBehaviour.Instance.ScheduleMessage(packet, teamId);
        AddToArchive();
        // Destroying and removing button from the instanced dictionary
        Destroy(_instancedButtons[_currentEmailData].gameObject);
        _instancedButtons.Remove(_currentEmailData);
    }
    private void AddToArchive()
    {
        EmailArchiveButton button = Instantiate(_emailArchiveButton);
        button.transform.SetParent(_archiveButtonsParent);
        //button.Initialize(ServerBehaviour.Instance.GetUserDataByID(teamId - 1).GetEmailsCount() - 1, teamId, _singleEmailUI);
        button.Initialize(_currentEmailData, _singleEmailUI);
    }
    public static string MakeEmailTitle(UserData.TextData pEmailData)
    {
        return $"Email to {pEmailData.Sender}";
    }
}