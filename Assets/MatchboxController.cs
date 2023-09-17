using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MatchboxController : MonoBehaviour
{
    [SerializeField] private GameObject matchPrefab;
    [SerializeField] private XRBaseInteractable matchInteractable;

    private void OnEnable()
    {
        matchInteractable.selectEntered.AddListener(SpawnMatch);
    }

    private void OnDisable()
    {
        matchInteractable.selectEntered.RemoveListener(SpawnMatch);

    }

    private void SpawnMatch(SelectEnterEventArgs arg0)
    {
        var m_XRManager = arg0.manager;
        var m_interactor = arg0.interactorObject;
        //var m_interactable = arg0.interactableObject;

        Debug.Log("SpawningMatch");
        m_XRManager.SelectEnter(m_interactor, arg0.interactableObject);

        var match = Instantiate(matchPrefab);
        var m_interactable = match.GetComponent<XRGrabInteractable>();
        m_XRManager.SelectEnter(m_interactor, m_interactable);
    }
}
