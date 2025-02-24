using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseInteractionExecutor : MonoBehaviour
{
    [SerializeField] private string _phaseName;
    [SerializeField] private UnityEvent OnPhaseExecute;
    public void Execute(string pPhaseName)
    {
        Debug.Log($"Trying to call method {pPhaseName}");
        if (pPhaseName.Equals(_phaseName))
        {
            OnPhaseExecute?.Invoke();
        }
    }
}
