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
    public void UpdateSearchInput()
    {
        ProgramFetcher.SearchInput = _input.text;
    }
}
