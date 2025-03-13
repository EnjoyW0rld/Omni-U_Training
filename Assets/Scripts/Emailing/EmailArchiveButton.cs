using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailArchiveButton : MonoBehaviour
{
    [SerializeField] private Button _showEmailButton;
    [SerializeField] private TextMeshProUGUI _archiveTitle;
    [Header("Pretty UI variables")]
    [SerializeField] private bool _isPrettyUI;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _from;
    [SerializeField] private TextMeshProUGUI _to;
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
        if (!_isPrettyUI)
        {
            _archiveTitle.text = TeamDataHandler.MakeEmailTitle(pEmail);
        }
        else
        {
            _title.text = pEmail.Title;
            _from.text = pEmail.Sender;
            _to.text = pEmail.Recipient;
        }
    }
    public void CallArchivedEmail()
    {
        _emailUI.gameObject.SetActive(true);
        _emailUI.SetEmailText(_email);
    }
}