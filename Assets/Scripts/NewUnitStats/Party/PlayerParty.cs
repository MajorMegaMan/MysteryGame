using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class PlayerParty
    {
        List<UnitCharacterInfo> m_partyList;

        public UnitCharacterInfo leader { get { return m_partyList[0]; } }

        public PlayerParty()
        {
            m_partyList = new List<UnitCharacterInfo>();
        }

        public PlayerParty(SavedParty savedParty, GameMapTile leaderTile, NewUnitManager unitManager)
        {
            m_partyList = new List<UnitCharacterInfo>();
            CreateParty(savedParty, leaderTile, unitManager);
        }

        public void Add(UnitCharacterInfo characterInfo)
        {
            m_partyList.Add(characterInfo);
        }

        public void CreateParty(SavedParty savedParty, GameMapTile leaderTile, NewUnitManager unitManager)
        {
            if (savedParty.PartySize() == 0)
            {
                // party is not big enough
                return;
            }

            // Create Leader first
            UnitCharacterInfo leaderCharInfo = savedParty.GetPartyCharacter(0);
            UnitTokenStub leaderToken = unitManager.SpawnUnit(leaderCharInfo, leaderTile);
            leaderToken.SetController(UnitTokenController.GetUnitController(0));

            // Add to party
            Add(leaderToken.characterInfo);

            // Create the rest
            for (int i = 1; i < savedParty.PartySize(); i++)
            {
                UnitCharacterInfo charInfo = savedParty.GetPartyCharacter(i);
                UnitTokenStub token = unitManager.SpawnUnit(charInfo, leaderTile.GetNeighbour((Tile.NeighbourDirection)i) as GameMapTile);
                token.SetController(UnitTokenController.GetUnitController(1));

                // Add To Party
                Add(token.characterInfo);
            }
        }
    }
}
