using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailArchiveButton : MonoBehaviour
{
    [SerializeField] private Button _showEmailButton;
    [SerializeField] private TextMeshProUGUI _archiveTitle;
    private EmailUI _emailUI;
    private UserData.TextData _email;

    public void Initialize(int pEmailID, EmailUI pEmailUI)
    {
        _emailUI = pEmailUI;
        _showEmailButton.onClick.AddListener(CallArchivedEmail);
    }
    public void Initialize(UserData.TextData pEmail, EmailUI pEmailUI)
    {
        _email = pEmail;
        _emailUI = pEmailUI;
        _showEmailButton.onClick.AddListener(CallArchivedEmail);
        _archiveTitle.text = TeamDataHandler.MakeEmailTitle(pEmail);
    }
    public void CallArchivedEmail()
    {
        _emailUI.gameObject.SetActive(true);
        _emailUI.SetEmailText(_email);
    }
}
