using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    public class UnitTokenStub : GameMapToken, ITurnTaker, IPooledObject
    {
        UnitCharacterInfo m_characterInfo = null;

        UnitTokenController m_unitController = null;
        Tile.NeighbourDirection m_currentLookDirection = 0;

        NewModelObject m_modelObject = null;

        public Tile.NeighbourDirection currentLookDirection { get { return m_currentLookDirection; } }
        public UnitCharacterInfo characterInfo { get { return m_characterInfo; } }

        public void GenerateUnitFromProfile(UnitProfile unitProfile)
        {
            m_characterInfo = new UnitCharacterInfo(unitProfile, this);
        }

        public void GenerateFromCharacterInfo(UnitCharacterInfo characterInfo)
        {
            m_characterInfo = characterInfo.CreateCopy();
            m_characterInfo.SetToken(this);
        }

        public void SetController(UnitTokenController controller)
        {
            m_unitController = controller;
        }    

        public void SetModelObject(NewModelObject modelObject)
        {
            m_modelObject = modelObject;
            modelObject.transform.SetParent(transform);
            modelObject.transform.localPosition = Vector3.zero;
        }

        public override int GetTokenID()
        {
            return (int)TempTokenID.unit;
        }

        public void LookTowards(Tile.NeighbourDirection direction)
        {
            m_currentLookDirection = direction;

            // For now, dodgy way to get the unit to face the direction it's moving
            Vector3 assumedNeighbourOffset = currentTile.GetNeighbourOffset(direction);
            transform.forward = assumedNeighbourOffset;
        }

        #region ITurnTaker
        float ITurnTaker.GetTurnValue()
        {
            return m_characterInfo.unitStats.GetStat(CoreStatKey.speed).value;
        }

        ITurnAction ITurnTaker.FindUnitTurnAction()
        {
            ITurnAction turnAction = m_unitController.FindUnitTurnAction(this, out Tile.NeighbourDirection direction);
            LookTowards(direction);
            return turnAction;
        }

        bool ITurnTaker.IsEngaged()
        {
            return isMoving;
        }

        void ITurnTaker.EndTurn()
        {
            //turnManager.EndCurrentTurn();
            NewTurnManager.EndCurrentTurn();
        }
        #endregion

        #region IPooledObject
        void IPooledObject.SetIsActiveInPool(bool isActive)
        {
            gameObject.SetActive(isActive);
            if(isActive)
            {
                NewTurnManager.AddUnit(this);
            }
            else
            {
                NewTurnManager.RemoveUnit(this);
            }
        }
        #endregion

        public override void OnEnterTile(GameMapTile gameMapTile)
        {
            var itemToken = gameMapTile.GetToken(TempTokenID.item) as ItemTokenStub;
            if(itemToken != null)
            {
                Inventory.BaseItemStub item = itemToken.item;
                if (m_characterInfo.inventory.AddItem(itemToken.item))
                {
                    Debug.Log("Picked up " + item.itemName);
                    NewItemManager.ReleaseItemToken(itemToken);
                }
                else
                {
                    Debug.Log("Stepped on " + item.itemName);
                }
            }
        }
    }
}
