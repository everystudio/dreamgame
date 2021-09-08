using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using anogamelib;

public class PlayerController : StateMachineBase<PlayerController>
{
	private void Start()
	{
		SetState(new PlayerController.Idle(this));
	}

	private class Idle : StateBase<PlayerController>
	{
		private GameInput m_gameInput;
		public Idle(PlayerController _machine) : base(_machine)
		{
		}
		public override void OnEnterState()
		{
			base.OnEnterState();
			m_gameInput = new GameInput();
			m_gameInput.Enable();
		}
		public override void OnUpdateState()
		{
			base.OnUpdateState();

			machine.transform.Translate(m_gameInput.Player.Move.ReadValue<Vector2>()*Time.deltaTime);
		}

	}
}
