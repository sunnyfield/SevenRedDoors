using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { PASSIVE, ACTIVE, DELAY }

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public Transform target;
    //private CapsuleCollider2D targetCollider;
    private Transform targetPoint;
    public Vector2 focusAreaSize;
    private Vector2 cameraPosition;
    public Transform bottomLeftCameraBorder;
    public Transform topRightCameraBorder;
    private Vector2 camTarget;
    /*private Vector3 offset;
    private Vector3 smoothedPosition;
    
    private float smoothSpeed = 10f;*/

    //private float verticalOffset = -0.3f;
    //private float focusAreaDistance = 0f;
    private const float lookAheadDistanceHorizontal = 2f;
    private float cameraDampTimeX = 0.6f;
    private float cameraDampTimeY = 0.3f;
    private const float minDampTime = 0.1f;
    private const float maxDampTime = 0.6f;
    private float zoneVelocityX;
    private float zoneVelocityY;

    private float smoothLookVelocityHorizontal;
    private float smoothVelocityVertical;

    private float topBorder, bottomBorder;
    private float leftBordet, rightBorder;
    [HideInInspector]
    public float camLenght, camHeight;

    private float recoil;

    private float timer;

    private CameraState state = CameraState.PASSIVE;


    private struct DeadZone
    {
        private Transform character;
        private Transform sprite;
        public Vector2 center;
        private Vector2 size;
        public Vector2 offset;
        public Vector2 cameraTarget;
        private float lookAhead;
        private float left;
        private float right;
        private float top;
        private float bottom;

        public DeadZone(Transform character_, Transform sprite_, Vector2 size_, float lookAhead_)
        {
            sprite = sprite_;
            offset = Vector2.zero;
            character = character_;
            center = character.localPosition;
            size = size_;
            lookAhead = lookAhead_;
            cameraTarget.x = center.x + lookAhead * character.right.x;
            cameraTarget.y = center.y;

            left = center.x - size.x;
            right = center.x + size.x;
            top = center.y + size.y;
            bottom = center.y - size.y;
        }

        public void ZoneUpdate()
        {
            if (character.localPosition.x < left) offset.x = character.localPosition.x - left;
            else if (character.localPosition.x > right) offset.x = character.localPosition.x - right;
            else offset.x = 0;
            if (character.localPosition.y < bottom) offset.y = character.localPosition.y - bottom;
            else if (character.localPosition.y > top) offset.y = character.localPosition.y - top;
            else offset.y = 0;

            center += offset;
            cameraTarget.x = center.x + lookAhead * character.right.x;
            cameraTarget.y = center.y;

            left = center.x - size.x;
            right = center.x + size.x;
            top = center.y + size.y;
            bottom = center.y - size.y;
        }
    }


    private struct FocusArea
    {
        public Transform character;
        public Vector2 center;
        public Vector2 size;
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

        public FocusArea(Transform character_, Vector2 size_)
        {
            character = character_;
            size = size_;
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

            //left = center - size

            if (targetPoint.x < left) velocity.x = targetPoint.x - left;
            else if (targetPoint.x > right) velocity.x = targetPoint.x - right;

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
    DeadZone deadZone;

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
        targetPoint = target.GetChild(4);

        //focusArea = new FocusArea(target, focusAreaSize);
        deadZone = new DeadZone(target, targetPoint, focusAreaSize/2, lookAheadDistanceHorizontal);

        camLenght = camera.ViewportToWorldPoint(new Vector3(1, 1, 1)).x - camera.ViewportToWorldPoint(new Vector3(0, 1, 1)).x;
        camHeight = camera.ViewportToWorldPoint(new Vector3(1, 1, 1)).y - camera.ViewportToWorldPoint(new Vector3(1, 0, 1)).y;

        topBorder = topRightCameraBorder.position.y - camHeight / 2f;
        bottomBorder = bottomLeftCameraBorder.position.y + camHeight / 2f;
        leftBordet = bottomLeftCameraBorder.position.x + camLenght / 2f;
        rightBorder = topRightCameraBorder.position.x - camLenght / 2f;

        deadZone.ZoneUpdate();
        camTarget = deadZone.cameraTarget;
    }

    private void Update()
    {
        //focusArea.Update(targetPlayer.localPosition);
        deadZone.ZoneUpdate();
        camTarget.y = deadZone.cameraTarget.y;
        switch(state)
        {
            case CameraState.PASSIVE:
                if (Mathf.Abs(deadZone.offset.x) > 0.0001f) state = CameraState.ACTIVE;
                else
                {
                    if (camTarget.x != deadZone.cameraTarget.x)
                    {
                        timer = 1.3f;
                        state = CameraState.DELAY;
                    }
                }
                
                break;
            case CameraState.DELAY:
                if (Mathf.Abs(deadZone.offset.x) > 0.0001f) state = CameraState.ACTIVE;
                else
                {
                    if (timer > 0) timer -= Time.deltaTime;
                    else
                    {
                        camTarget.x = deadZone.cameraTarget.x;
                        state = CameraState.PASSIVE;
                    }
                }
                break;
            case CameraState.ACTIVE:
                if (Mathf.Abs(deadZone.offset.x) < 0.0001f)
                {
                    camTarget.x = deadZone.cameraTarget.x;
                    cameraDampTimeX = maxDampTime;
                    state = CameraState.PASSIVE;
                }
                else
                {
                    camTarget.x = deadZone.cameraTarget.x;
                    if (cameraDampTimeX > minDampTime) cameraDampTimeX -= 0.8f * Time.deltaTime;
                }
                break;
        }

        
        //focusPosition = deadZone.center + Vector2.up * verticalOffset;

        //focusPosition = focusArea.center + Vector2.up * verticalOffset;

        //focusAreaDistance += Mathf.Abs(focusArea.velocity.x);

        //if (focusArea.velocity.x != 0f)
        //{
        //    lookAheadDirectionHorizontal = Mathf.Sign(focusArea.velocity.x);
        //    if (focusAreaDistance >= 0.6f)
        //    {
        //        lookAheadStop = false;
        //        targetLookAheadHorizontal = lookAheadDirectionHorizontal * lookAheadDistanceHorizontal;
        //    }
        //    else
        //    {
        //        if (!lookAheadStop)
        //        {
        //            lookAheadStop = true;
        //            targetLookAheadHorizontal = currentLookAheadHorizontal + (lookAheadDirectionHorizontal * lookAheadDistanceHorizontal - currentLookAheadHorizontal) / 4f;
        //        }

        //    }
        //}
        //else if (PlayerController.instance.sideHorizontal == 0f)
        //    focusAreaDistance = 0f;

        //currentLookAheadHorizontal = Mathf.SmoothDamp(currentLookAheadHorizontal, targetLookAheadHorizontal, ref smoothLookVelocityHorizontal, lookAheadSmoothTimeHorizontal);

        //focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityVertical, smoothTimeVertical);
        //zoneVelocityX = 
        //print(zoneVelocityX);


        //if (Mathf.Abs(deadZone.offset.x) > 0.0001f)
        //{
        //    if (cameraDampTimeX > minDampTime) cameraDampTimeX -= 0.8f * Time.deltaTime;
        //    else cameraDampTimeX = minDampTime;
        //    print(cameraDampTimeX);
        //}
        //else
        //{
        //    //if (cameraDampTimeX < maxDampTime) cameraDampTimeX += 0.8f * Time.deltaTime;
        //    /*else*/ cameraDampTimeX = maxDampTime;
        //}


        cameraPosition.x = Mathf.SmoothDamp(transform.localPosition.x, camTarget.x, ref smoothLookVelocityHorizontal, cameraDampTimeX);
        cameraPosition.y = Mathf.SmoothDamp(transform.localPosition.y, camTarget.y, ref smoothVelocityVertical, cameraDampTimeY);
        //focusPosition += Vector2.right * currentLookAheadHorizontal;
    }

    private void LateUpdate()
    {
            transform.position = new Vector3(Mathf.Clamp(cameraPosition.x, leftBordet, rightBorder) + recoil, Mathf.Clamp(cameraPosition.y, bottomBorder, topBorder), -10f);
    }

    public void Recoil() { StartCoroutine(RecoilRoutine()); }

    public IEnumerator RecoilRoutine()
    {
        recoil = -target.transform.right.x * 0.1f;
        yield return null;
        recoil = 0f;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        //Gizmos.DrawCube(focusArea.center + (Vector2)transform.localPosition, focusAreaSize);
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(deadZone.center, focusAreaSize);
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawSphere(deadZone.cameraTarget, 0.2f);
    }
}
