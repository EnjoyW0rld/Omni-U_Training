using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseQueueUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _prefabTitle;
    private GeneralPhase _phase;
    public void Initialize(GeneralPhase pPhase)
    {
        _phase = pPhase;
        if (pPhase is EmailPhase)
        {
            _prefabTitle.text = $"Email phase with title: {(_phase as EmailPhase).EmailText.Title}";
        }
    }
}
