using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using anogamelib;

public class MouseScroll : MonoBehaviour
{
    public EventIntListener int_listener;
	private GameInput m_gameInput;

	private void OnEnable()
	{
        if (m_gameInput == null)
        {
            m_gameInput = new GameInput();
        }
		m_gameInput.UI.ScrollWheel.performed += ScrollWheel_performed;
        m_gameInput.Enable();
    }

	private void ScrollWheel_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
	}

	private void OnDisable()
	{
        m_gameInput.Disable();
	}

	void Update()
    {
        Vector2 value = m_gameInput.UI.ScrollWheel.ReadValue<Vector2>();
        float mouseScrollDelta = value.y;

        if (mouseScrollDelta > 0)
        {
            int_listener.Dispatch(1);
        }
        else if (mouseScrollDelta < 0)
        {
            int_listener.Dispatch(-1);
        }
    }
}
