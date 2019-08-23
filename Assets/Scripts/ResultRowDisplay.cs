using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRowDisplay : MonoBehaviour
{
    public Text Title;
    public Text Description;

    private ProgramEntry _entry;

    public void Show(ProgramEntry program)
    {
        _entry = program;
        if (Title != null) Title.text = _entry.TitleFinnish;
        if (Description != null) Description.text = _entry.Description;
    }
    public void Expand()
    {
        ResultsDisplay.Instance.ShowDetailsScreen();
    }

    public void Collapse()
    {
        ResultsDisplay.Instance.ShowListScreen();
    }
}
