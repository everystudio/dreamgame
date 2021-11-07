using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVolume : MonoBehaviour
{
	//[SerializeField]
	//private BoxCollider2D m_collider;

    [SerializeField]
    private string[] m_damageableTags;

    [SerializeField]
	private float m_fDamage;

	private List<int> m_attackedInstances = new List<int>();

	private IDamageCallback m_callbackInterface;

	protected bool m_canDealDamage = true;

	[SerializeField]
	private bool m_canDoDuplicateDamage = false;

	[SerializeField]
	private bool m_canDamageMultiple = true;

	public GameObject m_owner;

    public void Configure(DamageVolumeConfiguration config)
    {
        m_owner = config.Owner;
        m_fDamage = config.Damage;
        m_damageableTags = config.TargetTags;
        //m_collider.size = new Vector2(config.Size.x, config.Size.y);
        m_canDoDuplicateDamage = config.AllowDuplicateDamage;
        m_canDamageMultiple = config.CanDamageMultiple;

        m_attackedInstances.Clear();
        m_canDealDamage = true;

        this.gameObject.SetActive(true);

        if (config.ActiveTime != 0)
        {
            StartCoroutine(DisableDamageAfterTime(config.ActiveTime));
        }
    }
    public void OnEnable()
    {
        m_attackedInstances.Clear();
    }

    public void SetCallBack(IDamageCallback _callbackInterface)
    {
        m_callbackInterface = _callbackInterface;
    }

    protected IEnumerator DisableDamageAfterTime(float _disableTime)
    {
        yield return new WaitForSeconds(_disableTime);
        this.gameObject.SetActive(false);
    }

    private bool HasDamageableTag(string _targetTag)
    {
        // éwíËÇ™Ç»Ç¢èÍçáÇÕÅAÇ»ÇÒÇ≈Ç‡ìñÇΩÇÈ
        if(m_damageableTags.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < m_damageableTags.Length; i++)
        {
            if (_targetTag == m_damageableTags[i])
                return true;
        }
        return false;
    }

    public float RequestDamage(Health _health)
    {
        if (!m_canDealDamage || _health.IsDead)
        {
            return 0;
        }

        if (!m_canDamageMultiple)
        {
            m_canDealDamage = false;
        }

        int getInstanceID = _health.gameObject.GetInstanceID();
        if (!CheckDuplicateDamage(getInstanceID) && HasDamageableTag(_health.tag))
        {
            m_attackedInstances.Add(getInstanceID);

            if (m_callbackInterface != null)
            {
                DamageInfo damageInfo = new DamageInfo
                    (
                    _health.MinHP,
                    _health.MaxHP,
                    transform.position,
                    _health.transform.position,
                    _health.gameObject.name,
                    m_fDamage
                    );

                m_callbackInterface.OnDamageDone(_health, damageInfo);
            }
            OnDamageRequested(_health);
            return m_fDamage;
        }
        return 0;
    }
    private bool CheckDuplicateDamage(int _id)
    {
        if (m_canDoDuplicateDamage)
        {
            return false;
        }

        for (int i = 0; i < m_attackedInstances.Count; i++)
        {
            if (m_attackedInstances[i] == _id)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void OnDamageRequested(Health _Health) { }

    private void ApplyDamage(Collider2D other)
    {
        Health getHealth = other.GetComponent<Health>();
        if (getHealth != null)
        {
            float getDamage = RequestDamage(getHealth);
            if (getDamage != 0)
            {
                Collider2D collider = GetComponent<Collider2D>();
                Vector3 hitLocation = collider.bounds.ClosestPoint(((other.transform.position - this.transform.position) * 0.5f) + this.transform.position);
                getHealth.Damage(getDamage, hitLocation, m_owner);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HasDamageableTag(other.tag))
        {
            ApplyDamage(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (HasDamageableTag(collision.gameObject.tag))
        {
            ApplyDamage(collision.collider);
        }
    }
}
