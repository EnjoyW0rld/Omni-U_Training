using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailArchiveButton : MonoBehaviour
{
    private int _emailID;
    private int _teamID;
    [SerializeField] private Button _showEmailButton;
    [SerializeField] private TextMeshProUGUI _archiveTitle;
    private EmailUI _emailUI;

    public void Initialize(int pEmailID, int pTeamID, EmailUI pEmailUI)
    {
        _emailUI = pEmailUI;
        _emailID = pEmailID;
        _teamID = pTeamID;
        _showEmailButton.onClick.AddListener(CallArchivedEmail);
    }
    public void CallArchivedEmail()
    {
        _emailUI.gameObject.SetActive(true);
        UserData.TextData data;
        if (ServerBehaviour.IsThisUserServer)
        {
            data = ServerBehaviour.Instance.GetUserDataByID(_teamID - 1).GetEmail(_emailID);
        }
        else
        {
            data = new UserData.TextData();
        }
        _emailUI.SetEmailText(data);
    }
}
