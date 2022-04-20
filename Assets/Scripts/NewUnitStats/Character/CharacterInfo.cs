using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewUnitStats.Inventory;

namespace NewUnitStats
{
    [System.Serializable]
    public class CharacterInfo
    {
        [SerializeField] string m_characterName;

        [SerializeField] UnitProfile m_profileBase = null;
        [SerializeField] UnitStats m_unitStats = null;
        [SerializeField] Inventory.Inventory<CharacterInfo, BaseItemStub> m_inventory = null;

        [SerializeField] GameMapTile.Access m_tileAccess = 0;

        // Token reference is only used during runtime. It has no meaningful connection to character data and therefor does not need to be serialized.
        // Token reference is NOT copied with the copy function.
        [System.NonSerialized] UnitTokenStub m_token = null;

        // getters
        public UnitStats unitStats { get { return m_unitStats; } }
        public UnitProfile profileBase { get { return m_profileBase; } }
        public Inventory.Inventory<CharacterInfo, BaseItemStub> inventory { get { return m_inventory; } }

        public GameMapTile.Access tileAccess { get { return m_tileAccess; } }

        CharacterInfo()
        {

        }

        public CharacterInfo(UnitProfile unitProfile, UnitTokenStub token)
        {
            m_profileBase = unitProfile;
            m_unitStats = new UnitStats(unitProfile.baseValues);
            m_inventory = new NewUnitStats.Inventory.Inventory<CharacterInfo, BaseItemStub>(this);

            m_token = token;
        }

        public CharacterInfo CreateCopy()
        {
            CharacterInfo copy = new CharacterInfo();
            copy.m_characterName = m_characterName;
            copy.m_profileBase = m_profileBase;
            copy.m_unitStats = new UnitStats(m_unitStats);
            copy.m_inventory = new NewUnitStats.Inventory.Inventory<CharacterInfo, BaseItemStub>(copy, m_inventory);
            copy.m_tileAccess = m_tileAccess;
            return copy;
        }
    }
}
