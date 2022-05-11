using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    [CreateAssetMenu(fileName = "New Saved Party", menuName = "Saved Party")]
    public class SavedParty : ScriptableObject
    {
        [SerializeField] ScriptableCharacterList m_characterList = null;
        [SerializeField] List<int> m_partyIndices = new List<int>();

        public void AddCharacterIndex(int index)
        {
            m_partyIndices.Add(index);
        }

        public void RemoveCharacterIndex(int index)
        {
            m_partyIndices.Remove(index);
        }

        public bool ContainsCharacterIndex(int index)
        {
            return m_partyIndices.Contains(index);
        }

        public int PartySize()
        {
            return m_partyIndices.Count;
        }

        public void RemovePartyIndex(int index)
        {
            m_partyIndices.RemoveAt(index);
        }

        public UnitCharacterInfo GetPartyCharacter(int index)
        {
            return m_characterList.GetCharacterInfo(m_partyIndices[index]);
        }
    }
}
