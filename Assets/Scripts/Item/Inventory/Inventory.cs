using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class Inventory : MonoBehaviour
{
    public Dictionary<int, InventoryEventDispatcher> m_eventDispatchers = new Dictionary<int, InventoryEventDispatcher>();

    [SerializeField]
    private int m_inventorySize = 30;
    public int InventorySize { get { return m_inventorySize; } }

    private int m_selectedSlotIndex = -1;
    public int SelectedSlotIndex { get { return m_selectedSlotIndex; } }

    [SerializeField]
    private SaveablePrefab m_droppableItemPrefab = null;

    private Dictionary<int, InventoryItem> m_items = new Dictionary<int, InventoryItem>();
	public Dictionary<int, InventoryItem> Items { get { return m_items; } }


    [SerializeField]
    private ItemCollection m_initialItems;

    private bool m_bInitialized;
	private bool m_isDirty;

	private void Start()
    {
        if( !m_bInitialized && m_initialItems != null)
		{
            foreach(InventoryItem item in m_initialItems.Items)
			{
                // アイテム追加todo
            }
            m_bInitialized = true;
		}
    }

    public void AddListener(GameObject _target)
    {
        int hashCode = _target.GetHashCode();
        if (!m_eventDispatchers.ContainsKey(hashCode))
        {
            m_eventDispatchers.Add(hashCode, new InventoryEventDispatcher(_target, this));
        }
    }
    public void RemoveListener(GameObject _target)
    {
        int hashCode = _target.GetHashCode();
        if (m_eventDispatchers.ContainsKey(hashCode))
        {
            m_eventDispatchers.Remove(hashCode);
        }
    }
    public void UseSelectedItem()
    {
        // todo
        //UseItem(m_selectedSlotIndex);
    }

    public void SelectItemByIndex(int _iSlotIndex)
    {
        if (_iSlotIndex < 0 || _iSlotIndex > m_inventorySize || _iSlotIndex == m_selectedSlotIndex)
        {
            return;
        }

        DeSelectItem(m_selectedSlotIndex);

        m_selectedSlotIndex = _iSlotIndex;

        foreach (var dispatcher in m_eventDispatchers.Values)
        {
            dispatcher.DispatchSelectItem(_iSlotIndex, true);
        }

        GetItem(_iSlotIndex)?.Data?.Action?.ItemActiveAction(this, _iSlotIndex);
    }

    public void SwitchItem(int _iDirection)
    {
        SelectItemByIndex(m_selectedSlotIndex + _iDirection);
    }

    public void DeSelectItem(int _iSlotIndex)
    {
        GetItem(_iSlotIndex)?.Data?.Action?.ItemUnactiveAction(this, _iSlotIndex);

        foreach (var dispatcher in m_eventDispatchers.Values)
        {
            dispatcher.DispatchSelectItem(_iSlotIndex, false);
        }
    }

    public InventoryItem GetItem(ItemData _itemData, out int _iIndex)
    {
        if (_itemData.HasSlot)
        {
            foreach (KeyValuePair<int, InventoryItem> getItem in m_items)
            {
                if (getItem.Value.Data == _itemData)
                {
                    _iIndex = getItem.Key;
                    return getItem.Value;
                }
            }
        }
        /*todo invisibleitem追加対応が必要
        else
        {
            for (int i = 0; i < invisibleItems.Count; i++)
            {
                if (invisibleItems[i].Data == _itemData)
                {
                    _iIndex = i;
                    return invisibleItems[i];
                }
            }
        }
        */
        _iIndex = -1;
        return null;
    }
    public InventoryItem GetItem(int _iIndex)
    {
        InventoryItem getItem;
        m_items.TryGetValue(_iIndex, out getItem);

        return getItem;
    }

    public void DropItem(int _slotIndex)
    {
        Debug.Log($"DropItem:{_slotIndex}");
        if (!GetItem(_slotIndex).Data.IsRemoveable)
        {
            return;
        }

        m_isDirty = true;

        LootableItem lootableItem = m_droppableItemPrefab.Retrieve<LootableItem>();
        //LootableItem lootableItem = Instantiate(m_prefabLootable).GetComponent<LootableItem>();

        if (lootableItem != null)
        {
            Vector2 aimDirection = Vector2.zero;
            /*
            Aimer getAimer = this.GetComponent<Aimer>();

            if (getAimer != null)
            {
                aimDirection = getAimer.GetAimDirection();
            }
            */

            lootableItem.transform.position = (Vector2)this.transform.position + (aimDirection * (lootableItem.PickupDistance() * 1.05f));
            lootableItem.Configure(GetItem(_slotIndex).Data, GetItem(_slotIndex).Amount , true);
            lootableItem.gameObject.SetActive(true);

            foreach (var dispatcher in m_eventDispatchers.Values)
            {
                dispatcher.DispatchDropItem(_slotIndex, lootableItem);
            }
        }

        RemoveItem(_slotIndex);
    }



    public void MoveItem(int _iSlotIndexOne, int _iSlotIndexTwo, Inventory _targetInventory = null)
    {
        m_isDirty = true;

        InventoryItem itemOne = GetItem(_iSlotIndexOne);
        InventoryItem itemTwo = (_targetInventory == null) ? GetItem(_iSlotIndexTwo) : _targetInventory.GetItem(_iSlotIndexTwo);

        bool itemOneValid = itemOne != null;
        bool itemTwoValid = itemTwo != null;

        // Item stacking
        if (itemOneValid && itemTwoValid)
        {
            //Debug.Log("itemOneValid && itemTwoValid");
            if (itemTwo != null)
            {
                if (itemTwo.Data.CanStack)
                {
                    if (itemTwo.Data == itemOne.Data)
                    {
                        RemoveItem(_iSlotIndexOne, true);

                        itemTwo.Amount += itemOne.Amount;
                        ReloadItemSlot(_iSlotIndexTwo);
                    }
                }
                return;
            }
        }

        if (itemOneValid)
        {
            //Debug.Log("itemOneValid");
            RemoveItem(_iSlotIndexOne, true);
        }

        if (itemTwoValid)
        {
            //Debug.Log("itemTwoValid");
            if (_targetInventory == null)
            {
                RemoveItem(_iSlotIndexTwo, true);
            }
            else
            {
                _targetInventory.RemoveItem(_iSlotIndexTwo, true);
            }
        }

        if (itemOneValid)
        {
            //Debug.Log("itemOneValid2");
            if (_targetInventory == null)
            {
                AddItem(itemOne.Data, itemOne.Amount, _iSlotIndexTwo, false);
            }
            else
            {
                _targetInventory.AddItem(itemOne.Data, itemOne.Amount, _iSlotIndexTwo, false);
            }
        }

        if (itemTwoValid)
        {
            //Debug.Log("itemTwoValid2");
            AddItem(itemTwo.Data, itemTwo.Amount, _iSlotIndexOne, false);
        }
    }

    public bool AddItem(ItemData _data, int amount, int slotIndex = -1, bool scanForStack = true)
    {
        m_isDirty = true;

        // Is item stackable? Then lets search the inventory first.
        if (scanForStack && _data.CanStack)
        {
            int index;
            InventoryItem getItem = GetItem(_data, out index);

            if (getItem != null)
            {
                Debug.Log(getItem);
                getItem.Amount += amount;
                foreach (var dispatcher in m_eventDispatchers.Values)
                {
                    dispatcher.DispatchItemLoad(index, getItem.Data, getItem.Amount);
                }

                return true;
            }
        }

        if (_data.HasSlot)
        {
            if (slotIndex == -1)
            {
                if (_data.HasSlot)
                {
                    for (int i = 0; i < m_inventorySize; i++)
                    {
                        if (!m_items.ContainsKey(i))
                        {
                            slotIndex = i;

                            break;
                        }
                    }
                }
            }

            if (slotIndex != -1)
            {
                InventoryItem newItem = new InventoryItem()
                {
                    Amount = (_data.CanStack) ? amount : 0,
                    Data = _data,
                    //Energy = _data.EnergyStartValue
                };

                m_items.Add(slotIndex, newItem);

                newItem.Data?.Action?.ItemAcquisitionAction(this, slotIndex);

                foreach (var dispatcher in m_eventDispatchers.Values)
                {
                    dispatcher.DispatchItemLoad(slotIndex, _data, (_data.CanStack) ? amount : 0);
                }

                return true;
            }
        }
        /*
        else
        {
            InventoryItem newItem = new InventoryItem()
            {
                Amount = (_data.CanStack) ? amount : 0,
                Data = _data,
                //Energy = _data.EnergyStartValue
            };

            //invisibleItems.Add(newItem);

            _data.Action?.ItemAcquisitionAction(this, -1);

            foreach (var dispatcher in eventDispatchers.Values)
            {
                dispatcher.DispatchItemLoad(-1, _data, (_data.CanStack) ? amount : 0);
            }

            return true;
        }
        */
        return false;
    }
    public void RemoveItem(int _iSlotIndex, bool m_bSwapItem = false)
    {
        if (!GetItem(_iSlotIndex).Data.IsRemoveable && !m_bSwapItem)
        {
            return;
        }
        m_isDirty = true;
        GetItem(_iSlotIndex)?.Data?.Action?.ItemRemoveAction(this, _iSlotIndex);

        m_items.Remove(_iSlotIndex);

        foreach (var dispatcher in m_eventDispatchers.Values)
        {
            dispatcher.DispatchRemoveItem(_iSlotIndex);
        }
    }
    public void ReduceItem(ItemData _itemData , int _iReduce)
    {
        int iIndex = 0;

        InventoryItem inventoryItem = GetItem(_itemData, out iIndex);

        if (inventoryItem == null )
        {
            return;
        }

        if( inventoryItem.Amount == _iReduce)
        {
            RemoveItem(iIndex);
        }
        else if(_iReduce < inventoryItem.Amount)
        {
            m_isDirty = true;
            inventoryItem.Amount -= _iReduce;
            foreach (var dispatcher in m_eventDispatchers.Values)
            {
                dispatcher.DispatchItemLoad(iIndex, _itemData, inventoryItem.Amount);
            }
        }
    }



    public void ReloadItemSlot(int _iSlotIndex)
    {
        InventoryItem item = GetItem(_iSlotIndex);

        bool foundItem = item != null;

        foreach (var dispatcher in m_eventDispatchers.Values)
        {
            if (foundItem)
            {
                dispatcher.DispatchItemLoad(_iSlotIndex, item.Data, item.Amount);
            }
            else
            {
                dispatcher.DispatchRemoveItem(_iSlotIndex);
            }
        }
    }
    public void ReloadItem(InventoryItem _inventoryItem)
    {
    }

    public void ReloadAllItemSlots()
    {
        for (int i = 0; i < m_inventorySize; i++)
        {
            ReloadItemSlot(i);
        }
    }

    public bool HasCheckWithAmount(ItemData _itemData, int _iAmount)
    {
        int itemIndex = 0;
        InventoryItem inventoryItem = GetItem(_itemData, out itemIndex);
        if (inventoryItem != null)
        {
            if (_iAmount <= inventoryItem.Amount)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckReicpe(RecipeModel _recipe)
    {
        foreach(RecipeRequireItem req in _recipe.requireItems)
        {
            if(!HasCheckWithAmount(req.itemData, req.amount))
            {
                return false;
            }
        }
        return true;
    }


    #region Save Region

    [System.Serializable]
    public struct InventoryItemSave
    {
        public int index;
        public string guidString;   // これ多分使わなくなる
        public int amount;
        public ItemEnergy energy;
    }

    [System.Serializable]
    public struct InventorySaveData
    {
        public bool initialized;
        public InventoryItemSave[] savedItems;
    }

    public InventorySaveData inventorySaveData;

    public string OnSave()
    {
        inventorySaveData = new InventorySaveData()
        {
            initialized = m_bInitialized,
            savedItems = new InventoryItemSave[m_items.Count]
        };

        int counter = 0;

        foreach (KeyValuePair<int, InventoryItem> item in m_items)
        {
            inventorySaveData.savedItems[counter] = new InventoryItemSave()
            {
                index = item.Key,
                guidString = item.Value.Data.GetGuid(),
                amount = item.Value.Amount,
                energy = item.Value.Energy
            };
            counter++;
        }

        return JsonUtility.ToJson(inventorySaveData);
    }

    public void OnLoad(string _data)
    {
        inventorySaveData = JsonUtility.FromJson<InventorySaveData>(_data);

        if (inventorySaveData.savedItems.Length != 0)
        {
            m_items.Clear();

            for (int i = 0; i < inventorySaveData.savedItems.Length; i++)
            {
                InventoryItemSave getSave = inventorySaveData.savedItems[i];

                ItemData getItemData = ScriptableAssetDatabase.GetAsset(getSave.guidString) as ItemData;
                //ItemData getItemData = getSave.itemData;
                //ItemData getItemData = null;
                if (getItemData != null)
                {
                    AddItem(getItemData, getSave.amount, getSave.index);

                    // TODO: Create cleaner way to modify additional data in items.
                    if (getItemData.HasEnergy)
                    {
                        GetItem(getSave.index).Energy = getSave.energy;
                        ReloadItemSlot(getSave.index);
                    }
                }
                else
                {
                    Debug.Log($"Attempted to obtain guid: {getSave.guidString}");
                }
            }

            m_bInitialized = inventorySaveData.initialized;
        }
    }

    public bool OnSaveCondition()
    {
        if (m_isDirty)
        {
            m_isDirty = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
