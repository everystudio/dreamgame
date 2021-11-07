using UnityEngine;
using System.Collections;
using TMPro;
using anogamelib;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class LootableItem : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public class References
    {
        public Rigidbody2D rigidBody2D;
        public SpriteRenderer spriteRenderer;
        public TextMeshPro amountText;
        public BoxCollider2D boxCollider2D;
    }

    [System.Serializable]
    public class Configuration
    {
        public ItemData data;
        public int amount;
        public float moveSpeed = 2;
        public float activationWait = 1;
    }

    [SerializeField]
    private References references;

    [SerializeField]
    private Configuration configuration;

    private Coroutine moveCoroutine;

    private GameObject playerItemPicker;
    private bool isLooted;

    private void OnValidate()
    {
        if (configuration.data != null)
        {
            references.spriteRenderer.sprite = configuration.data.Icon;
        }
    }

    public float PickupDistance()
    {
        return references.boxCollider2D.size.magnitude * 0.5f;
    }

    public void Configure(ItemData data, int amount)
    {
        configuration.data = data;
        configuration.amount = amount;
        isLooted = false;
        Refresh();
    }

    private void OnEnable()
    {
        if (configuration.data != null)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (configuration.data != null)
        {
            references.spriteRenderer.sprite = configuration.data.Icon;

            if (configuration.amount > 1 && configuration.data.CanStack)
            {
                references.amountText.text = configuration.amount.ToString();
                references.amountText.gameObject.SetActive(true);
            }
            else
            {
                references.amountText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLooted && collision.CompareTag("PlayerItemPicker"))
        {
            playerItemPicker = collision.gameObject;
            Invoke("MoveToPlayer", configuration.activationWait);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isLooted && collision.CompareTag("PlayerItemPicker"))
        {
            playerItemPicker = null;
            CancelInvoke("MoveToPlayer");
        }
    }

    private void MoveToPlayer()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToPlayer(playerItemPicker));
    }

    IEnumerator MoveToPlayer(GameObject _target)
    {
        while (Vector3.Distance(this.transform.position, _target.transform.position) > 0.05f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _target.transform.position, Time.deltaTime * configuration.moveSpeed);
            yield return null;
        }

        if (configuration.data != null)
        {
            /*
            Inventory getInventory = _target.GetComponent<Inventory>();
            getInventory.AddItem(configuration.data, configuration.amount);
            */
        }

        this.gameObject.SetActive(false);
    }

    [System.Serializable]
    private struct SaveData
    {
        public string data;
        public int amount;
    }

    public string OnSave()
    {
        if (!gameObject.activeSelf)
        {
            return string.Empty;
        }

        return JsonUtility.ToJson(new SaveData()
        {
            data = configuration.data.GetGuid(),
            amount = configuration.amount
        });
    }

    public void OnLoad(string data)
    {
        SaveData getData = JsonUtility.FromJson<SaveData>(data);
        /*
        if (!string.IsNullOrEmpty(data))
        {
            Configure(ScriptableAssetDatabase.GetAsset(getData.data) as ItemData, getData.amount);
        }*/
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}
