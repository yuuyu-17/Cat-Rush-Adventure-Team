using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultsManagement : MonoBehaviour
{
   [SerializeField] private Button HomeGoButton;

    void Start()
    {
        HomeGoButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("HomeSene");
    }
}
