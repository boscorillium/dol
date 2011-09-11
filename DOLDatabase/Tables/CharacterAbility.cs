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
        [DataTable(TableName = "CharacterAbility")]
        public class CharacterAbility : DataObject
        {
            private string name;
            private bool enabled;
            private string ability;
            private int level;

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
            public bool Enabled
            {
                get { return enabled; }
                set { Dirty = true; enabled = value; }
            }

            [DataElement(AllowDbNull = false)]
            public string Ability
            {
                get { return ability; }
                set { ability = value; }
            }

            [DataElement(AllowDbNull = false)]
            public int Level
            {
                get { return level; }
                set { Dirty = true; level = value; }
            }
        }
    }
}
