using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour
{
    private const int TEAMS_COUNT = 3;
    [SerializeField] private GameObject[] _teamButtons;
    [SerializeField] private string _playerSceneName;

    //private bool[] _teamsInUse;

    private void Start()
    {
        if (ClientBehaviour.Instance != null)
        {
            TeamSelectionContainer accessContainer = new TeamSelectionContainer(TeamSelectionContainer.Instruction.TeamAccessibility);
            NetworkPacket accessPackage = new NetworkPacket();
            accessPackage.Write(accessContainer);
            ClientBehaviour.Instance.SchedulePackage(accessPackage);
            Debug.Log("Sending access package");
        }
    }

    public void Use(TeamSelectionContainer pContainer)
    {

        ClientUse(pContainer);
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
    /// <summary>
    /// Send connect to team request to the server specifying desired team number
    /// </summary>
    /// <param name="pTeamID"></param>
    public void ConnectToTeam(int pTeamID)
    {
        TeamSelectionContainer cont = new TeamSelectionContainer(TeamSelectionContainer.Instruction.ConnectRequest);
        cont.TeamId = pTeamID;
        NetworkPacket pack = new NetworkPacket();
        pack.Write(cont);

        ClientBehaviour.Instance.SchedulePackage(pack);
    }
}
