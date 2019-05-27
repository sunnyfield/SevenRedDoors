using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTrigger : MonoBehaviour
{

    private Coroutine activateRoutine;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite spriteON;
    [SerializeField]
    private Sprite spriteOFF;
    private Transform Icon;
    private BoxCollider2D boxTrigger;
    private Transform icon;
    [SerializeField]
    public GameObject objectToActivate;
    private ITriggerable interfaceToActivate;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        spriteOFF = Resources.Load<Sprite>("Images/Interactive/Trigger_OFF");
        spriteRenderer.sprite = spriteOFF;
        spriteON = Resources.Load<Sprite>("Images/Interactive/Trigger_ON");

        //Icon = transform.Find("Icon");
        icon = transform.GetChild(0);
        icon.gameObject.SetActive(false);
        
        boxTrigger = gameObject.GetComponent<BoxCollider2D>();

        interfaceToActivate = objectToActivate.GetComponent<ITriggerable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            if (activateRoutine == null)
            {
                collision.GetComponent<PlayerController>().trigger = this;
                icon.gameObject.SetActive(true);
                activateRoutine = StartCoroutine(Activate());
                InputHandler.instance.ActivateInteractiveButton();
            }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activateRoutine != null)
        {
            collision.GetComponent<PlayerController>().trigger = null;
            InputHandler.instance.DeactivateInteractiveButton();
            StopCoroutine(activateRoutine);
            activateRoutine = null;
            icon.gameObject.SetActive(false);
        }
    }

    private IEnumerator Activate()
    {
        Vector3 targetIconScale = new Vector3(0.55f, 0.55f, 1f);

        while (true)
        {
            if (icon.localScale.x > 0.5f)
                targetIconScale = new Vector3(0.15f, 0.15f, 1f);
            else if (icon.localScale.x < 0.2f)
                targetIconScale = new Vector3(0.55f, 0.55f, 1f);

            icon.localScale = Vector3.Lerp(icon.localScale, targetIconScale, Time.deltaTime * 5f);

            yield return null;
        }    
    }

    public void TurnOn()
    {
        spriteRenderer.sprite = spriteON;
        boxTrigger.enabled = false;
        interfaceToActivate.Activate();
    }
}
