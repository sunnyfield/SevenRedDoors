using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    //private Transform target;
    private CapsuleCollider2D targetCollider;
    private Transform targetPlayer;
    private Transform targetPoint;
    public Vector2 focusAreaSize;
    private Vector2 focusPosition;
    public Transform bottomLeftCameraBorder;
    public Transform topRightCameraBorder;
    /*private Vector3 offset;
    private Vector3 smoothedPosition;
    
    private float smoothSpeed = 10f;*/

    private float verticalOffset = -0.3f;
    private float focusAreaDistance = 0f;
    private float lookAheadDistanceHorizontal = 2f;
    private float lookAheadSmoothTimeHorizontal = 0.8f;
    private float smoothTimeVertical = 0.2f;

    private float currentLookAheadHorizontal;
    private float targetLookAheadHorizontal;
    private float lookAheadDirectionHorizontal;
    private float smoothLookVelocityHorizontal;
    private float smoothVelocityVertical;
    private bool lookAheadStop;

    private float topBorder, bottomBorder;
    private float leftBordet, rightBorder;
    [HideInInspector]
    public float camLenght, camHeight;

    private float recoil;



    private struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        //public FocusArea(Bounds targetBounds, Vector2 size)
        //{
        //    left = targetBounds.center.x - size.x / 2f;
        //    right = targetBounds.center.x + size.x / 2f;
        //    bottom = targetBounds.min.y;
        //    top = targetBounds.min.y + size.y;

        //    velocity = Vector2.zero;
        //    center = new Vector2((left + right) / 2f, (top + bottom) / 2f);
        //}

        public FocusArea(Vector2 size)
        {
            left = -size.x / 2f;
            right = size.x / 2f;
            bottom = -size.y / 2f;
            top = size.y / 2f;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2f, (top + bottom) / 2f);
        }

        //public void Update(Bounds targetBounds)
        //{
        //    float shiftX = 0;
        //    float shiftY = 0;

        //    if (targetBounds.min.x < left)
        //        shiftX = targetBounds.min.x - left;
        //    else if (targetBounds.max.x > right)
        //        shiftX = targetBounds.max.x - right;

        //    left += shiftX;
        //    right += shiftX;

        //    if (targetBounds.min.y < bottom)
        //        shiftY = targetBounds.min.y - bottom;
        //    else if (targetBounds.max.y > top)
        //        shiftY = targetBounds.max.y - top;

        //    top += shiftY;
        //    bottom += shiftY;

        //    center = new Vector2((left + right) / 2f, (top + bottom) / 2f);
        //    velocity = new Vector2(shiftX, shiftY);
        //}

        public void Update(Vector3 targetPoint)
        {
            float shiftX = 0;
            float shiftY = 0;

            if (targetPoint.x < left) shiftX = targetPoint.x - left;
            else if (targetPoint.x > right) shiftX = targetPoint.x - right;

            left += shiftX;
            right += shiftX;

            if (targetPoint.y < bottom) shiftY = targetPoint.y - bottom;
            else if (targetPoint.y > top) shiftY = targetPoint.y - top;

            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2f, (top + bottom) / 2f);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    FocusArea focusArea;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Camera camera = GetComponent<Camera>();
        targetPlayer = PlayerController.instance.gameObject.transform;
        targetPoint = PlayerController.instance.gameObject.transform.GetChild(4);
        targetCollider = PlayerController.instance.GetComponent<CapsuleCollider2D>();

        focusArea = new FocusArea(focusAreaSize);

        camLenght = camera.ViewportToWorldPoint(new Vector3(1, 1, 1)).x - camera.ViewportToWorldPoint(new Vector3(0, 1, 1)).x;
        camHeight = camera.ViewportToWorldPoint(new Vector3(1, 1, 1)).y - camera.ViewportToWorldPoint(new Vector3(1, 0, 1)).y;

        topBorder = topRightCameraBorder.position.y - camHeight / 2f;
        bottomBorder = bottomLeftCameraBorder.position.y + camHeight / 2f;
        leftBordet = bottomLeftCameraBorder.position.x + camLenght / 2f;
        rightBorder = topRightCameraBorder.position.x - camLenght / 2f;
    }

    private void Update()
    {
        focusArea.Update(targetPlayer.localPosition);

        focusPosition = focusArea.center + Vector2.up * verticalOffset;

        focusAreaDistance += Mathf.Abs(focusArea.velocity.x);

        if (focusArea.velocity.x != 0f)
        {
            lookAheadDirectionHorizontal = Mathf.Sign(focusArea.velocity.x);
            if (focusAreaDistance >= 0.6f)
            {
                lookAheadStop = false;
                targetLookAheadHorizontal = lookAheadDirectionHorizontal * lookAheadDistanceHorizontal;
            }
            else
            {
                if (!lookAheadStop)
                {
                    lookAheadStop = true;
                    targetLookAheadHorizontal = currentLookAheadHorizontal + (lookAheadDirectionHorizontal * lookAheadDistanceHorizontal - currentLookAheadHorizontal) / 4f;
                }

            }
        }
        else if (PlayerController.instance.sideHorizontal == 0f)
            focusAreaDistance = 0f;

        currentLookAheadHorizontal = Mathf.SmoothDamp(currentLookAheadHorizontal, targetLookAheadHorizontal, ref smoothLookVelocityHorizontal, lookAheadSmoothTimeHorizontal);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityVertical, smoothTimeVertical);
        focusPosition += Vector2.right * currentLookAheadHorizontal;
    }

    private void LateUpdate()
    {
            transform.position = new Vector3(Mathf.Clamp(focusPosition.x, leftBordet, rightBorder) + recoil, Mathf.Clamp(focusPosition.y, bottomBorder, topBorder), -10f);
    }

    public void Recoil() { StartCoroutine(RecoilRoutine()); }

    public IEnumerator RecoilRoutine()
    {
        recoil = -PlayerController.instance.transform.right.x * 0.06f;
        yield return new WaitForSeconds(0.08f);
        recoil = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
