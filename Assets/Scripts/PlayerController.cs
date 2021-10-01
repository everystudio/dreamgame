using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using anogamelib;

public class PlayerController : StateMachineBase<PlayerController>
{
	private float m_fMovementSpeed = 3.0f;
	private Animator m_animator;

	private List<IMove2D> IMoveInterfaces = new List<IMove2D>();

	private void Start()
	{
		m_animator = GetComponent<Animator>();
		SetState(new PlayerController.Idle(this));

		GetComponentsInChildren<IMove2D>(true, IMoveInterfaces);
	}

	private void SetDir(Vector2 _dir)
	{
		m_animator.SetFloat("dir_x", _dir.normalized.x);
		m_animator.SetFloat("dir_y", _dir.normalized.y);
	}

	private void DispatchMoveEvent(Vector2 _direction, float _speed)
	{
		for (int i = 0; i < IMoveInterfaces.Count; i++)
		{
			IMoveInterfaces[i].OnMoveHandle(_direction, _speed);
		}
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

			Vector2 vec2MovementDir = m_gameInput.Player.Move.ReadValue<Vector2>();
			float fMoveLength = machine.m_fMovementSpeed * Time.deltaTime;
			machine.transform.Translate(vec2MovementDir.normalized * fMoveLength);

			machine.GetComponent<Rigidbody2D>().MovePosition((Vector2)machine.transform.position + (vec2MovementDir * fMoveLength));

			if (vec2MovementDir != Vector2.zero)
			{
				machine.SetDir(vec2MovementDir.normalized);
			}

			machine.DispatchMoveEvent(vec2MovementDir.normalized, fMoveLength);
		}

	}
}
