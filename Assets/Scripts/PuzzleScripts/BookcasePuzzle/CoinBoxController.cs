using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CoinBoxController : MonoBehaviour
{
    [SerializeField] private List<CustomSocketInteractor> _coinSockets = new List<CustomSocketInteractor>();
    private Dictionary<int, bool> _socketState = new Dictionary<int, bool>();

    private delegate void UnlockBookCase();
    private UnlockBookCase _unlockBookCase;

    private void OnEnable()
    {
        for (int i = 0; i < _coinSockets.Count; i++)
        {
            int m_socketIndex = i;
            _coinSockets[i].selectEntered.AddListener(delegate { SetCoinSocket(m_socketIndex); });
            _socketState[i] = false;
        }

        _unlockBookCase += CompleteCoinBox;
    }

    private void OnDisable()
    {
        for (int i = 0; i < _coinSockets.Count; i++)
        {
            _coinSockets[i].selectEntered.RemoveAllListeners();
        }

        _unlockBookCase -= CompleteCoinBox; 
    }

    private void SetCoinSocket(int i)
    {
        Debug.Log($"Socket Sorted {i}");
        _socketState[i] = true;
        CheckState();
    }

    private void CheckState()
    {
        for (int i = 0; i < _socketState.Count; i++)
        {
            if (_socketState[i])
            {
                Debug.Log($"Coin {i} entered");
            }
            else
            {
                return;
            }
        }
        Debug.Log("All coins entered");
        _unlockBookCase?.Invoke();
    }

    private void CompleteCoinBox()
    {
        BookCasePuzzle.onCompletePuzzle?.Invoke();
    }

}
