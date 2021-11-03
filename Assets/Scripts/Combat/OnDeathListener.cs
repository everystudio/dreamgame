using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathListener : MonoBehaviour,IKillable
{
    [SerializeField]
    private Health[] targets;

    [SerializeField]
    private float m_fDelay;

    public enum NotifyLocation { thisLocation, targetLocation };
    [SerializeField]
    private NotifyLocation location;

    [SerializeField]
    private UnityEventVector2 onDamaged;

    private void Awake()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].AddListener(this);
        }
    }

    public void OnDeath(Health _health)
    {
        StartCoroutine(DispatchAction(_health));
    }
    private IEnumerator DispatchAction(Health _health)
    {
        yield return (m_fDelay == 0f) ? null : new WaitForSeconds(m_fDelay);
        onDamaged.Invoke((location == NotifyLocation.thisLocation) ?
            this.transform.position : _health.transform.position);
    }
}


