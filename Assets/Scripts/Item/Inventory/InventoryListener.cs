using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryListener : MonoBehaviour
{
    [SerializeField]
    private Inventory m_target;

    public Inventory Target
	{
		get { return m_target; }
		set
		{
			if(m_target != null && m_target != value)
			{

			}
			m_target = value;
			m_target?.AddListener(this.gameObject);
			m_target.ReloadAllItemSlots();
		}
	}

	private List<Inventory> m_obtainedInventories = new List<Inventory>();

	private void OnEnable()
	{
		if (m_target != null)
		{
			m_target?.AddListener(this.gameObject);
		}
	}
	private void OnDisable()
	{
		UnSubscribeToAllInventories();
	}
	private void UnSubscribeToAllInventories()
	{
		for (int i = 0; i < m_obtainedInventories.Count; i++)
		{
			m_obtainedInventories[i].RemoveListener(this.gameObject);
			m_obtainedInventories.Remove(m_obtainedInventories[i]);
		}

		if (m_target != null && this.gameObject != null)
		{
			m_target.RemoveListener(this.gameObject);
		}
	}
}
