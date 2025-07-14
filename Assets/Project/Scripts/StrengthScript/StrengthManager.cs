using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StrengthManager : MonoBehaviour
{
   [SerializeField] private Button HomeGoButton;

   [Header("強化画面UI")]
   [SerializeField] private TextMeshProUGUI coinsText; // 所持コイン表示用
   [SerializeField] private TextMeshProUGUI currentSpeedText; // 現在の速度表示用
   [SerializeField] private TextMeshProUGUI nextSpeedText; // 次のレベルの速度表示用
   [SerializeField] private TextMeshProUGUI costText; // 強化コスト表示
   [SerializeField] private Button upgradeSpeedButton; // 移動速度強化ボタン

    private void Start()
    {
        HomeGoButton.onClick.AddListener(OnHomeGoButtonClicked);
        upgradeSpeedButton.onClick.AddListener(OnUpgradeSpeedButtonClicked);
        UpdateUI();
    }

    private void OnHomeGoButtonClicked()
    {
        SceneManager.LoadScene("HomeSene");
    }

    private void OnUpgradeSpeedButtonClicked()
    {
        if (InGameManager.Instance != null)
        {
            bool success = InGameManager.Instance.UpgradePlayerMoveSpeed();
            if (success)
            {
                Debug.Log("移動速度の強化に成功しました！");
            }
            else
            {
                Debug.Log("コインが足りず、移動速度の強化に失敗しました。");
            }
            UpdateUI(); // 強化後にUIを更新
        }
        else
        {
            Debug.LogError("InGameManagerが見つかりません。強化できません。");
        }
    }

    /// <summary>
    /// 強化画面のUI表示を更新します。
    /// </summary>
    private void UpdateUI()
    {
        if (InGameManager.Instance == null)
        {
            Debug.LogError("InGameManagerがシーンに存在しません。強化画面UIを更新できません。", this);
            return;
        }

        // 所持コイン数の表示
        if (coinsText != null)
        {
            coinsText.text = "所持コイン: " + InGameManager.Instance.totalCoinsCollected.ToString();
        }

        // 現在の移動速度の表示
        if (currentSpeedText != null)
        {
            currentSpeedText.text = $"現在の速度: {InGameManager.Instance.GetActualPlayerMoveSpeed():F2}";
        }

        // 次の強化コストと次の速度の表示
        int nextCost = InGameManager.Instance.GetMoveSpeedUpgradeCost();
        float nextSpeed = InGameManager.Instance.basePlayerMoveSpeed + InGameManager.Instance.playerMoveSpeedLevel * InGameManager.Instance.moveSpeedPerLevel; // 次のレベルの速度

        if (costText != null)
        {
            costText.text = $"強化コスト: {nextCost} コイン";
            // コインが足りない場合はコストの色を変える
            costText.color = (InGameManager.Instance.totalCoinsCollected >= nextCost) ? Color.white : Color.red;
        }

        if (nextSpeedText != null)
        {
            nextSpeedText.text = $"次の速度: {nextSpeed:F2}";
        }

        // 強化ボタンの有効/無効化
        if (upgradeSpeedButton != null)
        {
            upgradeSpeedButton.interactable = (InGameManager.Instance.totalCoinsCollected >= nextCost);
        }
    }
}
