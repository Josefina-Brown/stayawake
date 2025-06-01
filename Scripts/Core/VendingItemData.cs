// En un assembly compartido o uno que no cause dependencia cíclica
using UnityEngine;

public enum VendingItemType { Food, Money }

[System.Serializable]

public class VendingItemData
{
    public string itemName;
    public int ticketCost;
    public VendingItemType type;
    public int valueRestore;
    public Sprite icon;

    public AudioClip soundEffect;
}
