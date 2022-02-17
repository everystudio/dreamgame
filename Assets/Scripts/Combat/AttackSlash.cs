using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlash : MonoBehaviour
{
	private Mover m_mover;
	private Vector2 m_direction;
	private void Awake()
	{
		m_mover = GetComponent<Mover>();
	}
	public IEnumerator Slash(Vector2 _dir)
	{
		m_direction = _dir;
		Debug.Log(_dir);
		yield return new WaitForSeconds(1.0f);
		Destroy(gameObject);
	}

	private void Update()
	{
		m_mover.Move(m_direction);
	}

}
