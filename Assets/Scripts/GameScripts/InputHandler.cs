using System.Collections;
using System.Collections.Generic;
using GameScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;

    private PlayerController player;
    public EventTrigger buttonRight;
    public EventTrigger buttonLeft;
    public EventTrigger buttonJump;
    public EventTrigger buttonFire;
    public EventTrigger buttonReload;
    public EventTrigger buttonInteract;

    MoveInput move = MoveInput.NONE;
    ActionInput action = ActionInput.NONE;

    //private static bool instanced = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        //if (!instanced) instanced = true;
        //else Destroy(gameObject);
    }

    void Start()
    { 
        player = GameObject.Find("/Scene/Player").GetComponent<PlayerController>();
        BaseAIBehaviorState.target = player.transform;

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { move = MoveInput.RIGHT; });
        buttonRight.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { move = MoveInput.NONE; });
        buttonRight.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { move = MoveInput.LEFT; });
        buttonLeft.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { move = MoveInput.NONE; });
        buttonLeft.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.JUMP; });
        buttonJump.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.FIRE; });
        buttonFire.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.RELOAD; });
        buttonReload.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.ACTIVATE; });
        buttonInteract.triggers.Add(entry);
        buttonInteract.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameController.Instance.PauseToggle();

        HandlePlayerInput(player);
    }

    public void ActivateInteractiveButton()
    {
        buttonInteract.gameObject.SetActive(true);
    }

    public void DeactivateInteractiveButton()
    {
        buttonInteract.gameObject.SetActive(false);
    }

    private void HandlePlayerInput(PlayerController player)
    {
#if UNITY_EDITOR
        move = MoveInput.NONE;
        action = ActionInput.NONE;

        if ((int)Input.GetAxisRaw("Horizontal") == 1) move = MoveInput.RIGHT;
        else if ((int)Input.GetAxisRaw("Horizontal") == -1) move = MoveInput.LEFT;

        if (Input.GetButtonDown("Jump")) action = ActionInput.JUMP;
        else if (Input.GetKeyDown(KeyCode.F)) action = ActionInput.FIRE;
        else if (Input.GetKeyDown(KeyCode.R)) action = ActionInput.RELOAD;
        else if (Input.GetKeyDown(KeyCode.E)) action = ActionInput.ACTIVATE;
#endif
        player.HandleInput(move, action);
        action = ActionInput.NONE;
    }
}
