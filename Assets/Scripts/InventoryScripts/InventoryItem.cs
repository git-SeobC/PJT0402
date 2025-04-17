using UnityEngine;


public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    KeyItem1,
    KeyItem2
}

public class InventoryItem : MonoBehaviour
{
    public string itemName; //아이템이름
    public string description; //아이템 설명*
    public ItemType itemType; //아이템타입
    public Sprite icon; //아이콘 이미지

    public int attackPower;
    public int defensePower;
    public int healAmount;

    private void Start()
    {
        SetDefaultByType();
    }

    private void SetDefaultByType()
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                itemName = "기본 검";
                description = "공격력 + 5";
                attackPower = 5;
                icon = Resources.Load<Sprite>("Icons/weapon_icon");
                break;
            case ItemType.Armor:
                itemName = "가죽 방어구";
                description = "방여력 + 5";
                defensePower = 3;
                icon = Resources.Load<Sprite>("Icons/armor_icon");
                break;
            case ItemType.Potion:
                itemName = "회복 포션";
                description = "체력 + 20 회복";
                healAmount = 20;
                icon = Resources.Load<Sprite>("Icons/potion_icon");
                break;
            case ItemType.KeyItem1:
                itemName = "문1, 문2 열쇠";
                icon = Resources.Load<Sprite>("Icons/key1_icon");
                break;
            case ItemType.KeyItem2:
                itemName = "문3, 문4 열쇠";
                icon = Resources.Load<Sprite>("Icons/key2_icon");
                break;


        }
    }

}