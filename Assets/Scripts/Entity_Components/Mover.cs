using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;
using Chronos;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour , ISaveable
{
	[SerializeField]
	private float m_fSpeed;

	private List<IMove2D> IMoveInterfaces = new List<IMove2D>();
	private List<IFreezeMovement> IFreezeMovementInterfaces = new List<IFreezeMovement>();

	private Rigidbody2D m_rb;

	private bool m_isMovementFrozen;
	public bool IsMovementFrozen { get { return m_isMovementFrozen; } }

	[SerializeField]
	private Vector2 m_direction;
	public Vector2 Direction { get { return m_direction; } }

	private Timeline m_timeline;
	public void SetTimeline(Timeline _timeline)
	{
		m_timeline = _timeline;
	}

	private void Awake()
	{
		GetComponentsInChildren<IMove2D>(true, IMoveInterfaces);
		GetComponentsInChildren<IFreezeMovement>(true, IFreezeMovementInterfaces);
		m_rb = GetComponent<Rigidbody2D>();

		DispatchMoveEvent(Vector2.zero, 0);
	}

	public void Move(Vector2 _direction)
	{
		if (m_isMovementFrozen)
		{
			return;
		}

		_direction.Normalize();

		float deltaTime = Time.deltaTime;
		if (m_timeline != null)
		{
			deltaTime = m_timeline.deltaTime;
		}
		transform.Translate(((_direction * m_fSpeed) * deltaTime), Space.World);

		if (!(_direction.x == 0f && _direction.y == 0f))
		{
			m_direction = _direction;
		}

		DispatchMoveEvent(_direction, (_direction.x == 0 && _direction.y == 0) ? 0 : m_fSpeed);
	}
	public void SetPosition(Vector2 _position)
	{
		m_rb.MovePosition(_position);
		m_direction = Vector2.zero;

		DispatchMoveEvent(Vector2.zero, 0);
	}

	private void DispatchMoveEvent(Vector2 _direction, float _speed)
	{
		for (int i = 0; i < IMoveInterfaces.Count; i++)
		{
			IMoveInterfaces[i].OnMoveHandle(_direction, _speed);
		}
	}

	private void DispatchFreezeEvent(bool _isFrozen)
	{
		for (int i = 0; i < IFreezeMovementInterfaces.Count; i++)
		{
			IFreezeMovementInterfaces[i].OnMovementFrozen(_isFrozen);
		}
	}


	#region Saving(ISaveŽÀ‘•Žž‚É—LŒø)
	private Vector2 m_lastSavedPosition;

	[System.Serializable]
	public struct SaveData
	{
		public Vector2 position;
	}

	public string OnSave()
	{
		m_lastSavedPosition = this.transform.position;

		return JsonUtility.ToJson(new SaveData()
		{
			position = m_lastSavedPosition
		});
	}

	public void OnLoad(string data)
	{
		SaveData saveData = JsonUtility.FromJson<SaveData>(data);
		Vector2 newPosition = saveData.position;
		this.transform.position = newPosition;
		if (m_rb.bodyType != RigidbodyType2D.Static)
		{
			m_rb.MovePosition(newPosition);
		}

		DispatchMoveEvent(Vector2.zero, 0);
		m_lastSavedPosition = newPosition;
	}
	public bool OnSaveCondition()
	{
		return m_lastSavedPosition != (Vector2)this.transform.position;
	}

	#endregion
}
