using UnityEngine;

[CreateAssetMenu(menuName ="Items/ItemCollection")]
public class ItemCollection : ScriptableObject
{
	[SerializeField]
	private InventoryItem[] items;

	public InventoryItem[] Items { get { return items; } }
}
