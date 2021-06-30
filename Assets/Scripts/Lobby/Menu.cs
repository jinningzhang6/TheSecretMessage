using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public InputField NicknameInputField;
    public GameObject PopoverBackground;

    // Start is called before the first frame update
    void Start()
    {
        PopoverBackground.SetActive(false);
    }

    public void OnConfirmedNickName()
    {
        Debug.Log($"OnConfirmNicknameClicked: {NicknameInputField.text}");
        PlayerPrefs.SetString("usersname", NicknameInputField.text);
        SceneManager.LoadScene("GameLobby");
    }

    public void JoinGame()
    {
        PopoverBackground.SetActive(true);
    }

    public void HidePopOver()
    {
        PopoverBackground.SetActive(false);
    }

}
