using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour
{
    private const int TEAMS_COUNT = 3;
    [SerializeField] private GameObject[] _teamButtons;
    //private bool[] _teamsInUse;

    private void Start()
    {
        if (ClientBehaviour.Instance != null)
        {
            TeamSelectionContainer accessContainer = new TeamSelectionContainer(TeamSelectionContainer.Instruction.TeamAccessibility);
            NetworkPacket accessPackage = new NetworkPacket();
            //accessContainer.Serialize(accessPackage);
            accessPackage.Write(accessContainer);
            ClientBehaviour.Instance.SchedulePackage(accessPackage);
            Debug.Log("Sending access package");
        }
    }

    public void Use(TeamSelectionContainer pContainer)
    {
        if (ServerBehaviour.IsThisUserServer)
        {
            MasterUse();
        }
        else
        {
            ClientUse(pContainer);
        }
    }

    private void ClientUse(TeamSelectionContainer pContainer)
    {
        if (pContainer.TeamsInUse == null || pContainer.TeamsInUse.Length < 0) return;

        for (int i = 0; i < TEAMS_COUNT; i++)
        {
            Debug.Log("Removing unised teams");
            _teamButtons[i].SetActive(pContainer.TeamsInUse[i]);
        }
    }
    private void MasterUse()
    {
        /*if (_instruction == Instruction.TeamAccessibility)
        {
            NetworkPacket teamsInUsePacket = new NetworkPacket();
            teamsInUsePacket.Write(this);

            ServerBehaviour.Instance.ScheduleMessage(teamsInUsePacket);
        }*/
    }
}
