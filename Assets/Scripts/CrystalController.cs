using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [SerializeField] Transform crystal;
    [SerializeField] private float pos_Offset;
    [SerializeField] private float pos_FrontOffset;

    [SerializeField] private float[] targetPositions;
    [SerializeField] private GameObject symbolInteractorPrefab;
    [SerializeField] private List<GameObject> symbols;
    [SerializeField] private List<SphereCollider> symbolInteractors;

    private void Awake()
    {
        SetTargets();
    }
    public void Init()
    {
        StartCoroutine(InitialiseCrystal());
    }
    public void Finish()
    {
        StartCoroutine(RetractCrystal());
    }
    private IEnumerator InitialiseCrystal()
    {
        float t = 0;
        float dur = 2f;

        Vector3 startPos = crystal.localPosition;
        Vector3 loweredPos = startPos - new Vector3(0, 0.5f, 0);

        //Debug.Log("Pressing");
        while (t < 1)
        {
            crystal.localPosition = Vector3.Lerp(startPos, loweredPos, t);
            t += Time.deltaTime / dur;  // This change ensures that t ranges from 0 to 1 during the duration
            yield return null;
        }
        crystal.localPosition = loweredPos;  // Ensure the final position is correct
    }
    private IEnumerator RetractCrystal()
    {
        float t = 0;
        float dur = 2f;

        Vector3 startPos = crystal.localPosition;
        Vector3 loweredPos = startPos - new Vector3(0, -0.5f, 0);

        while (t < 1)
        {
            crystal.localPosition = Vector3.Lerp(startPos, loweredPos, t);
            t += Time.deltaTime / dur;  // This change ensures that t ranges from 0 to 1 during the duration
            yield return null;
        }
        crystal.localPosition = loweredPos;  // Ensure the final position is correct
    }
    private void SetTargets()
    {
        foreach (var target in targetPositions)
        {
            var customRotation = new Vector3(0, target, 0);

            Quaternion customRotQuat = Quaternion.Euler(customRotation);

            Vector3 rotatedUp = customRotQuat * crystal.transform.up;
            Vector3 rotatedForward = customRotQuat * crystal.transform.forward;

            Vector3 worldEndPosition = crystal.transform.position + (rotatedUp * pos_Offset) + (rotatedForward * pos_FrontOffset);

            symbolInteractors.Add(Instantiate(symbolInteractorPrefab, worldEndPosition, Quaternion.identity, crystal).GetComponent<SphereCollider>());
        }
        foreach (var symbol in symbols)
        {
            symbol.AddComponent<SymbolTarget>();
        }
    }
    public void EnableTargets()
    {
        foreach (var symbolInteractor in symbolInteractors)
        {
            symbolInteractor.enabled = true;
        }
    }
    public void RotateCrystal(float angle)
    {
        //Debug.Log(rotation);
        var newRotation = RoundToNearest22Dot5Degrees((angle * 180f));
        var snapRot = crystal.transform.up * newRotation;

        crystal.transform.localRotation = Quaternion.Euler(snapRot);
    }
    void OnDrawGizmos()
    {
        foreach (var target in targetPositions)
        {
            Gizmos.color = Color.white;

            var customRotation = new Vector3(0, target, 0);

            Quaternion customRotQuat = Quaternion.Euler(customRotation);

            Vector3 rotatedUp = customRotQuat * crystal.transform.up;
            Vector3 rotatedForward = customRotQuat * crystal.transform.forward;

            Vector3 worldEndPosition = crystal.transform.position + (rotatedUp * pos_Offset) + (rotatedForward * pos_FrontOffset);

            Gizmos.DrawLine(crystal.transform.position, worldEndPosition);
            Gizmos.DrawWireSphere(worldEndPosition, 0.1f);
        }
    }
    private float RoundToNearest22Dot5Degrees(float angle)
    {
        return Mathf.Round(angle / 22.5f) * 22.5f;
    }
}
