using UnityEngine;
using UnityEngine.InputSystem;
public class CameraZoomScript : MonoBehaviour
{
    [SerializeField] private CinemachineCameraOffset zoomController;
    private float currentXAmount;
    private float currentYAmount;
    private float currentZAmount;
    [SerializeField] private InputActionReference scrollFunction;
    [SerializeField] private InputActionReference cameraMovePan;
    [SerializeField] private InputActionReference cameraPanActivion;
    private float currentZoomAmount;
    private float stampedZoom;
    private bool isPanningAllowed = false;
    
    private void Start()
    {
        currentXAmount = zoomController.m_Offset.x;
        currentYAmount = zoomController.m_Offset.y;
        currentZAmount = zoomController.m_Offset.z;
    }
    
    private void OnEnable()
    {
            scrollFunction.action.performed += FirstScroll;
            /*cameraMovePan.action.performed += StartPanCameraMovement;
            cameraPanActivion.action.performed -= DoNotAllowPanCamera;
            cameraPanActivion.action.performed += AllowPanCamera;*/
        
            scrollFunction.action.Enable();
            /*cameraMovePan.action.Enable();
            cameraPanActivion.action.Enable();*/
    }

    private void OnDisable()
    {
            scrollFunction.action.performed -= FirstScroll;
            /*cameraMovePan.action.performed -= StartPanCameraMovement;
            cameraPanActivion.action.performed -= AllowPanCamera;
            cameraPanActivion.action.performed += DoNotAllowPanCamera;*/
            
            scrollFunction.action.Disable();
            /*cameraMovePan.action.Disable();
            cameraPanActivion.action.Disable();*/
    }

    private void FirstScroll(InputAction.CallbackContext context)
    {
        float scrollingDelta = scrollFunction.action.ReadValue<Vector2>().y;

        currentZoomAmount += scrollingDelta * 0.1f;
        
        currentZoomAmount = Mathf.Clamp(currentZoomAmount, -0.4f, 1.4f);
        
        ChangedZoomAmount(currentZoomAmount);
    }

    /*private void StartPanCameraMovement(InputAction.CallbackContext context)
    {
        if (isPanningAllowed)
        {
            Debug.Log("PanningIsAllowed");
            Vector2 panInputs = context.ReadValue<Vector2>();
            panInputs.x = currentXAmount;
            panInputs.y = currentYAmount;
            PanCamera(panInputs);
        }
    }*/
    
    /*private void AllowPanCamera(InputAction.CallbackContext context)
    {
        isPanningAllowed = true;
    }*/

    /*private void DoNotAllowPanCamera(InputAction.CallbackContext context)
    {
        isPanningAllowed = false;
    }*/

    private void ChangedZoomAmount(float zoomingInOrOut)
    {
        currentZAmount = Mathf.Clamp(currentZAmount, -20, -7);

        Debug.Log(zoomingInOrOut);

        if (zoomingInOrOut <= stampedZoom)
        {
            currentZAmount -= 1f;
            currentZAmount = zoomController.m_Offset.z = currentZAmount;
        }

        if (zoomingInOrOut >= stampedZoom)
        {
            currentZAmount += 1f;
            currentZAmount = zoomController.m_Offset.z = currentZAmount;
        }

        if (currentZAmount < -6 && currentZAmount > -21)
        {
            stampedZoom = zoomingInOrOut;
            Debug.ClearDeveloperConsole();
        }

        /*if (currentZAmount == -20)
        {
            stampedZoom = zoomingInOrOut;
        }*/

}

    /*private void PanCamera(Vector2 panDelta)
    {
        Debug.Log("Changing The X & Y Values");
        zoomController.m_Offset.x = panDelta.x;
        zoomController.m_Offset.y = panDelta.y;
    }*/
}
