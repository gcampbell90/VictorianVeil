using UnityEngine;

public abstract class BasePuzzle : MonoBehaviour, IPuzzle
{
    [field: SerializeField]public bool DebugMode { get; private set; }
    protected virtual void Awake()
    {
        if (DebugMode != GameManager.Instance.DebugMode) return; //Should be false on start, unless enabled by dev
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