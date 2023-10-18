using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform _puzzleTaskListTransform;
    [SerializeField] private GameObject _puzzleTaskPrefab;

    [SerializeField] private TextMeshProUGUI _sceneLoadName;
    [SerializeField] private Slider _sceneLoadSlider;

    private Dictionary<BasePuzzle, Toggle> _taskList = new Dictionary<BasePuzzle, Toggle>();

    public delegate void LoadSceneProgress(float progress);
    public static LoadSceneProgress onLoadScene;

    public delegate void AddTask(BasePuzzle puzzle);
    public static AddTask addPuzzleTask;

    public delegate void ToggleAction(BasePuzzle puzzle);
    public static ToggleAction togglePuzzleTask;

    private void OnEnable()
    {
        onLoadScene += ProgressUpdate;
        addPuzzleTask += AddPuzzleTaskToUI;
        togglePuzzleTask += CompletePuzzle;

        GameManager.Instance.onGameQuit += ClearPuzzleList;
    }

    private void OnDisable()
    {
        onLoadScene -= ProgressUpdate;
        addPuzzleTask -= AddPuzzleTaskToUI;
        togglePuzzleTask -= CompletePuzzle;

        GameManager.Instance.onGameQuit -= ClearPuzzleList;

    }

    private void ProgressUpdate(float progress)
    {
        _sceneLoadSlider.value = progress;
    }

    private void AddPuzzleTaskToUI(BasePuzzle puzzle)
    {
        var m_puzzleTask = Instantiate(_puzzleTaskPrefab, _puzzleTaskListTransform);
        m_puzzleTask.GetComponentInChildren<TextMeshProUGUI>().text = puzzle.name;
        var toggle = m_puzzleTask.GetComponentInChildren<Toggle>();
        _taskList.Add(puzzle, toggle);
    }

    public void CompletePuzzle(BasePuzzle puzzle)
    {
        _taskList.TryGetValue(puzzle, out Toggle toggle);
        toggle.isOn = true;
    }

    private void ClearPuzzleList()
    {
        foreach (Transform item in _puzzleTaskListTransform.GetComponentInChildren<Transform>())
        {
            Destroy(item.gameObject);
        }
        _taskList.Clear();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
