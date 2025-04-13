using UnityEngine;
using UnityEngine.UI;

public class VendingUI : MonoBehaviour
{
    public GameObject panel; // el menú
    public Button itemButtonPrefab;
    public Transform itemButtonContainer;

    private VendingMachine currentMachine;

    public void OpenMenu(VendingMachine machine)
    {
        currentMachine = machine;
        panel.SetActive(true);

        Cursor.visible = true;      // Muestra el cursor.
        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor.

        foreach (Transform child in itemButtonContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < machine.items.Length; i++)
        {
            int index = i; // evitar problemas con el closure
            VendingItem item = machine.items[i];

            Button btn = Instantiate(itemButtonPrefab, itemButtonContainer);
            btn.GetComponentInChildren<Text>().text = $"{item.itemName} ({item.ticketCost} tks)";
            btn.onClick.AddListener(() => BuyItem(index));
        }
    }

    public void BuyItem(int index)
    {
        currentMachine.BuyItem(index);
        CloseMenu();
    }

    public void CloseMenu()
    {
        Cursor.visible = false;     
        Cursor.lockState = CursorLockMode.Confined; 
        panel.SetActive(false);
    }
}
