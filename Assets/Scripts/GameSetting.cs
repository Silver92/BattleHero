using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour {

    #region UI public field
    [Tooltip("Initial Panel")]
    public GameObject InitSubPanel;
    [Tooltip("Prestart Panel")]
    public GameObject StartSubPanel;
    [Tooltip("Option Panel")]
    public GameObject OptionSubPanel;
    
    [Tooltip("The inputfield for the username")]
    public InputField usernameInputField;
    [Tooltip("Sound checking toggle")]
    public Toggle soundToggle;
    #endregion

    #region General Game Management
    /// <summary>
    /// Starts the game fight.
    /// </summary>
    public void StartGame(){
        PlayerPrefs.SetString ("Username", usernameInputField.text);
        SceneManager.LoadScene("GamePlay");
    }

    /// <summary>
    /// Switchs the sound play.
    /// </summary>
    public void SwitchSound(){
        if (soundToggle.isOn) PlayerPrefs.SetInt ("SoundOn", 1);
        else PlayerPrefs.SetInt ("SoundOn", 0);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void ExitGame(){
        Application.Quit ();
    }
    #endregion

    #region UI Management
    /// <summary>
    /// Initialize the UI.
    /// </summary>
    void Start () {
        ActiveInitPanel ();
    }

    /// <summary>
    /// Actives the init panel only.
    /// </summary>
    public void ActiveInitPanel(){
        InitSubPanel.SetActive (true);
        StartSubPanel.SetActive (false);
        OptionSubPanel.SetActive (false);
    }

    /// <summary>
    /// Actives the start panel only.
    /// </summary>
    public void ActiveStartPanel(){
        InitSubPanel.SetActive (false);
        StartSubPanel.SetActive (true);
        OptionSubPanel.SetActive (false);
    }

    /// <summary>
    /// Actives the option panel only.
    /// </summary>
    public void ActiveOptionPanel(){
        InitSubPanel.SetActive (false);
        StartSubPanel.SetActive (false);
        OptionSubPanel.SetActive (true);
    }
    #endregion
    
}
