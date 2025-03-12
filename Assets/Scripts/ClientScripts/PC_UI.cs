using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_UI : MonoBehaviour
{
    [SerializeField] private EmailUI _emailUI;
    [SerializeField] private EmailArchiveButton _emailArchiveButtonPrefab;
    [SerializeField] private Transform _archiveEmailsParent;
    [SerializeField] private GameObject _archiveTab;

    public void ShowArchive()
    {
        _archiveTab.gameObject.SetActive(true);
    }
    public void HideArchive()
    {
        _archiveTab.SetActive(false);
        _emailUI.gameObject.SetActive(false);
    }
    public void AddToArchive(UserData.TextData pEmailData, bool pCallNotification = true)
    {
        EmailArchiveButton archiveButton = Instantiate(_emailArchiveButtonPrefab);
        archiveButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = TeamDataHandler.MakeEmailTitle(pEmailData);
        archiveButton.Initialize(pEmailData, _emailUI);
        archiveButton.transform.SetParent(_archiveEmailsParent);
        if (pCallNotification) ReferenceHandler.GetObject<NotificationHandler>(true).CallNotification(NotificationHandler.NotificationType.Email);
    }
}