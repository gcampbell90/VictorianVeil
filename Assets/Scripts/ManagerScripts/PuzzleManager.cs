using System.Collections.Generic;

public class PuzzleManager
{
    public List<BasePuzzle> _puzzles = new List<BasePuzzle>();

    internal void AddPuzzle(BasePuzzle puzzle)
    {
        _puzzles.Add(puzzle);
        UIController.addPuzzleTask(puzzle);
    }
}
