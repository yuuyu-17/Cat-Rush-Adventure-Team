using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManagement : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("HomeSene");
    }
}
