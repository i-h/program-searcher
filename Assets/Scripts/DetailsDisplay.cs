using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DetailsDisplay : MonoBehaviour
{
    public Text Title;
    public Text Description;
    public Text ContentRating;
    public Text CountryOfOrigin;
    public Text MediaType;
    public string URL;

    ProgramEntry _currentProgram;
    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Display(ProgramEntry program)
    {
        _currentProgram = program;

        Title.text = program.TitleFinnish;
        Description.text = program.Description;
        ContentRating.text = program.ContentRating;
        CountryOfOrigin.text = program.CountryOfOrigin;
        URL = GetProgramURL(program.ID);
        MediaType.text = GetMediaType(program.MediaType);

    }

    private string GetMediaType(string typeMediaKeyword)
    {
        switch (typeMediaKeyword)
        {
            case "TVContent":
                return "TV";
            case "RadioContent":
                return "Radio";
            default:
                return typeMediaKeyword;
        }
    }

    private string GetProgramURL(string ID)
    {
        return "https://areena.yle.fi/" + ID;
    }

    public void OpenLink()
    {
        Application.OpenURL(URL);
    }

    public RectTransform GetRectTransform()
    {
        return _rectTransform;
    }
}
