using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    [CreateAssetMenu(fileName = "New Character List", menuName = "NewUnitTests/Chracter List")]
    public class ScriptableCharacterList : ScriptableObject
    {
        [SerializeField] List<UnitCharacterInfo> m_characters = new List<UnitCharacterInfo>();

        public void AddCharacter(UnitCharacterInfo characterInfo)
        {
            m_characters.Add(characterInfo.CreateCopy());
        }

        public UnitCharacterInfo GetCharacterInfo(int index)
        {
            return m_characters[index];
        }
    }
}
