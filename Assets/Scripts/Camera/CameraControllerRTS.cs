using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerRTS : MonoBehaviour
{
    [SerializeField] GameObject player;
    
    private Transform rtsCamera;

    private float defaultCameraSpeed = 0.5f;
    private float fastCameraSpeed = 2f;
    private float cameraMovementSpeed;
    private float cameraMovementTime = 5f;
    private float cameraRotationSpeed = 1f;
    private Vector3 cameraZoomSpeed = new Vector3(0f, -10f, 10f);
    private int mouseWheelRange = 3;
    private int mouseWheelCounter = 0;

    private Vector3 newCameraPosition;
    private Quaternion newCameraRotation;
    private Vector3 newCameraZoom;

    // Mouse Movement
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    [SerializeField] Transform terrain;
    private float terrainSizeX;
    private float terrainSizeZ;

    // Start is called before the first frame update
    void Start()
    {
        rtsCamera = transform.GetChild(0);
        newCameraZoom = rtsCamera.transform.localPosition;

        cameraMovementSpeed = defaultCameraSpeed;
        newCameraPosition = transform.position;
        newCameraRotation = transform.rotation;

        terrainSizeX = terrain.localScale.x * 5;
        terrainSizeZ = terrain.localScale.z * 5;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        LeaveRTSView();
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ResetCamera();
        }
        GetMovementSpeedInput();
        GetMovementDirectionInput();
        GetRotationInput();
        GetZoomInput();

       // MoveWithMouse();
    }

    private void GetMovementDirectionInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newCameraPosition += (transform.forward * cameraMovementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newCameraPosition += (transform.forward * -cameraMovementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newCameraPosition += (transform.right * cameraMovementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newCameraPosition += (transform.right * -cameraMovementSpeed);
        }
        CheckFutureCameraPositon(newCameraPosition);
        transform.position = Vector3.Lerp(transform.position, newCameraPosition, Time.deltaTime * cameraMovementTime);
    }

    private void GetMovementSpeedInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraMovementSpeed = fastCameraSpeed;
        }
        else
        {
            cameraMovementSpeed = defaultCameraSpeed;
        }
    }

    private void GetRotationInput()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            newCameraRotation *= Quaternion.Euler(Vector3.up * cameraRotationSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newCameraRotation *= Quaternion.Euler(Vector3.up * -cameraRotationSpeed);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newCameraRotation, Time.deltaTime * cameraMovementTime);
    }

    private void ResetCamera()
    {
        newCameraPosition = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z), Time.deltaTime * (cameraMovementTime * 500));
        newCameraRotation = Quaternion.Euler(0, 0, 0);
    }

    private void GetZoomInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && mouseWheelCounter < mouseWheelRange)
        {
            newCameraZoom += cameraZoomSpeed;
            mouseWheelCounter++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && mouseWheelCounter > (mouseWheelRange * -1))
        {
            newCameraZoom -= cameraZoomSpeed;
            mouseWheelCounter--;
        }
        rtsCamera.localPosition = Vector3.Lerp(rtsCamera.localPosition, newCameraZoom, Time.deltaTime * cameraMovementTime);
    }

    // Function will be used when developing the game further, it is not integrated yet
    // Mouse
    private void MoveWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                newCameraPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }

    private void LeaveRTSView()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.Locked;
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<PlayerController>().ActvateFPS();
        }
    }

    private void CheckFutureCameraPositon(Vector3 currentCameraPosition)
    {
        newCameraPosition = new Vector3(
            Mathf.Clamp(currentCameraPosition.x, -terrainSizeX, terrainSizeX), 
            currentCameraPosition.y, 
            Mathf.Clamp(currentCameraPosition.z, -terrainSizeZ, terrainSizeZ));
    }
}
