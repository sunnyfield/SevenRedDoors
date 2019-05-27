using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    private PlayerController player;
    public EventTrigger buttonRight;
    public EventTrigger buttonLeft;
    public EventTrigger buttonJump;
    public EventTrigger buttonFire;
    public EventTrigger buttonReload;

    MoveInput move = MoveInput.NONE;
    ActionInput action = ActionInput.NONE;

    //private static bool instanced = false;

    private void Awake()
    {
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
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { action = ActionInput.NONE; });
        buttonJump.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.FIRE; });
        buttonFire.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { action = ActionInput.NONE; });
        buttonFire.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(delegate { action = ActionInput.RELOAD; });
        buttonReload.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(delegate { action = ActionInput.NONE; });
        buttonReload.triggers.Add(entry);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameController.instance.PauseToggle();

        HandlePlayerInput(player);
    }

    private void HandlePlayerInput(PlayerController player)
    {

        //if ((int)Input.GetAxisRaw("Horizontal") == 1) move = MoveInput.RIGHT;
        //else if ((int)Input.GetAxisRaw("Horizontal") == -1) move = MoveInput.LEFT;

        //if (Input.GetButtonDown("Jump")) action = ActionInput.JUMP;
        //else if (Input.GetKeyDown(KeyCode.F)) action = ActionInput.FIRE;
        //else if (Input.GetKeyDown(KeyCode.R)) action = ActionInput.RELOAD;
        //else if (Input.GetKeyDown(KeyCode.E)) action = ActionInput.ACTIVATE;

        player.HandleInput(move, action);
    }
}
