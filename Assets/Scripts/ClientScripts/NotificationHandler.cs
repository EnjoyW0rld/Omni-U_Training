using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _emailNotifications;
    [SerializeField] private GameObject _browserNotification;
    [SerializeField] private GameObject _generalEmailNotification;
    private int _pendingEmailsNotifications;

    private void Start()
    {
        DrawEmailNotifications();
    }

    public void HideBrowserNotification()
    {
        _browserNotification.SetActive(false);
    }
    public void ShowBrowserNotification()
    {
        _browserNotification.SetActive(true);
    }
    //----------
    // Email notification functions
    //----------
    public void AddEmailNotification()
    {
        _pendingEmailsNotifications++;
        _generalEmailNotification?.SetActive(true);
        DrawEmailNotifications();
    }
    public void DecrementUnreadEmails()
    {
        _pendingEmailsNotifications--;
        _generalEmailNotification?.SetActive(false);
        DrawEmailNotifications();
    }
    private void DrawEmailNotifications()
    {
        if(_pendingEmailsNotifications == 0)
        {
            _emailNotifications.gameObject.SetActive(false);
            return;
        }
        _emailNotifications.gameObject.SetActive(true);
        _emailNotifications.text = _pendingEmailsNotifications.ToString();
    }
}