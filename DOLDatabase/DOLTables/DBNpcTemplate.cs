/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 * 
 * This DB holds all information needed for all types of NPCs.
 * It just hold the Information about the different Types, not if and where it
 * is on the world. This is handled in MobDB. With this, it is possible to assign
 * Spells, Brains and other stuff to Mobs, that was just possible to get 
 * hardcoded before.
 */

using System;
using DOL.Database;
using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName="NpcTemplate")]
 	public class DBNpcTemplate : DataObject
	{
 		private int 		m_templateId;
 		private string		m_name = "";
 		private string		m_guildName = "";
		private ushort		m_model;
		private byte		m_size=50;
		private short		m_maxSpeed=50;
		private string		m_equipmentTemplateID = "";
		private bool		m_ghost;
		private byte		m_meleeDamageType = 1;
		private byte 		m_parryChance;
		private byte 		m_evadeChance;
		private byte 		m_blockChance;
		private byte 		m_leftHandSwingChance;
		private string		m_spells = "";
		private string		m_styles = "";

		private static bool	m_autoSave;

		public DBNpcTemplate()
		{
			m_autoSave = false;
		}

		public override bool AutoSave
		{
			get { return m_autoSave; }
			set { m_autoSave = value; }
		}

		[DataElement(AllowDbNull = false)]
		public int TemplateId
		{
			get { return m_templateId; }
			set
			{
				Dirty = true;
				m_templateId = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public string Name
		{
			get { return m_name; }
			set
			{
				Dirty = true;
				m_name = value;
			}
		}

		[DataElement(AllowDbNull=true)]
		public string GuildName
		{
			get { return m_guildName; }
			set
			{
				Dirty = true;
				m_guildName = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public ushort Model
		{
			get { return m_model; }
			set
			{
				Dirty = true;
				m_model = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public byte Size
		{
			get { return m_size; }
			set
			{
				Dirty = true;
				m_size = value;
			}
		}

		[DataElement(AllowDbNull=false)]
		public short MaxSpeed
		{
			get { return m_maxSpeed; }
			set
			{
				Dirty = true;
				m_maxSpeed = value;
			}
		}

		[DataElement(AllowDbNull=true)]
		public string EquipmentTemplateID
		{
			get { return m_equipmentTemplateID; }
			set
			{
				Dirty = true;
				m_equipmentTemplateID = value;
			}
		}

		[DataElement(AllowDbNull=true)]
		public bool Ghost
		{
			get { return m_ghost; }
			set
			{
				Dirty = true;
				m_ghost = value;
			}
		}

		[DataElement(AllowDbNull=true)]
		public byte MeleeDamageType
		{
			get { return m_meleeDamageType; }
			set
			{
				Dirty = true;
				m_meleeDamageType = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public byte ParryChance
		{
			get { return m_parryChance; }
			set
			{
				Dirty = true;
				m_parryChance = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public byte EvadeChance
		{
			get { return m_evadeChance; }
			set
			{
				Dirty = true;
				m_evadeChance = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public byte BlockChance
		{
			get { return m_blockChance; }
			set
			{
				Dirty = true;
				m_blockChance = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public byte LeftHandSwingChance
		{
			get { return m_leftHandSwingChance; }
			set
			{
				Dirty = true;
				m_leftHandSwingChance = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public string Spells
		{
			get { return m_spells; }
			set
			{
				Dirty = true;
				m_spells = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public string Styles
		{
			get { return m_styles; }
			set
			{
				Dirty = true;
				m_styles = value;
			}
		}
	}
}