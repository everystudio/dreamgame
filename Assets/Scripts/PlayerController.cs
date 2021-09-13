using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using anogamelib;

public class PlayerController : StateMachineBase<PlayerController>
{
	private float m_fMovementSpeed = 3.0f;
	private Animator m_animator;
	private void Start()
	{
		m_animator = GetComponent<Animator>();
		SetState(new PlayerController.Idle(this));
	}

	private void SetDir(Vector2 _dir)
	{
		m_animator.SetFloat("dir_x", _dir.normalized.x);
		m_animator.SetFloat("dir_y", _dir.normalized.y);
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

			Vector2 vec2Movement = m_gameInput.Player.Move.ReadValue<Vector2>();
			machine.transform.Translate(vec2Movement * machine.m_fMovementSpeed * Time.deltaTime);
			if (vec2Movement != Vector2.zero)
			{
				machine.SetDir(vec2Movement);
			}
		}

	}
}
