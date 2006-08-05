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
 */

using System;
using System.Collections.Generic;
using System.Data;
using DOL.Database.DataAccessInterfaces;
using DOL.Database.DataTransferObjects;
using MySql.Data.MySqlClient;

namespace DOL.Database.MySql.DataAccessObjects
{
	public class GamePlayerInventoryDao : IGamePlayerInventoryDao
	{
		protected static readonly string c_rowFields = "`InventoryId`,`IsCloakHoodUp`";
		private readonly MySqlState m_state;

		public virtual GamePlayerInventoryEntity Find(int key)
		{
			GamePlayerInventoryEntity result = new GamePlayerInventoryEntity();

			m_state.ExecuteQuery(
				"SELECT " + c_rowFields + " FROM `gameplayerinventory` WHERE `InventoryId`='" + m_state.EscapeString(key.ToString()) + "'",
				CommandBehavior.SingleRow,
				delegate(MySqlDataReader reader)
				{
					FillEntityWithRow(ref result, reader);
				}
			);

			return result;
		}

		public virtual void Create(GamePlayerInventoryEntity obj)
		{
			m_state.ExecuteNonQuery(
				"INSERT INTO `gameplayerinventory` VALUES (`" + obj.Inventory.ToString() + "`,`" + obj.IsCloakHoodUp.ToString() + "`);");
		}

		public virtual void Update(GamePlayerInventoryEntity obj)
		{
			m_state.ExecuteNonQuery(
				"UPDATE `gameplayerinventory` SET `InventoryId`='" + m_state.EscapeString(obj.Inventory.ToString()) + "', `IsCloakHoodUp`='" + m_state.EscapeString(obj.IsCloakHoodUp.ToString()) + "' WHERE `InventoryId`='" + m_state.EscapeString(obj.Inventory.ToString()) + "'");
		}

		public virtual void Delete(GamePlayerInventoryEntity obj)
		{
			m_state.ExecuteNonQuery(
				"DELETE FROM `gameplayerinventory` WHERE `InventoryId`='" + m_state.EscapeString(obj.Inventory.ToString()) + "'");
		}

		public virtual void SaveAll()
		{
			// not used by this implementation
		}

		public virtual int CountAll()
		{
			return -1;
		}

		protected virtual void FillEntityWithRow(ref GamePlayerInventoryEntity entity, MySqlDataReader reader)
		{
			entity.Inventory = reader.GetInt32(0);
			entity.IsCloakHoodUp = reader.GetString(1);
		}

		public virtual Type TransferObjectType
		{
			get { return typeof(GamePlayerInventoryEntity); }
		}

		public IList<string> VerifySchema()
		{
			return null;
			m_state.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS `gameplayerinventory` ("
				+"`InventoryId` int,"
				+"`IsCloakHoodUp` char(1) character set ascii"
				+", primary key `InventoryId` (`InventoryId`)"
			);
		}

		public GamePlayerInventoryDao(MySqlState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			m_state = state;
		}
	}
}
