using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    [System.Serializable]
    class PanelControl
    {
        public string panelName = "NewPanel";
        [SerializeField] GameObject m_panelObject = null;
        [SerializeField] KeyCode m_key = 0;
        IPanelState m_currentState = HidingMenuState.instance;

        public bool isShowing { get { return m_currentState == ShowingMenuState.instance; } }

        #region Functions
        public void Init()
        {
            m_currentState = HidingMenuState.instance;
            m_currentState.Enter(this);
        }

        public bool GetKeyDown()
        {
            return Input.GetKeyDown(m_key);
        }

        public void Toggle()
        {
            m_currentState.Invoke(this);
        }

        void SetPanelActive(bool isActive)
        {
            m_panelObject.SetActive(isActive);
        }

        public void ShowPanel()
        {
            SetCurrentState(ShowingMenuState.instance);
        }

        public void HidePanel()
        {
            SetCurrentState(HidingMenuState.instance);
        }

        void SetCurrentState(IPanelState panelState)
        {
            m_currentState = panelState;
            panelState.Enter(this);
        }
        #endregion

        #region HelperClasses
        interface IPanelState
        {
            void Invoke(PanelControl panelControl);

            void Enter(PanelControl panelControl);
        }

        class ShowingMenuState : SingletonBase<ShowingMenuState>, IPanelState
        {
            public void Invoke(PanelControl panelControl)
            {
                panelControl.SetCurrentState(HidingMenuState.instance);
            }

            public void Enter(PanelControl panelControl)
            {
                panelControl.SetPanelActive(true);
            }
        }

        class HidingMenuState : SingletonBase<HidingMenuState>, IPanelState
        {
            public void Invoke(PanelControl panelControl)
            {
                panelControl.SetCurrentState(ShowingMenuState.instance);
            }

            public void Enter(PanelControl panelControl)
            {
                panelControl.SetPanelActive(false);
            }
        }
        #endregion
    }

    // Variables
    [SerializeField] PanelControl m_mainMenuPanel = new PanelControl();
    [SerializeField] List<PanelControl> m_panelkeyInputs = null;
    PanelControl m_currentActivePanel = null;

    // Start is called before the first frame update
    void Start()
    {
        m_mainMenuPanel.Init();
        for (int i = 0; i < m_panelkeyInputs.Count; i++)
        {
            m_panelkeyInputs[i].Init();
        }
        m_currentActivePanel = m_panelkeyInputs[0];
        m_currentActivePanel.ShowPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_mainMenuPanel.GetKeyDown())
        {
            // This panel has been activated
            m_mainMenuPanel.Toggle();
        }
        for (int i = 0; i < m_panelkeyInputs.Count; i++)
        {
            if(m_panelkeyInputs[i].GetKeyDown())
            {
                // This panel has been activated
                if(m_currentActivePanel != m_panelkeyInputs[i])
                {
                    if (!m_mainMenuPanel.isShowing)
                    {
                        m_mainMenuPanel.ShowPanel();
                    }

                    m_currentActivePanel.HidePanel();
                    m_currentActivePanel = m_panelkeyInputs[i];
                    m_currentActivePanel.Toggle();
                }
                else
                {
                    if (m_mainMenuPanel.isShowing)
                    {
                        m_mainMenuPanel.HidePanel();
                    }
                    else
                    {
                        m_mainMenuPanel.ShowPanel();
                    }
                }
            }
        }
    }
}
