using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManagement : MonoBehaviour
{
    [SerializeField] private Button InGameButton;
    [SerializeField] private Button StrengthButton;
    [SerializeField] private Button RibraryButton;

    void Start()
    {
        InGameButton.onClick.AddListener(OnInGameClicked);
        StrengthButton.onClick.AddListener(OnStrengthClicked);
        RibraryButton.onClick.AddListener(OnRibraryClicked);
    }

    private void OnInGameClicked()
    {
        Debug.Log("InGame");
        SceneManager.LoadScene("InGameSene");
    }

    private void OnStrengthClicked()
    {
        Debug.Log("Strength");
        SceneManager.LoadScene("StrengthSene");
    }

    private void OnRibraryClicked()
    {
        Debug.Log("Ribrary");
        SceneManager.LoadScene("RibrarySene");
    }
}
