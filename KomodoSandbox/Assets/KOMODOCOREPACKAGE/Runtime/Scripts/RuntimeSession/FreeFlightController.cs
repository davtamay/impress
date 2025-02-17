//#define TESTING_BEFORE_BUILDING

using UnityEngine.EventSystems;
using UnityEngine;
using WebXR;
using System.Collections;
using Komodo.Utilities;

//namespace Komodo.Runtime
//{
    public class FreeFlightController : MonoBehaviour, IUpdatable
    {
        [Tooltip("Enable/disable rotation control. For use in Unity editor only.")]
        public bool rotationEnabled = true;

        private WebXRDisplayCapabilities capabilities;

        [Tooltip("Rotation sensitivity")]
        public float rotationSensitivity = 3f;

        public bool naturalRotationDirection = true;

        [Tooltip("Enable/disable translation control. For use in Unity editor only.")]
        public bool translationEnabled = true;

        [Tooltip("Pan Sensitivity sensitivity + middle mouse hold")]
        public float panSensitivity = 0.1f;

        public bool naturalPanDirection = true;

        [Tooltip("Strafe Speed")]
        public float strafeSpeed = 5f;

        public bool naturalStrafeDirection = true;

        [Tooltip("Forward and Backwards Hyperspeed Scroll")]
        public float scrollSpeed = 10f;

        public bool naturalScrollDirection = true;

        [Tooltip("Snap Rotation Angle")]
        public int turningDegrees = 30;

        Quaternion originalRotation;

        public GameObject floorIndicator;
        [SerializeField] private Camera spectatorCamera;
        //[SerializeField] private TeleportPlayer cameraOffset;

        //[Tooltip("Hierarchy: Spectator Camera -> TeleportationLine")]
        //[SerializeField] private GameObject teleportationIndicator;

        Vector3 targetPosition; // this is the position that the floorIndicator should be;

        public Transform desktopCamera;

        private Transform playspace;

        private float minimumX = -360f;
        private float maximumX = 360f;

        private float minimumY = -90f;
        private float maximumY = 90f;


        //to check on ui over objects to disable mouse drag while clicking buttons
        //private StandaloneDesktopInputModule standaloneInputModule_Desktop;

        ////used for syncing our XR player position with desktop
        //public TeleportPlayer teleportPlayer;

        void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR || TESTING_BEFORE_BUILDING
            WebXRManager.OnXRChange += onXRChange;
#else 
            WebXRManagerEditorSimulator.OnXRChange += onXRChange;
#endif
            WebXRManager.OnXRCapabilitiesUpdate += onXRCapabilitiesUpdate;

          //  desktopCamera = transform;
        //  standaloneInputModule_Desktop = EventSystemManager.Instance.desktopStandaloneInput;
        }

        //private void WebXRManager_OnXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        //{
        //    if (state == WebXRState.VR)
        //    {
        //     //   SetToXR();
        //    }
        //    else if (state == WebXRState.NORMAL)
        //    {
        //     //   SetToDesktop();
        //    }
        //}

        public void Start()
        {
            //wait for our ui to be set up before we allow user to move around with camera
          //  GameStateManager.Instance.DeRegisterUpdatableObject(this);
            //isUpdating = false;

            //get our references for the player we are moving and its xrcamera and desktopcamera
            //TryGetPlayerReferences();

            //originalRotation = desktopCamera.localRotation;

            //playspace = GameObject.FindWithTag(TagList.xrCamera).transform;
           // desktopCamera = GameObject.FindWithTag(TagList.desktopCamera).transform;//transform;

        //    originalRotation = desktopCamera.localRotation;

            playspace = GameObject.FindWithTag(TagList.xrCamera).transform;
      //  desktopCamera = Camera.main.transform; 
          //  desktopCamera = GameObject.FindWithTag(TagList.desktopCamera).transform;//transform;

            originalRotation = desktopCamera.localRotation;

            //if (EventSystemManager.IsAlive)
            //{
            //    //get our desktop eventsystem
            //    if (!standaloneInputModule_Desktop)
            //        standaloneInputModule_Desktop = EventSystemManager.Instance.desktopStandaloneInput;
            //}

      
          
            
            
            
            //start using our freflightcontroller after we finish loading UI
           GameStateManager.Instance.RegisterUpdatableObject(this);

            //teleportPlayer.BeginPlayerHeightCalibration(left hand? right hand?); //TODO turn back on and ask for handedness 
        }

        public void EnableFreeFlightCameraUpdates()
        {
            enabled = true;
            GameStateManager.Instance.RegisterUpdatableObject(this);
        }

        public void OnSceneLoadedAndFirstTransportIsDone()
        {
            GameStateManager.Instance.RegisterUpdatableObject(this);
        }

   //     teleportPlayer.


        public void OnUpdate(float deltaTime)
        {
            if (translationEnabled)
            {
                MovePlayerFromInput();
            }

            if (IsMouseInteractingWithMenu()) {
                return;
            }

            if ((rotationEnabled && Input.GetMouseButton(0))) //|| (rotationEnabled && Input.GetMouseButton(1)))
            {
                RotatePlayerFromInput();
            }

            if (Input.GetMouseButton(2))
            {
                PanPlayerFromInput();
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                HyperspeedPanPlayerFromInput();   
            }

            //ShowTeleportationIndicator();

            //MousePositionToTeleportationIndicator();

            SyncXRWithSpectator();
        }

        private void onXRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            if (state == WebXRState.VR)
            {
                GameStateManager.Instance.DeRegisterUpdatableObject(this);
                //isUpdating = false;

                //Reset the XR rotation of our VR Cameras to avoid leaving weird rotations from desktop mode
                curRotationX = 0f;

            var result = Quaternion.Euler(new Vector3(0, curRotationY, 0));

          //  teleportPlayer.SetXRAndSpectatorRotation(result);

            }
            else if(state == WebXRState.NORMAL)
            {
                //commented to avoid setting rotation back on which causes rotational issues when switching cameras
                //  EnableAccordingToPlatform();

                //set desktop camera the same as the xr camera on xr exit
                curRotationX = 0f;

                desktopCamera.position = playspace.position;

                desktopCamera.localRotation = Quaternion.Euler(new Vector3(0, curRotationY, 0));

                SyncXRWithSpectator();

                GameStateManager.Instance.RegisterUpdatableObject(this);
               // isUpdating = true;
            }
        }

        /// <summary>
        /// Get a reference for a playerset to move
        /// </summary>
        //public void TryGetPlayerReferences()
        //{
        //    var player = GameObject.FindWithTag(TagList.player);

        //    if (!player)
        //        Debug.Log("player not found for FreeFlightController.cs");
        //    else
        //    {
        //       if(player.TryGetComponent(out TeleportPlayer tP))
        //        {
        //            teleportPlayer = tP;
        //        }
        //        else
        //        {
        //            Debug.Log("no TeleportPlayer script found for player in FreeFlightController.cs");
        //        }
        //    }


        ////  playspace = GameObject.FindWithTag(TagList.xrCamera).transform;
        //desktopCamera = Camera.main.transform;// GameObject.FindWithTag(TagList.desktopCamera).transform;//transform;

        //    //if(!playspace)
        //    //    Debug.Log("no XRCamera tagged object found in FreeFlightController.cs");

        //    //if (!desktopCamera)
        //    //    Debug.Log("no desktopCamera tagged object found in FreeFlightController.cs");

        //}

        private void onXRCapabilitiesUpdate(WebXRDisplayCapabilities vrCapabilities)
        {
            capabilities = vrCapabilities;
            EnableAccordingToPlatform();
        }

        #region Snap Turns and Move Functionality - Button event linking functions (editor UnityEvent accessible)
        Quaternion xQuaternion;
        Quaternion yQuaternion;
    
        public void RotatePlayer(int rotateDirection)
        {
            switch (rotateDirection)
            {

                case 3:
                    //LEFT
                    curRotationX -= turningDegrees;
                    break;

                case 4:
                    //RIGHT
                    curRotationX += turningDegrees;
                    break;

                case 2:
                    //UP
                    curRotationY -= turningDegrees;
                    break;

                case 1:
                    //DOWN
                    curRotationY += turningDegrees;
                    break;
            }

            curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
            curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

            desktopCamera.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
        }

        public void RotateXRPlayer(int rotateDirection)
        {
            switch (rotateDirection)
            {
                case 3:
                    //LEFT
                    curRotationX -= turningDegrees;
                    break;

                case 4:
                    //RIGHT
                    curRotationX += turningDegrees;
                    break;

                case 2:
                    //UP
                    curRotationY -= turningDegrees;
                    break;

                case 1:
                    //DOWN
                    curRotationY += turningDegrees;
                    break;
            }
            curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
            curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

            var result = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));

         //   teleportPlayer.SetXRAndSpectatorRotation(result);
        }

        float curRotationX = 0f;
        float curRotationY = 0f;
        public void RotatePlayerWithDelta(int rotateDirection)
        {
            var delta = Time.deltaTime * strafeSpeed;

            switch (rotateDirection)
            {
                case 3:
                    //LEFT
                    curRotationX -= 45F * delta;
                    break;

                case 4:
                    //RIGHT
                    curRotationX += 45F * delta;
                    break;

                case 2:
                    //UP
                    curRotationY -= 45 * delta;
                    break;

                case 1:
                    //DOWN
                    curRotationY += 45 * delta;
                    break;
            }
            curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
            curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

            desktopCamera.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
        }

        public void MovePlayer(int moveDirection)
        {
            switch (moveDirection)
            {

                case 1:

                    float x = 1;
                    var movement = new Vector3(x, 0, 0);
                    movement = desktopCamera.TransformDirection(movement);
                    desktopCamera.position += movement;
                    break;

                case 2:

                    x = -1;
                    movement = new Vector3(x, 0, 0);
                    movement = desktopCamera.TransformDirection(movement);
                    desktopCamera.position += movement;
                    break;

                case 3:

                    float z = 1;
                    movement = new Vector3(0, 0, z);
                    movement = desktopCamera.TransformDirection(movement);
                    desktopCamera.position += movement;
                    break;

                case 4:

                    z = -1;
                    movement = new Vector3(0, 0, z);
                    movement = desktopCamera.TransformDirection(movement);
                    desktopCamera.position += movement;
                    break;
            }
        }

        public void MovePlayerFromInput() {
            var accumulatedImpactMul = Time.deltaTime * strafeSpeed;
            float x = Input.GetAxis("Horizontal") * accumulatedImpactMul;
            float z = Input.GetAxis("Vertical") * accumulatedImpactMul;

            if (Input.GetKey(KeyCode.Q)) RotatePlayerWithDelta(2);
            if (Input.GetKey(KeyCode.E)) RotatePlayerWithDelta(1);
            if (Input.GetKey(KeyCode.Alpha2)) RotatePlayerWithDelta(3);
            if (Input.GetKey(KeyCode.Alpha3)) RotatePlayerWithDelta(4);


            var movement = new Vector3(x, 0, z);
            movement = desktopCamera.TransformDirection(movement) * (naturalStrafeDirection ? -1 : 1);
            desktopCamera.position += movement;
        }
        private Vector3 lastMousePosition;
        public void RotatePlayerFromInput() {
          
            if (Input.touchCount >= 2)
            {
                // Exit early if there are two or more touches.
                return;
            }


            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 deltaMousePosition = touch.deltaPosition * 0.1f;
                    curRotationY += deltaMousePosition.x * rotationSensitivity * (naturalRotationDirection ? -1 : 1);
                    curRotationX -= deltaMousePosition.y * rotationSensitivity * (naturalRotationDirection ? -1 : 1);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePosition = Input.mousePosition;
                }
                Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
                deltaMousePosition *= 0.1f;

                curRotationY += deltaMousePosition.x * rotationSensitivity * (naturalRotationDirection ? -1 : 1); //horizontal mouse motion translates to rotation around the Y axis.
                curRotationX -= deltaMousePosition.y * rotationSensitivity * (naturalRotationDirection ? -1 : 1); // vertical mouse motion translates to rotation around the X axis.
             
                lastMousePosition = Input.mousePosition;
            }
                curRotationX = ClampAngle(curRotationX, minimumY, maximumY);
                curRotationY = ClampAngle(curRotationY, minimumX, maximumX);

                desktopCamera.localRotation = Quaternion.Euler(new Vector3(curRotationX, curRotationY, 0));
        }

        public void PanPlayerFromInput() {
            var x = Input.GetAxis("Mouse X") * panSensitivity * (naturalPanDirection ? -1 : 1);
            var y = Input.GetAxis("Mouse Y") * panSensitivity * (naturalPanDirection ? -1 : 1);

            desktopCamera.position += desktopCamera.TransformDirection(new Vector3(x, y));
        }

        public void HyperspeedPanPlayerFromInput() {
            desktopCamera.position += desktopCamera.TransformDirection(scrollSpeed * new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * (naturalScrollDirection ? -1 : 1)));
            // transform.parent.position += Camera.main.transform.TransformDirection(scrollSpeed * new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * (naturalScrollDirection ? -1 : 1)));


        //  // Get the scroll wheel input.
        //float scrollInput = Input.GetAxis("Mouse ScrollWheel") * (naturalScrollDirection ? -1 : 1);

        //// Calculate the movement direction and magnitude based on the scroll input.
        //// This ensures we're moving along the camera's local z-axis, which is its forward direction.
        //Vector3 moveDirection = scrollInput * scrollSpeed * Time.deltaTime * Vector3.forward;

        //// Transform the direction from local space to world space relative to the camera.
        //Vector3 worldDirection = Camera.main.transform.TransformDirection(moveDirection);

        //// Apply the movement to the camera's position.
        //Camera.main.transform.position += worldDirection;
    }
    //public void HyperspeedPanPlayerFromInput()
    //{
    //    desktopCamera.position += desktopCamera.TransformDirection(scrollSpeed * new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * (naturalScrollDirection ? -1 : 1)));
    //}

    public bool IsMouseInteractingWithMenu() {

            if (EventSystem.current != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                return true;
                //if (standaloneInputModule_Desktop.GetCurrentFocusedObject_Desktop())
                //{
                //    if (standaloneInputModule_Desktop.GetCurrentFocusedObject_Desktop().layer == LayerMask.NameToLayer("UI"))
                //    {
                //        return true;
                //    }
                //}
            }
            }

            return false;
        }

        public void SyncXRWithSpectator() {
            //synchronize xr camera with desktop camera transforms
       //     teleportPlayer.SetXRPlayerPositionAndLocalRotation(desktopCamera.position, desktopCamera.localRotation);
        }

        #endregion


        /// Enables rotation and translation control for desktop environments.
        /// For mobile environments, it enables rotation or translation according to
        /// the device capabilities.
        void EnableAccordingToPlatform()
        {
            rotationEnabled = translationEnabled = !capabilities.canPresentVR;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) 
            {
                angle += 360f;
            }
            if (angle > 360f) 
            {
                angle -= 360f;
            }
            return Mathf.Clamp(angle, min, max);
        }

        /// <Summary> 
        /// Show teleportation indicator while holding right click.
        /// </Summary>
        //public void ShowTeleportationIndicator()
        //{  
        //    if (Input.GetMouseButtonDown(1)) {

        //        teleportationIndicator.SetActive(true);

        //    } else if (Input.GetMouseButtonUp(1)) {

        //        teleportationIndicator.SetActive(false);
        //    }
        //}

        /// <Summary>
        /// This function turns mouse position in an xy coordinate into a ray.
        /// The RaycastHit will hit something in the scene and becomes the z coordinate of the mouse's position.
        /// It will then assign mouse's position to floorIndicator.
        /// </Summary>
        public void MousePositionToTeleportationIndicator() 
        {
            // Check if a right click is currently down or two fingers are touching the screen
            if (Input.GetMouseButton(1) || (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began))
            {
                // Get the position of the mouse or the second touch
                Vector3 position = Input.GetMouseButton(1) ? Input.mousePosition : Input.GetTouch(1).position;

                // Get a ray from the position
                Ray ray = spectatorCamera.ScreenPointToRay(position);

                // Perform a raycast to see if the ray hits a collider
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Set the target position to the hit point plus an offset
                    Vector3 targetPosition = hit.point;

                    // Update the floor indicator position and rotation
                    floorIndicator.transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
                    floorIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }
            }

        

        }
    }
//}
