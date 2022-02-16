using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Camera m_camera = null;

    [SerializeField] Unit m_followTarget = null;
    Transform m_followTransform = null;

    [SerializeField] Vector3 m_camOffset = Vector3.zero;
    [SerializeField] float m_camDistance = 5.0f;

    public delegate void VoidAction();
    VoidAction m_moveAction = () => { };
    VoidAction m_onRenderEvent = null;

    public Camera cam { get { return m_camera; } }

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    void SetCamera()
    {
        transform.position = m_followTransform.position - transform.forward * m_camDistance + m_camOffset;
    }

    public void SetFollowTarget(Unit targetUnit)
    {
        m_followTarget = targetUnit;
        if (m_followTarget != null)
        {
            m_followTransform = targetUnit.transform;
            m_moveAction = SetCamera;
        }
        else
        {
            m_followTransform = null;
            m_moveAction = () => { };
        }
    }

    private void OnValidate()
    {
        SetFollowTarget(m_followTarget);
    }

    public void AddRenderEvent(VoidAction action)
    {
        m_onRenderEvent += action;
    }

    public void RemoveRenderEvent(VoidAction action)
    {
        m_onRenderEvent -= action;
    }

    // update every render step to ensure that the camera is following the target
    void OnURPRender(ScriptableRenderContext context, Camera camera)
    {
        // Follow Transform
        m_moveAction.Invoke();

        m_onRenderEvent?.Invoke();
    }

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnURPRender;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnURPRender;
    }
}
