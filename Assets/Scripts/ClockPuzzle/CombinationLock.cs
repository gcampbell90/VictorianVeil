using System;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    [Header("Combination Passcode")]
    [SerializeField] private CombinationPasscode combination;

    public delegate void SetCogNumber(XRCogWheelInteractable xrCog);
    public static SetCogNumber setCogNumber;

    public delegate void CogWheelCompletedEventHandler();
    public event CogWheelCompletedEventHandler OnCogWheelCompleted;

    private void OnEnable()
    {
        setCogNumber += SetCog;
    }

    private void OnDisable()
    {
        setCogNumber -= SetCog;
    }

    void SetCog(XRCogWheelInteractable xrCog)
    {
        CheckCombo();
    }

    void CheckCombo()
    {
        var cogs = transform.GetComponentsInChildren<XRCogWheelInteractable>();

        bool cog1 = false, cog2 = false, cog3 = false, cog4 = false;
        if (cogs[0].currNum == combination.digit1)
        {
            Debug.Log("Cog1 Correct");
            cog1 = true;
        }
        if (cogs[1].currNum == combination.digit2)
        {
            Debug.Log("Cog2 Correct");
            cog2 = true;
        }
        if (cogs[2].currNum == combination.digit3)
        {
            Debug.Log("Cog3 Correct");
            cog3 = true;
        }
        if (cogs[3].currNum == combination.digit4)
        {
            Debug.Log("Cog4 Correct");
            cog4 = true;
        }

        if (cog1 && cog2 && cog3 && cog4)
        {
            CompletePuzzle();
        }
    }

    private void CompletePuzzle()
    {
        Debug.Log("Puzzle Completed");
        OnCogWheelCompleted?.Invoke();
    }
}

[System.Serializable]
public struct CombinationPasscode
{
    [Range(0, 9)]
    public int digit1;
    [Range(0, 9)]
    public int digit2;
    [Range(0, 9)]
    public int digit3;
    [Range(0, 9)]
    public int digit4;
}