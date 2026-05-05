using UnityEngine;
using TMPro;

public class ZindanKapisi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ipucuYazisi;
    bool yakinMi = false;

    void Start()
    {
        if (ipucuYazisi != null)
            ipucuYazisi.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!yakinMi) return;

        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (KeyManager.Instance != null && KeyManager.Instance.AreAllKeysCollected())
            {
                if (ipucuYazisi != null)
                    ipucuYazisi.transform.parent.gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                if (ipucuYazisi != null)
                {
                    ipucuYazisi.transform.parent.gameObject.SetActive(true);
                    ipucuYazisi.text = "Önce anahtarı tamamla!";
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = true;
            if (ipucuYazisi != null)
            {
                ipucuYazisi.transform.parent.gameObject.SetActive(true);
                ipucuYazisi.text = "[Space] Kapiyi Ac";
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            yakinMi = false;
            if (ipucuYazisi != null)
                ipucuYazisi.transform.parent.gameObject.SetActive(false);
        }
    }
}