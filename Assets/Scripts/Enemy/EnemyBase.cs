using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using Chronos;

public class EnemyBase : StateMachineBase<EnemyBase>
{
	private GlobalClock m_clock;
	private Timeline m_timeLine;

	public EventGameObject m_begenCombat;
	[SerializeField]
	private EventInt begentest2;

	private void Start()
	{
		SetState(new EnemyBase.Idle(this));
	}

	private void OnCollisionEnter2D(Collision2D _collision)
	{
		if (_collision.gameObject.CompareTag("Player")){
			Debug.Log("hit_player");
			m_begenCombat.Invoke(gameObject);
			begentest2.Invoke(0);
		}
	}


	private class Idle : StateBase<EnemyBase>
	{
		public float wait_time;
		public Idle(EnemyBase _machine) : base(_machine)
		{
		}
		public override void OnUpdateState()
		{
			//wait_time += 
		}
	}
}
