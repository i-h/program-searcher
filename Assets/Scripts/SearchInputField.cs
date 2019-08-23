using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class SearchInputField : MonoBehaviour
{
    private InputField _input;
    private void Awake()
    {
        _input = GetComponent<InputField>();
    }
    /// <summary>
    /// Update the contents of this InputField to the ProgramFetcher.SearchInput variable
    /// </summary>
    public void UpdateSearchInput()
    {
        ProgramFetcher.SearchInput = _input.text;
    }
}
