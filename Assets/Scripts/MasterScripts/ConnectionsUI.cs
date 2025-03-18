using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Placeholder UI, would be changed in the future to something more meaningfull
/// </summary>
public class ConnectionsUI : MonoBehaviour
{
    [SerializeField] private RawImage[] _indicators;

    private void FixedUpdate()
    {
        if (_indicators == null || _indicators.Length < 0) return;
        bool[] arr = ServerBehaviour.Instance.GetTeamNumbers();
        for (int i = 0; i < arr.Length; i++)
        {
            _indicators[i].color = arr[i] ? new Color(0.78f, 0.1f, 0.42f) : new Color(0.1f, 0.78f, 0.53f);
            
        }
        /*bool[] arr = ServerBehaviour.Instance.GetTeamNumbers();
        int conn = ServerBehaviour.Instance.ConnectionsCount;
        for (int i = 0; i < _indicators.Length; i++)
        {
            _indicators[i].SetActive(conn >= i + 1);
        }*/
    }
}
