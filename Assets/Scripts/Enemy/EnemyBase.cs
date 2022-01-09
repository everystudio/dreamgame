using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class EnemyBase : StateMachineBase<EnemyBase>
{
	private void Start()
	{
		SetState(new EnemyBase.Idle(this));
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
