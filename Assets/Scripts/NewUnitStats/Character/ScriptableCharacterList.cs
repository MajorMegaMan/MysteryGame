using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NewUnitStats
{
    [CreateAssetMenu(fileName = "New Character List", menuName = "NewUnitTests/Chracter List")]
    public class ScriptableCharacterList : ScriptableObject
    {
        [SerializeField] List<CharacterInfo> m_characters = new List<CharacterInfo>();

        public void AddCharacter(CharacterInfo characterInfo)
        {
            m_characters.Add(characterInfo.CreateCopy());
        }

        public CharacterInfo GetCharacterInfo(int index)
        {
            return m_characters[index];
        }
    }
}
