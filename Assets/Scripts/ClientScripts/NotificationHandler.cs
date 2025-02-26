using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    [Serializable] public enum NotificationType { General = 1, Email = 2, Browser = 3 }

    [SerializeField] private GameObject _generalNotifcation;
    [SerializeField] private GameObject _emailNotification;
    [SerializeField] private GameObject _browserNotification;
    private Dictionary<NotificationType, GameObject> _notificationDict;
    private void Start()
    {
        _notificationDict = new Dictionary<NotificationType, GameObject>()
        {
            {NotificationType.General, _generalNotifcation},
            {NotificationType.Email, _emailNotification},
            {NotificationType.Browser, _browserNotification}
        };
    }

    public void CallNotification()
    {
        _generalNotifcation.SetActive(true);
    }
    public void HideNotification()
    {
        _generalNotifcation.SetActive(false);
    }
    public void CallNotification(NotificationType pNotification)
    {
        _notificationDict[pNotification].SetActive(true);
    }
    public void HideNotification(NotificationType pNotification)
    {
        _notificationDict[pNotification].SetActive(false);
    }
    public void CallNotification(int pNotification)
    {
        _notificationDict[(NotificationType)pNotification].SetActive(true);
    }
    public void HideNotification(int pNotification)
    {
        _notificationDict[(NotificationType)pNotification].SetActive(false);
    }
}
