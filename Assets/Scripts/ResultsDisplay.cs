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
    public LoadingIcon LoadingBar;

    public RectTransform ListDisplay;
    public DetailsDisplay DetailsDisplay;

    private List<ProgramEntry> _listItems = new List<ProgramEntry>();
    private ScrollRect _scrollView;
    private bool _scrollCooldown = false;

    private Screen _currentScreen = Screen.List;
    
    /// <summary>
    /// Display a list of ProgramEntry-objects in this ResultDisplay
    /// </summary>
    /// <param name="results">The list of ProgramEntry-objects to visualize</param>
    /// <param name="append">false = clear the list before loading new ones (now cleared in DisplayLoadingBar)</param>
    public void DisplayResults(List<ProgramEntry> results, bool append)
    {        
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

    /// <summary>
    /// Display an error (or information) message on this ResultsDisplay
    /// </summary>
    /// <param name="content">The text to be shown</param>
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

    /// <summary>
    /// Display a loading bar that reads the ProgramFetcher progress
    /// </summary>
    /// <param name="append">false = clear the list before adding the Loading bar</param>
    public void DisplayLoadingBar(bool append)
    {
        if (!append) ClearList();
        Instantiate<LoadingIcon>(LoadingBar, ContentContainer);
        if (append) _scrollView.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// React to the list being scrolled
    /// </summary>
    /// <param name="position">Current normalized position of the list</param>
    public void OnScroll(Vector2 position)
    {
        if (position.y <= 0) {
            FetchNext();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OnBackButtonPressed();
    }

    private void FetchNext()
    {
        if (_currentScreen == Screen.List && !_scrollCooldown && !ProgramFetcher.EndOfResults && !ProgramFetcher.SearchInProgress && _listItems.Count > 0)
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

    /// <summary>
    /// Populate the DetailsDisplay with the passed program's info
    /// </summary>
    /// <param name="program">Data to populate the DetailsDisplay with</param>
    public void ShowDetailsFor(ProgramEntry program)
    {
        DetailsDisplay.Display(program);
        ShowDetailsScreen();
    }

    /// <summary>
    /// Display the search results list screen
    /// </summary>
    public void ShowListScreen()
    {
        ShowScreen(Screen.List);
    }
    /// <summary>
    /// Display the program details screen
    /// </summary>
    public void ShowDetailsScreen()
    {
        ShowScreen(Screen.Details);
    }
    /// <summary>
    /// Display a screen matching the passed enumerator
    /// </summary>
    /// <param name="target">Screen to display</param>
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


    void OnBackButtonPressed()
    {
        if (_currentScreen != Screen.List)
        {
            ShowListScreen();
        } else
        {
            Application.Quit();
        }
    }
}
