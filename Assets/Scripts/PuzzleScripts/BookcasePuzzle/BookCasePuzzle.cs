using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookCasePuzzle : BasePuzzle
{
    [SerializeField] private CoinBoxController _coinBox;

    public delegate void CompleteCoinPuzzle();
    public static CompleteCoinPuzzle onCompletePuzzle;
    [SerializeField] private float pos_Offset;

    [Header("Debug Options")]
    [SerializeField] bool isUnlocked;

    //Unity Methods
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        onCompletePuzzle += TriggerBookCaseMovement;
    }

    protected override void Start()
    {
        base.Start();
        if (DebugMode || isUnlocked)
        {
            Debug.Log("CompletingBookcase");
            onCompletePuzzle?.Invoke();
        }
    }

    private void OnDisable()
    {
        onCompletePuzzle -= TriggerBookCaseMovement;
    }

    //Member Methods
    void TriggerBookCaseMovement()
    {
        StartCoroutine(MoveBookCase());
        CompletePuzzle();
    }

    private IEnumerator MoveBookCase()
    {
        float dur = 5f;
        float t = 0f;

        Vector3 openPos = transform.position + -transform.right * pos_Offset;
        Vector3 startPos = transform.position;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPos, openPos, t);
            t += Time.deltaTime / dur;
            yield return null;
        }
    }
    void OnDrawGizmos()
    {
        Vector3 startPos = transform.position;
        Vector3 worldEndPosition = startPos + -transform.right * pos_Offset;

        // Draw a yellow sphere at the transform's position
        //Debug.Log("DrawingGizmo");
        Gizmos.color = Color.white;

        Gizmos.DrawLine(transform.position,
             worldEndPosition);
        Gizmos.DrawWireSphere(worldEndPosition, 0.1f);
    }


}
