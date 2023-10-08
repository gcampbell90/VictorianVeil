using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTextMesh : MonoBehaviour
{
    private TextMeshPro _textMesh;
    private Transform _itemTransform;
    private Vector3 _offset;
    private Transform _cameraTransform;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (_itemTransform != null)
        {
            UpdatePosition();
        }
    }

    internal void SetItemText(InventoryObject item)
    {
        _itemTransform = item.transform;
        _offset = item.TextPos;

        SetText(item.ItemName);
        UpdatePosition();
        gameObject.SetActive(true);
    }

    internal void HideText()
    {
        gameObject.SetActive(false);
    }

    private void SetText(string name)
    {
        _textMesh.text = name;
    }

    private void UpdatePosition()
    {
        _textMesh.transform.position = _itemTransform.position + _offset;
        transform.rotation = Quaternion.LookRotation(transform.position - _cameraTransform.position);
        
    }

}
