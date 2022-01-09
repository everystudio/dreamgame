using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using anogamelib;
using System;
using Chronos;

public class PlayerController : StateMachineBase<PlayerController>
{
	private Animator m_animator;
	[SerializeField]
	private Mover m_mover;

	public GameObject m_prefAttack;

	private Timeline m_timelineField;

	//private List<IMove2D> IMoveInterfaces = new List<IMove2D>();

	private void Start()
	{
		m_animator = GetComponent<Animator>();
		SetState(new PlayerController.Idle(this));

		m_timelineField = GetComponent<Timeline>();
		if( m_timelineField == null)
		{
			m_timelineField = gameObject.AddComponent<Timeline>();
			m_timelineField.mode = TimelineMode.Global;
			m_timelineField.globalClockKey = "Field";
		}

		m_mover.SetTimeline(m_timelineField);

		//GetComponentsInChildren<IMove2D>(true, IMoveInterfaces);
	}

	private void SetDir(Vector2 _dir)
	{
		m_animator.SetFloat("dir_x", _dir.normalized.x);
		m_animator.SetFloat("dir_y", _dir.normalized.y);
	}

	private void DispatchMoveEvent(Vector2 _direction, float _speed)
	{
		/*
		for (int i = 0; i < IMoveInterfaces.Count; i++)
		{
			IMoveInterfaces[i].OnMoveHandle(_direction, _speed);
		}
		*/
	}

	private void CreateAttack(Vector2 _pos)
	{
		float angle = Vector2.SignedAngle(new Vector2(-1f, 0f), m_mover.Direction);
		AttackSlash script = Instantiate(m_prefAttack, new Vector3(_pos.x, _pos.y), Quaternion.AngleAxis(angle, new Vector3(0, 0, 1))).GetComponent<AttackSlash>();

		StartCoroutine(script.Slash(m_mover.Direction));
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
			m_gameInput.Player.Primary.performed += Primary_performed;

		}
		public override void OnUpdateState()
		{
			base.OnUpdateState();

			Vector2 vec2MovementDir = m_gameInput.Player.Move.ReadValue<Vector2>();

			if (vec2MovementDir != Vector2.zero)
			{
				machine.SetDir(vec2MovementDir.normalized);
			}
			machine.m_mover.Move(vec2MovementDir);
			//machine.DispatchMoveEvent(vec2MovementDir.normalized, fMoveLength);
		}

		private void Primary_performed(InputAction.CallbackContext obj)
		{
			Debug.Log(Mouse.current.position.ReadValue());
			Vector3 screenpos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,0f);
			Ray ray = Camera.main.ScreenPointToRay(screenpos);
			RaycastHit2D hit2d = Physics2D.Raycast(Mouse.current.position.ReadValue(), (Vector2)ray.direction);
			//Debug.Log(hit2d);
			//Debug.Log(hit2d.collider);
			if (hit2d.collider == null)
			{
				machine.SetState(new PlayerController.Attack(machine));
			}
		}

		public override void OnExitState()
		{
			m_gameInput.Player.Primary.performed -= Primary_performed;
		}
	}

	private class Attack : StateBase<PlayerController>
	{
		private AnimStateAttackEnd m_animStateAttackEnd;
		public Attack(PlayerController _machine) : base(_machine){}

		public override void OnEnterState()
		{
			machine.m_animator.SetTrigger("attack");

			machine.CreateAttack(machine.m_mover.transform.position);

			m_animStateAttackEnd = machine.m_animator.GetBehaviour<AnimStateAttackEnd>();
			m_animStateAttackEnd.OnAnimationEnd.AddListener(() => {
				machine.SetState(new PlayerController.Idle(machine));
			});
		}
		public override void OnExitState()
		{
			m_animStateAttackEnd.OnAnimationEnd.RemoveAllListeners();
		}
	}
}
