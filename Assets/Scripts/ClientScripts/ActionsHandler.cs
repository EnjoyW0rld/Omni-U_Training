using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for storing and executing actions
/// Is responsible for timing of the actions
/// </summary>
public class ActionsHandler : MonoBehaviour
{
    [SerializeField] private int _maxActions = 5;
    [SerializeField, Tooltip("In seconds")] private int _actionCooldown = 30;
    private int _currentActionCount;
    private float _timeToNextAction;

    [SerializeField] private GameObject[] _actionButtons;

    private void Start()
    {
        _timeToNextAction = _actionCooldown;
        _currentActionCount = _maxActions;
        if (_actionButtons == null || _actionButtons.Length != _maxActions) Debug.LogError("Incorrect amount of action buttons setted");
        PerformAction();
    }

    private void Update()
    {
        if (_currentActionCount < _maxActions)
        {
            _timeToNextAction -= Time.deltaTime;
            if (_timeToNextAction < 0)
            {
                _actionButtons[_currentActionCount].SetActive(true);
                _currentActionCount++;
                _timeToNextAction = _actionCooldown;
            }
        }
    }
    public bool PerformAction()
    {
        if (_currentActionCount > 0)
        {
            _currentActionCount--;
            _actionButtons[_currentActionCount].SetActive(false);
            return true;
        }
        return false;
    }
}