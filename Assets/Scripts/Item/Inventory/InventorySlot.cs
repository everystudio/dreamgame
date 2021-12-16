using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, 
    IPointerDownHandler, 
    IDragHandler, 
    IPointerUpHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    IRecieveItemSlotIcon
{
    private int m_slotIndex;

    [System.Serializable]
    public class References
    {
        public TextMeshProUGUI SlotText;
        public TextMeshProUGUI AmountText;
        public Image Icon;
        public Image Highlight;
        public Inventory Inventory { get; set; }
        public GameObject iconLayer;
        public Slider energySlider;
    }

    [System.Serializable]
    public class Settings
    {
        public bool displaySlotNumber = true;
        public bool equipItemOnClick = true;
        public bool moveItemOnDrag = false;
        public int slotIndexOffset = 1;
        public bool hasSelectionHighlight = false;
    }

    [SerializeField]
    private References m_references = new References();

    [SerializeField]
    private Settings m_settings = new Settings();

    private bool m_initialized = false;
    private bool m_isDragging = false;
    private bool m_hasEnergySlider = false;

    public int GetSlotIndex()
    {
        if (!m_initialized)
        {
            Initialize();
            return m_slotIndex;
        }
        else
        {
            return m_slotIndex;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!m_initialized)
        {
            m_slotIndex = transform.GetSiblingIndex() + m_settings.slotIndexOffset;

            if (m_settings.displaySlotNumber)
            {
                m_references.SlotText.gameObject.SetActive(true);
                m_references.SlotText.text = (m_slotIndex + 1).ToString();
            }
            else
            {
                m_references.SlotText.gameObject.SetActive(false);
            }

            m_references.Icon.gameObject.SetActive(false);
            m_references.AmountText.gameObject.SetActive(false);
            m_references.energySlider.gameObject.SetActive(false);

            SetHighlighted(false);

            m_initialized = true;
        }
    }

    private void SetHighlighted(bool _bFlag)
    {
        if (_bFlag)
        {
            m_references.Icon.transform.localScale = Vector3.one;
        }
        else
        {
            m_references.Icon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        m_references.Highlight.gameObject.SetActive(_bFlag);
    }


    public void OnRemoveItem(int _iIndex)
    {
        if (_iIndex == m_slotIndex)
        {
            m_references.Icon.gameObject.SetActive(false);
            m_references.AmountText.gameObject.SetActive(false);
            m_references.energySlider.gameObject.SetActive(false);
        }
    }

    public void OnInventoryInitialized(Inventory _inventory)
    {
        m_references.Inventory = _inventory;
        Initialize();
    }

    public void OnUseItem(int _iIndex, ItemData _data, int _iAmount, int _iReduce = 1)
    {
        if (_iIndex == m_slotIndex && _data != null)
        {
            if (_data.CanStack)
            {
                m_references.AmountText.gameObject.SetActive(true);
                m_references.AmountText.text = _iAmount.ToString();
            }
            if (_data.HasEnergy)
            {
                UpdateEnergySlider(_iIndex);
            }
        }
    }

    private void UpdateEnergySlider(int _iIndex)
    {
        if (m_references.energySlider != null)
        {
            m_references.energySlider.enabled = true;

            ItemEnergy currentEnergy = m_references.Inventory.GetItem(_iIndex).Energy;

            m_references.energySlider.minValue = currentEnergy.min;
            m_references.energySlider.maxValue = currentEnergy.max;

            m_references.energySlider.value = currentEnergy.current;
        }
    }

    public void OnItemLoaded(int _iIndex, ItemData _data, int _iAmount)
    {
        if (_iIndex == m_slotIndex && _data != null)
        {
            if (_iAmount != 0)
            {
                m_references.AmountText.gameObject.SetActive(true);
                m_references.AmountText.text = _iAmount.ToString();
            }
            else
            {
                m_references.AmountText.gameObject.SetActive(false);
            }

            if (_data.Icon != null)
            {
                m_references.Icon.sprite = _data.Icon;
                m_references.Icon.gameObject.SetActive(true);
            }
            else
            {
                m_references.Icon.gameObject.SetActive(false);
            }

            if (_data.HasEnergy)
            {
                m_hasEnergySlider = true;
                m_references.energySlider.gameObject.SetActive(true);
                UpdateEnergySlider(_iIndex);
            }
            else
            {
                m_hasEnergySlider = false;
                m_references.energySlider.gameObject.SetActive(false);
            }
        }
    }

    // interfaces

    public void OnItemSelect(int _iIndex, bool _bSelected)
    {
        if (_iIndex == m_slotIndex)
        {
            if (m_settings.hasSelectionHighlight)
            {
                SetHighlighted(_bSelected);
            }
        }
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        //if (m_settings.equipItemOnClick)
        {
            if (m_references.Inventory != null)
            {
                m_references.Inventory.SelectItemByIndex(m_slotIndex);
            }
            return;
        }
    }

    public void OnDrag(PointerEventData _eventData)
    {
        if (m_settings.moveItemOnDrag)
        {
            if (m_isDragging == false)
            {
                if (m_references.iconLayer != null)
                {
                    Transform iconLayerTransform = m_references.iconLayer.transform;

                    m_references.Icon.transform.SetParent(iconLayerTransform);
                    m_references.AmountText.transform.SetParent(iconLayerTransform);
                }

                m_isDragging = true;
            }

            Vector3 movePoint = _eventData.position;

            m_references.Icon.transform.position = movePoint;
            m_references.AmountText.transform.position = movePoint;

            if (m_hasEnergySlider)
            {
                m_references.energySlider.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        if (m_isDragging)
        {
            OnDragStop();

            List<RaycastResult> raycastResult = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventData, raycastResult);

            for (int i = 0; i < raycastResult.Count; i++)
            {
                if (raycastResult[i].gameObject == this.gameObject)
                {
                    return;
                }
                IRecieveItemSlotIcon getInterface = raycastResult[i].gameObject.GetComponent<IRecieveItemSlotIcon>();

                if (getInterface != null)
                {
                    getInterface.OnRecieveItemIcon(m_slotIndex, m_references.Inventory);
                    return;
                }
            }

            m_references.Inventory.DropItem(m_slotIndex);
        }
    }

    private void OnDragStop()
    {
        m_references.AmountText.transform.SetParent(this.transform);
        m_references.AmountText.transform.localPosition = Vector2.zero;
        m_references.AmountText.transform.localScale = Vector3.one;

        m_references.Icon.transform.SetParent(this.transform);
        m_references.Icon.transform.localPosition = Vector2.zero;
        m_references.Icon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        if (m_hasEnergySlider)
        {
            m_references.energySlider.gameObject.SetActive(true);
        }

        m_isDragging = false;
    }

    public void OnDisable()
    {
        if (m_isDragging)
        {
            Invoke("OnDragStop", 0.01f);

            EventSystem getEventSystem = EventSystem.current;

            // Restart event system, could not find a more elegant solution.
            getEventSystem.enabled = false;
            getEventSystem.enabled = true;
        }
    }

    public void OnRecieveItemIcon(int _iIndex, Inventory _sourceInventory)
    {
        if (_sourceInventory != m_references.Inventory)
        {
            _sourceInventory.MoveItem(_iIndex, m_slotIndex, m_references.Inventory);
        }
        else
        {
            _sourceInventory.MoveItem(_iIndex, m_slotIndex);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Display item info
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide item info
    }
}
