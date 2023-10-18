using UnityEngine;

public abstract class BasePuzzle : MonoBehaviour, IPuzzle
{
    public bool DebugMode { get; private set; }
    protected virtual void Awake()
    {
        DebugMode = GameManager.Instance.GetPuzzleDebugMode();
    }

    protected virtual void Start()
    {
        RegisterPuzzle();
    }

    public void RegisterPuzzle()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.puzzleManager.AddPuzzle(this);
        //Debug.Log($"Added {this.name}");
    }
    public virtual void CompletePuzzle()
    {
        UIController.togglePuzzleTask?.Invoke(this);
        Debug.Log($"{this.name} Puzzle Complete");
    }
}