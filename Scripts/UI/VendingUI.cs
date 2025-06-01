using UnityEngine;
using UnityEngine.UI;
using System;

public class VendingUI : MonoBehaviour
{
    public GameObject panel;
    public Button itemButtonPrefab;
    public Transform itemButtonContainer;

    private Action<int> onBuy;

    public void OpenMenu(VendingItemData[] items, Action<int> buyCallback)
    {
        onBuy = buyCallback;

        panel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        foreach (Transform child in itemButtonContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            var item = items[i];

            Button btn = Instantiate(itemButtonPrefab, itemButtonContainer);

            var iconImage = btn.transform.Find("icon")?.GetComponent<Image>();
            if (iconImage != null) iconImage.sprite = item.icon;

            var nameText = btn.transform.Find("name")?.GetComponent<Text>();
            if (nameText != null) nameText.text = item.itemName;

            var priceText = btn.transform.Find("price")?.GetComponent<Text>();
            if (priceText != null) priceText.text = $"{item.ticketCost} tickets";

            btn.onClick.AddListener(() => BuyItem(index));
        }
    }

    private void BuyItem(int index)
    {
        onBuy?.Invoke(index);
        CloseMenu();
    }

    public void CloseMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        panel.SetActive(false);
    }
}
