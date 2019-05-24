using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerController player;

    //private static bool instanced = false;

    private void Awake()
    {
        //if (!instanced) instanced = true;
        //else Destroy(gameObject);
    }

    void Start() { player = GameObject.Find("/Scene/Player").GetComponent<PlayerController>(); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameController.instance.PauseToggle();

        HandlePlayerInput(player);
    }

    private void HandlePlayerInput(PlayerController player)
    {
        MoveInput move = MoveInput.NONE;
        ActionInput action = ActionInput.NONE;

        if ((int)Input.GetAxisRaw("Horizontal") == 1) move = MoveInput.RIGHT;
        else if ((int)Input.GetAxisRaw("Horizontal") == -1) move = MoveInput.LEFT;

        if (Input.GetButtonDown("Jump")) action = ActionInput.JUMP;
        else if (Input.GetKeyDown(KeyCode.F)) action = ActionInput.FIRE;
        else if (Input.GetKeyDown(KeyCode.R)) action = ActionInput.RELOAD;
        else if (Input.GetKeyDown(KeyCode.E)) action = ActionInput.ACTIVATE;

        player.HandleInput(move, action);
    }
}
