using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
	[SerializeField]
	private PlayerController m_playerController;

	public PlayerController TargetPlayer { get { return m_playerController; } }

}
