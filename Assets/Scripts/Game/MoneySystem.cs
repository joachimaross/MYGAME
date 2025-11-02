using UnityEngine;
using TMPro;

/// <summary>
/// UI bridge for player money. If an EconomyManager is present it will use that singleton as the source of truth.
/// Otherwise it falls back to local money integer.
/// </summary>
public class MoneySystem : MonoBehaviour
{
    public int money = 100;
    public TMP_Text moneyText;

    void Start()
    {
        // If there's a centralized EconomyManager, prefer it
        var econ = MarketHustle.Economy.EconomyManager.Instance;
        if (econ != null)
        {
            // sync local money and subscribe to changes
            money = (int)econ.money;
            econ.OnMoneyChanged += OnGlobalMoneyChanged;
        }

        UpdateUI();
    }

    void OnDestroy()
    {
        var econ = MarketHustle.Economy.EconomyManager.Instance;
        if (econ != null) econ.OnMoneyChanged -= OnGlobalMoneyChanged;
    }

    void OnGlobalMoneyChanged(long newAmount)
    {
        money = (int)newAmount;
        UpdateUI();
    }

    public void AddMoney(int amount)
    {
        var econ = MarketHustle.Economy.EconomyManager.Instance;
        if (econ != null)
        {
            econ.AddMoney(amount);
            return;
        }

        money += amount;
        UpdateUI();
    }

    public void SpendMoney(int amount)
    {
        var econ = MarketHustle.Economy.EconomyManager.Instance;
        if (econ != null)
        {
            econ.TrySpend(amount);
            return;
        }

        money -= amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (moneyText != null) moneyText.text = "Money: $" + money.ToString();
    }
}
