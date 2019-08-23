using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultRowDisplay : MonoBehaviour
{
    public Text Title;
    public Text Description;

    private ProgramEntry _entry;

    /// <summary>
    /// Populate this ResultRowDisplay with the passed program data
    /// </summary>
    /// <param name="program">Data to fill this ResultRowDisplay with</param>
    public void Show(ProgramEntry program)
    {
        _entry = program;
        if (Title != null) Title.text = _entry.TitleFinnish;
        if (Description != null) Description.text = _entry.Description;
    }
    /// <summary>
    /// Display details of this program
    /// </summary>
    public void Expand()
    {
        ResultsDisplay.Instance.ShowDetailsFor(_entry);
    }
}
