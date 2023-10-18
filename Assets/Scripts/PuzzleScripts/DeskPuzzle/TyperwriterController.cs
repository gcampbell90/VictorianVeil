using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class TyperwriterController : MonoBehaviour
{
    public List<Transform> keys;

    public GameObject[] rayinteractors;

    public delegate void UnlockDesk();
    public static event UnlockDesk OnUnlockDesk;

    [SerializeField] private Transform[] passwordCombo;
    [SerializeField] private List<Transform> inputCombo;

    private void Start()
    {
        foreach (var key in keys)
        {
            var pushBtn = key.transform.gameObject.GetComponent<CustomXRButtonScript>();
            var keyobj = key.transform.GetChild(0).gameObject;
            pushBtn.button = keyobj.transform;
            pushBtn.onPress.AddListener(delegate { TrackKeys(key); });
            pushBtn.enabled = true;
        }
    }
    private void TrackKeys(Transform key)
    {
        Debug.Log(key.name);
        inputCombo.Add(key);

        // Optionally: Check the password after each key selection
        CheckPassword();
    }
    private void CheckPassword()
    {
        // If the input sequence is longer than the password, clear the input
        if (inputCombo.Count > passwordCombo.Length)
        {
            ResetInputCombo();
            return;
        }

        for (int i = 0; i < inputCombo.Count; i++)
        {
            if (inputCombo[i] != passwordCombo[i])
            {
                // Incorrect sequence
                ResetInputCombo();
                return;
            }
        }

        if (inputCombo.Count == passwordCombo.Length)
        {
            // Password is correct
            Unlock();
        }
    }
    private void ResetInputCombo()
    {
        inputCombo.Clear();
    }
    private void Unlock()
    {
        Debug.Log("Password Correct! Unlocking...");
        OnUnlockDesk?.Invoke();
        DisableTypewriter();
    }
    private void DisableTypewriter()
    {
        foreach (var key in keys)
        {
            var pushBtn = key.transform.gameObject.GetComponent<CustomXRButtonScript>();
            pushBtn.onPress.RemoveListener(delegate { TrackKeys(key); });
            Destroy(pushBtn);
            var keyobj = key.transform.GetChild(0).gameObject;
            Destroy(keyobj.GetComponent<Rigidbody>());
            Destroy(keyobj.GetComponent<SphereCollider>());
            Destroy(this);
        }
    }
}
