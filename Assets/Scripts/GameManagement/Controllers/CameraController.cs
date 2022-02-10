using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Camera m_camera = null;

    [SerializeField] Unit m_followTarget = null;
    Transform m_followTransform = null;

    [SerializeField] Vector3 m_camOffset = Vector3.zero;
    [SerializeField] float m_camDistance = 5.0f;

    delegate void VoidAction();
    VoidAction m_moveAction = () => { };

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_moveAction.Invoke();
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
}
