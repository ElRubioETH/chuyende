using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class Selection_Manager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Button buttonMale;
    public Button buttonFemale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonFemale.onClick.AddListener(() => OnButtonClick("Female"));
        buttonMale.onClick.AddListener(() => OnButtonClick("Male"));


    }

    // Update is called once per frame
    void OnButtonClick(string playerClass)
    {
        //doc thong tion ng chs 
        var playerName = nameInputField.text;
        //luu thong tin ng chs
        PlayerPrefs.SetString("PlayerName",playerName );
        PlayerPrefs.SetString("PlayerClass",playerClass );
        SceneManager.LoadScene("main");
    }
}
