using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public class FireplaceController : BasePuzzle
{
    [Header("Fireplace Items")]
    [SerializeField] private GameObject keyHoleSocketCoverGO;
   
    [Header("Socket Items")]
    [SerializeField] private GameObject circleSocketGO;
    [SerializeField] private GameObject keySocketGO;
    [SerializeField] Transform slotCover;
    
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinPosition;


    [Header("Interaction")]
    private XRSocketInteractor aztecCircleSocket;
    private XRSocketInteractor keySocket;

    [SerializeField] private XRPushButton[] pushButtons;
    [SerializeField] private XRPushButton[] passwordCombo;
    private List<XRPushButton> inputCombo = new List<XRPushButton>();

    XRSlideable keyHoleSlider;

    FireplaceKey fpKey;

    //Events
    public delegate void KeyEntered();
    public static KeyEntered onKeyEntered;

    public delegate void CodeEntered();
    public CodeEntered onCodeEntered;

    //Unity Methods

    private void OnEnable()
    {
        aztecCircleSocket.selectEntered.AddListener((args) => CheckIsValid(args, aztecCircleSocket));

        keySocket.hoverEntered.AddListener(RegisterKey);

        keyHoleSlider.onMovementCompleted += DisableKeyholeSocketCover;
        keyHoleSlider.onMovementCompleted += () => ToggleSlot(keySocket, true);
        
        onKeyEntered += ActivateFireplacePuzzle;
        onCodeEntered += CompletePuzzle;

        foreach (var pushButton in pushButtons)
        {
            pushButton.onPress.AddListener(delegate { ButtonPress(pushButton); });
        }
    }

    protected override void Awake()
    {
        base.Awake();

        aztecCircleSocket = circleSocketGO.GetComponent<XRSocketInteractor>();
        keySocket = keySocketGO.GetComponent<XRSocketInteractor>();
        keyHoleSlider = keyHoleSocketCoverGO.GetComponent<XRSlideable>();

    }


    protected override void Start()
    {
        base.Start();
        ToggleSlot(keySocket, false);
        ToggleSlot(aztecCircleSocket, false);

        if (DebugMode)
        {
            GetComponent<CrystalController>().Init();
            ToggleSlot(aztecCircleSocket, true);
            StartCoroutine(MoveSlotCover());
            CompletePuzzle();
        }
    }

    private void OnDisable()
    {
        aztecCircleSocket.selectEntered.RemoveListener((args) => CheckIsValid(args, aztecCircleSocket));

        keySocket.hoverEntered.RemoveListener(RegisterKey);

        keyHoleSlider.onMovementCompleted -= DisableKeyholeSocketCover;
        keyHoleSlider.onMovementCompleted -= () => ToggleSlot(keySocket, true);

        onKeyEntered -= ActivateFireplacePuzzle;

        foreach (var pushButton in pushButtons)
        {
            pushButton.onPress.RemoveListener(delegate { ButtonPress(pushButton); });
        }
    }

    private void RegisterKey(HoverEnterEventArgs arg0)
    {
        var manager = keySocket.interactionManager;
        fpKey = arg0.interactableObject.transform.GetComponent<FireplaceKey>();

        // Check if socket is currently holding an object
        if (keySocket.hasSelection)
        {
            Debug.Log(keySocket.hasSelection);
            var thing = keySocket.GetOldestInteractableSelected();
            Debug.Log(thing);
            manager.SelectEnter(keySocket, thing);
            // Force release the current object
        }
        fpKey.onKeyPlaced?.Invoke();
        StartCoroutine(DeactivateSocketAfterDelay(keySocket));
    }

    #region PushButtonLogic
    private void ButtonPress(XRPushButton pushButton)
    {
        Debug.Log(pushButton.name);
        TrackButtons(pushButton);
    }

    private void TrackButtons(XRPushButton pushButton)
    {
        inputCombo.Add(pushButton);

        // Optionally: Check the password after each key selection
        CheckButtonCombo();
    }

    private void CheckButtonCombo()
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
                Debug.Log("Resetting Input");
                // Incorrect sequence
                ResetInputCombo();
                return;
            }
        }

        if (inputCombo.Count == passwordCombo.Length)
        {
            // Password is correct
            Debug.Log("Password Correct");
            onCodeEntered?.Invoke();
        }
    }

    private void ResetInputCombo()
    {
        inputCombo.Clear();
    }
    #endregion

    private void DisableKeyholeSocketCover()
    {
        Destroy(keyHoleSlider);
        Destroy(keyHoleSlider.gameObject.GetComponent<Collider>());
        Destroy(keyHoleSlider.gameObject.GetComponent<Rigidbody>());
    }

    private void CheckIsValid(SelectEnterEventArgs arg0, XRSocketInteractor socket)
    {
        StartCoroutine(SwitchInteractableAfterDelay(arg0, socket));
    }

    private IEnumerator SwitchInteractableAfterDelay(SelectEnterEventArgs arg0, XRSocketInteractor socket)
    {
        var interactable = arg0.interactableObject.transform.GetComponent<XRGrabInteractable>();
        interactable.interactionLayers = ToggleInteractionLayer(interactable.interactionLayers, false);

        StartCoroutine(DeactivateSocketAfterDelay(socket));

        yield return new WaitForSeconds(1f);
        socket.socketActive = false;
        interactable.interactionLayers = ToggleInteractionLayer(interactable.interactionLayers, true);

        if (arg0.interactableObject.transform.name == "AztecPiece")
        {
            ChangeToKnobInteractable(arg0.interactableObject);
            //Debug.Log("Changing Interactions of AztecPiece");
        }
    }

    private IEnumerator DeactivateSocketAfterDelay(XRSocketInteractor socketInteractor)
    {
        yield return new WaitForSeconds(0.5f);
        socketInteractor.selectEntered.RemoveAllListeners();
        Destroy(socketInteractor);
        //Debug.Log("Destroyed Socket");

    }

    private void ChangeToKnobInteractable(IXRSelectInteractable interactableObject)
    {

        //Debug.Log("Changing To Knob");
        Destroy(interactableObject.transform.GetComponent<XRGrabInteractable>());
        Destroy(interactableObject.transform.GetComponent<XRGeneralGrabTransformer>());
        Destroy(interactableObject.transform.GetComponent<InventoryObject>());

        var xrknob = interactableObject.transform.gameObject.GetComponent<XRKnob>();

        var crystalController = transform.GetComponent<CrystalController>();
        xrknob.onValueChange.AddListener(crystalController.RotateCrystal);
        xrknob.enabled = true;
    }

    private InteractionLayerMask ToggleInteractionLayer(InteractionLayerMask layerMask, bool isOn)
    {
        int layer = isOn ? 2 : 30;
        layerMask = 1 << layer;
        return layerMask;
    }

    private void ActivateFireplacePuzzle()
    {
        StartCoroutine(MoveSlotCover());
        GetComponent<CrystalController>().Init();
        ToggleSlot(aztecCircleSocket, true);
    }

    private IEnumerator MoveSlotCover()
    {
        float t = 0;
        float dur = 2f;

        Vector3 startPos = slotCover.localPosition;
        Vector3 endPos = startPos - new Vector3(0, 0, 0.1f);

        while (t < 0.2f)
        {
            slotCover.localPosition = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / dur;  // This change ensures that t ranges from 0 to 1 during the duration
            yield return null;
        }

        t = 0;
        startPos = slotCover.localPosition;
        endPos = startPos - new Vector3(0.5f, 0, 0);
        while (t < 1)
        {
            slotCover.localPosition = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / dur;  // This change ensures that t ranges from 0 to 1 during the duration
            yield return null;
        }
        slotCover.localPosition = endPos;  // Ensure the final position is correct
    }

    private void ToggleSlot(XRSocketInteractor socket, bool isOn)
    {
        socket.socketActive = isOn;
        //Debug.Log($"{socket} + {socket.enabled}");
    }

    private new void CompletePuzzle()
    {
        base.CompletePuzzle();
        RevealCoin();
        GetComponent<CrystalController>().Finish();
    }

    private void RevealCoin()
    {
        Instantiate(coinPrefab, coinPosition.position, coinPosition.rotation);
    }

}
