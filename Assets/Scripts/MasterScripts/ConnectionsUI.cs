using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionsUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _indicators;

    private void FixedUpdate()
    {
        if (_indicators == null || _indicators.Length < 0) return;
        bool[] arr = ServerBehaviour.Instance.GetTeamNumbers();
        int conn = ServerBehaviour.Instance.ConnectionsCount;
        for (int i = 0; i < _indicators.Length; i++)
        {
            _indicators[i].SetActive(conn >= i + 1);
        }
    }
}
