using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ResultsDisplay : MonoBehaviour
{
    public enum Screen { None, List, Details }

    #region Singleton

    public static ResultsDisplay Instance {
        get
        {
            if(_instance == null)
            {
                Debug.LogError("Results display is not set up! Attach ResultsDisplay script to a Scroll View");                
            }
            return _instance;
        }
    }
    private static ResultsDisplay _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("You have an extra ResultsDisplay script attached to " + name + ".\nDestroying extra script!");
            Destroy(this);
        }

        _scrollView = GetComponent<ScrollRect>();
        ShowListScreen();
        ClearList();
    }

    #endregion

    public RectTransform ContentContainer;
    public ResultRowDisplay ResultRow;
    public Text ErrorRow;

    public RectTransform ListDisplay;
    public DetailsDisplay DetailsDisplay;

    private List<ProgramEntry> _listItems = new List<ProgramEntry>();
    private ScrollRect _scrollView;
    private bool _scrollCooldown = false;

    private Screen _currentScreen = Screen.List;

    public void DisplayResults(List<ProgramEntry> results, bool append)
    {
        if(!append) ClearList();
        foreach (ProgramEntry entry in results)
        {
            if (!_listItems.Contains(entry))
            {
                _listItems.Add(entry);
                ResultRowDisplay row = Instantiate<ResultRowDisplay>(ResultRow, ContentContainer);
                row.Show(entry);
            }
        }

        ActivateScrollCooldown();
    }

    public void DisplayError(string content)
    {
        ClearList();
        Text errorTxt = Instantiate<Text>(ErrorRow, ContentContainer);
        errorTxt.text = content;

    }

    private void ClearList()
    {
        for (int i = 0; i < ContentContainer.childCount; i++)
        {
            Destroy(ContentContainer.GetChild(i).gameObject);
        }
        _listItems.Clear();
            
    }

    public void DisplayLoadingBar(bool append)
    {
        Debug.Log("Loading!");
    }

    public void OnScroll(Vector2 position)
    {
        if (position.y <= 0) {
            FetchNext();
        }
    }

    private void Update()
    {
        //Debug.Log(_scrollView.verticalNormalizedPosition);
        if(_scrollView.verticalNormalizedPosition <= 0) {
            //FetchNext();
        }
    }

    private void FetchNext()
    {
        if (_currentScreen == Screen.List && !_scrollCooldown && !ProgramFetcher.SearchInProgress && _listItems.Count > 0)
        {
            ActivateScrollCooldown();

            Debug.Log("Bottom");
            ProgramFetcher.Instance.SearchNext();
        }
    }

    void ActivateScrollCooldown()
    {
        _scrollCooldown = true;
        Invoke("DisableScrollCooldown", 0.1f);
    }

    void DisableScrollCooldown()
    {
        _scrollCooldown = false;
    }

    public void ShowDetailsFor(ProgramEntry program)
    {
        DetailsDisplay.Display(program);
        ShowDetailsScreen();
    }

    public void ShowListScreen()
    {
        ShowScreen(Screen.List);
    }
    public void ShowDetailsScreen()
    {
        ShowScreen(Screen.Details);
    }
    public void ShowScreen(Screen target)
    {
        switch (target)
        {
            case Screen.List:
                ListDisplay.gameObject.SetActive(true);
                DetailsDisplay.gameObject.SetActive(false);
                _scrollView.content = ListDisplay;
                break;
            case Screen.Details:
                ListDisplay.gameObject.SetActive(false);
                DetailsDisplay.gameObject.SetActive(true);
                _scrollView.content = DetailsDisplay.GetRectTransform();
                break;
            case Screen.None:
                ListDisplay.gameObject.SetActive(false);
                DetailsDisplay.gameObject.SetActive(false);
                break;
        }
        _currentScreen = target;
    }
}
