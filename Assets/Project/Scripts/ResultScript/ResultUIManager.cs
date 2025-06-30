using UnityEngine;
using TMPro;

public class ResultUIManager : MonoBehaviour
{
    [Header("UI要素")]
    public TextMeshProUGUI coinsText; // コイン表示用TextをInspectorから割り当てる
    public TextMeshProUGUI[] itemTexts; // アイテム表示用TextをInspectorから割り当てる
    public TextMeshProUGUI totalDistanceText; // 総移動距離表示用TextをInspectorから割り当てる

    void Start()
    {
        DisplayResults();
    }

    private void DisplayResults()
    {
        if (InGameManager.Instance == null)
        {
            Debug.LogError("InGameManagerがシーンに存在しません。結果を表示できません。", this);
            return;
        }

        // コイン数の表示
        if (coinsText != null)
        {
            coinsText.text = "コイン: " + InGameManager.Instance.totalCoinsCollected.ToString();
        }

        // 獲得アイテムの表示
        int textIndex = 0;
        foreach (var itemEntry in InGameManager.Instance.collectedItems)
        {
            if (itemEntry.Value > 0) // 1個以上獲得したアイテムのみ表示
            {
                if (textIndex < itemTexts.Length)
                {
                    // ItemTypeのEnum名を文字列に変換し、スペースを加えて表示
                    string itemName = itemEntry.Key.ToString();
                    // 例: "HealthPotion" -> "Health Potion"
                    itemName = System.Text.RegularExpressions.Regex.Replace(itemName, "([a-z])([A-Z])", "$1 $2");

                    itemTexts[textIndex].text = $"{itemName}: {itemEntry.Value} 個";
                    textIndex++;
                }
                else
                {
                    Debug.LogWarning("アイテム表示用Textが足りません。");
                }
            }
        }

        // 残りのアイテム表示用Textを非表示にする（初期化時など）
        for (int i = textIndex; i < itemTexts.Length; i++)
        {
            if (itemTexts[i] != null)
            {
                itemTexts[i].text = ""; // またはgameObject.SetActive(false);
            }
        }

        //1ゲームでの総移動距離の表示
        if (totalDistanceText != null)
        {
            totalDistanceText.text = $"進行距離: {InGameManager.Instance.currentRunDistanceTraveled:F2} m";
        }
    }
}
