using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class NewGameManager : MonoBehaviour
    {
        [SerializeField] GameMap m_gameMap = null;
        [SerializeField] CameraController m_cameraController = null;

        [SerializeField] PlayerHUD m_playerHUD = null;

        PlayerParty m_playerParty = null;

        [Header("Initialisation")]
        [SerializeField] NewUnitManagerPackage m_unitManagerPackage = null;
        [SerializeField] NewItemManagerPackage m_itemManagerPackage = null;
        [SerializeField] SavedParty m_savedParty = null;

        [Header("Debug")]
        [SerializeField] ScriptableCharacterList debugSavedCharacters = null;
        [SerializeField] Inventory.BaseItemStub debugItem = null;

        private void Awake()
        {
            NewUnitManager.instance.Init(m_unitManagerPackage);
            NewItemManager.instance.Init(m_itemManagerPackage);

            UnitTokenController.CreateController<PlayerTokenController>();
            UnitTokenController.CreateController<DebugTokenController>();
        }

        private void Start()
        {
            CreateParty();
            m_cameraController.SetFollowTarget(m_playerParty.leader.token.transform);

            m_playerHUD.SetPlayerUnit(m_playerParty.leader);

            var newItem = NewItemManager.SpawnItemToken(debugItem, m_gameMap.startRoom.GetRandomTile() as GameMapTile);
        }

        private void Update()
        {
            NewTurnManager.Update();
        }

        void CreateParty()
        {
            GameMapTile leaderTile = m_gameMap.GetTile(m_gameMap.startRoom.GetCentre());
            m_playerParty = new PlayerParty(m_savedParty, leaderTile, NewUnitManager.instance);
        }
    }
}
