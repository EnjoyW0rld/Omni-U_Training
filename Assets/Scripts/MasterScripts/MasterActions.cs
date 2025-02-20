using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MasterActions : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _text;
    [SerializeField, Tooltip("Specify team in the correct order")] private TeamDataHandler[] _teamDataHandler;
    public void CallChangeScene()
    {
        NetworkSceneManager scene = new NetworkSceneManager();
        scene._sceneName = "SampleScene";
        NetworkPacket packet = new NetworkPacket();
        packet.Write(scene);
        ServerBehaviour.Instance.ScheduleMessage(packet);
    }
    public void ActivateTeam(int pTeamId)
    {
        for (int i = 0; i < _teamDataHandler.Length; i++)
        {
            if ((i + 1) == pTeamId) _teamDataHandler[i].ActivateTeamScreen();
            else _teamDataHandler[i].DeActivateTeamScreen();
        }
    }
    public void Use(ISerializable pObj)
    {
        if (pObj is EmailingContainer)
        {
            EmailingContainer container = pObj as EmailingContainer;
            //ServerBehaviour.Instance.GetCurrentUserData().AddEmail(container.Email, container.Sender);
            // _text[ServerBehaviour.Instance.GetCurrentConnectionTeam() - 1].text = (pObj as EmailingContainer).Email;
            _teamDataHandler[ServerBehaviour.Instance.GetCurrentConnectionTeam() - 1].UpdateText(container.Email,container.Sender);
        }
    }
}
