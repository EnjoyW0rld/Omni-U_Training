using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamScoreHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _teams;
    private GameObject _parentObject;
    private int[] _score;

    private void Start()
    {
        _parentObject = gameObject;
        _teams[ClientBehaviour.Instance.TeamNubmer - 1].color = Color.red;
        _score = new int[_teams.Length];
        UpdateScores();
    }

    private void UpdateScores()
    {
        for (int i = 0; i < _teams.Length; i++)
        {
            _teams[i].text = "Team " + (i + 1) + ": " + _score[i];
        }
    }
}
