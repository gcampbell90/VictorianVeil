using UnityEngine;

public class SymbolTarget : MonoBehaviour
{
    Material mat;

    private void Awake()
    {
        mat = transform.GetComponent<Renderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Lighting On");
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.white);

    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Lighting Off");
        mat.DisableKeyword("_EMISSION");
        //mat.SetColor("_EmissionColor", Color.white);
    }
}