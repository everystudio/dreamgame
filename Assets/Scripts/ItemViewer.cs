using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using UnityEngine.InputSystem;
// âºï\é¶ópèàóù
public class ItemViewer : StateMachineBase<ItemViewer>
{
	private GameInput m_gameInput;

	[SerializeField]
	private GameObject m_goInventoryRoot;

	private void Awake()
	{
		SetState(new ItemViewer.Wait(this));
	}

	private void OnEnable()
	{
		if(m_gameInput == null)
		{
			m_gameInput = new GameInput();
		}
		m_gameInput.Enable();
	}
	private void OnDisable()
	{
		m_gameInput.Disable();
	}

	private class Wait : StateBase<ItemViewer>
	{
		private bool m_bPushed = false;
		public Wait(ItemViewer _machine) : base(_machine)
		{
		}
		public override void OnUpdateState()
		{
			if(Mouse.current.rightButton.IsPressed())
			{
				m_bPushed = true;
			}
			if( m_bPushed && !Mouse.current.rightButton.IsPressed())
			{
				machine.SetState(new ItemViewer.Show(machine));
			}
		}
	}

	private class Show : StateBase<ItemViewer>
	{
		private bool m_bPushed = false;
		public Show(ItemViewer _machine) : base(_machine)
		{
		}
		public override void OnEnterState()
		{
			machine.m_goInventoryRoot.SetActive(true);
		}
		public override void OnUpdateState()
		{
			if (Mouse.current.rightButton.IsPressed())
			{
				m_bPushed = true;
			}
			if (m_bPushed && !Mouse.current.rightButton.IsPressed())
			{
				machine.SetState(new ItemViewer.Wait(machine));
			}
		}
		public override void OnExitState()
		{
			machine.m_goInventoryRoot.SetActive(false);
		}
	}
}
