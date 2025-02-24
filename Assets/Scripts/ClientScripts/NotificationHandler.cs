using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    public enum NotificationType { General, Email }

    [SerializeField] private GameObject _generalNotifcation;
    [SerializeField] private GameObject _emailNotification;
    private Dictionary<NotificationType, GameObject> _notificationDict;
    private void Start()
    {
        _notificationDict = new Dictionary<NotificationType, GameObject>()
        {
            {NotificationType.General, _generalNotifcation},
            {NotificationType.Email, _emailNotification}
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
}
