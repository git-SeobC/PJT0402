using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventoryItem> inventory = new List<InventoryItem>(); //아이템리스트

    private string savePath => Application.persistentDataPath + "/SvaeData.json"; //저장 파일 경로

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            LoadInventory(); //게임 시작 시 인벤토리 불러오기
        }
    }

    public void AddItem(InventoryItem item) //아이템 추가
    {
        inventory.Add(item);
        InventoryUI.Instance.UpdateInventoryUI();
        SaveInventory(); //저장
    }

    public void SaveInventory() //인벤토리 저장(Json 사용)
    {
        string json = JsonUtility.ToJson(new InventorySaveWrapper(inventory), true);
        File.WriteAllText(savePath, json);

    }

    public void LoadInventory() //인벤토리 불러오기
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            InventorySaveWrapper data = JsonUtility.FromJson<InventorySaveWrapper>(json);
            inventory = data.items;
        }
    }

    [System.Serializable]
    private class InventorySaveWrapper //Json 저장을위한 감싸는 클래스
    {
        public List<InventoryItem> items;

        public InventorySaveWrapper(List<InventoryItem> itemList)
        {
            this.items = itemList;
        }
    }
}