using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOL.Database;
using DOL.Database.Attributes;

namespace DOL
{
    namespace Database
    {
        /// <summary>
        /// Account table
        /// </summary>
        [DataTable(TableName = "CharacterSpellLine")]
        public class CharacterSpellLine : DataObject
        {
            private string name;
            private string spellLine;
            private int level;
            private bool enabled;

            [PrimaryKey]
            public string CharacterName
            {
                get
                {
                    return name;
                }
                set
                {
                    Dirty = true;
                    name = value;
                }
            }

            [DataElement(AllowDbNull = false)]
            public string SpellLine
            {
                get { return spellLine; }
                set { spellLine = value; }
            }

            [DataElement(AllowDbNull = false)]
            public int Level
            {
                get { return level; }
                set { Dirty = true; level = value; }
            }

            [DataElement(AllowDbNull = false)]
            public bool Enabled
            {
                get { return enabled; }
                set { Dirty = true; enabled = value; }
            }
        }
    }
}


