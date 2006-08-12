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
	public class FactionDao : IFactionDao
	{
		protected static readonly string c_rowFields = "`FactionId`,`Name`";
		private readonly MySqlState m_state;

		public virtual FactionEntity Find(int faction1)
		{
			FactionEntity result = new FactionEntity();

			m_state.ExecuteQuery(
				"SELECT " + c_rowFields + " FROM `faction` WHERE `FactionId`='" + m_state.EscapeString(faction1.ToString()) + "'",
				CommandBehavior.SingleRow,
				delegate(MySqlDataReader reader)
				{
					reader.Read();
					FillEntityWithRow(ref result, reader);
				}
			);

			return result;
		}

		public virtual void Create(FactionEntity obj)
		{
			m_state.ExecuteNonQuery(
				"INSERT INTO `faction` VALUES (`" + obj.Faction1.ToString() + "`,`" + obj.Name.ToString() + "`);");
		}

		public virtual void Update(FactionEntity obj)
		{
			m_state.ExecuteNonQuery(
				"UPDATE `faction` SET `FactionId`='" + m_state.EscapeString(obj.Faction1.ToString()) + "', `Name`='" + m_state.EscapeString(obj.Name.ToString()) + "' WHERE `FactionId`='" + m_state.EscapeString(obj.Faction1.ToString()) + "'");
		}

		public virtual void Delete(FactionEntity obj)
		{
			m_state.ExecuteNonQuery(
				"DELETE FROM `faction` WHERE `FactionId`='" + m_state.EscapeString(obj.Faction1.ToString()) + "'");
		}

		public virtual void SaveAll()
		{
			// not used by this implementation
		}

		public virtual IList<FactionEntity> SelectAll()
		{
			FactionEntity entity;
			List<FactionEntity> results = null;

			m_state.ExecuteQuery(
				"SELECT " + c_rowFields + " FROM `faction`",
				CommandBehavior.Default,
				delegate(MySqlDataReader reader)
				{
					results = new List<FactionEntity>(reader.FieldCount);
					while (reader.Read())
					{
						entity = new FactionEntity();
						FillEntityWithRow(ref entity, reader);
						results.Add(entity);
					}
				}
			);

			return results;
		}

		public virtual int CountAll()
		{
			return (int)m_state.ExecuteScalar(
			"SELECT COUNT(*) FROM `faction`");

		}

		protected virtual void FillEntityWithRow(ref FactionEntity entity, MySqlDataReader reader)
		{
			entity.Faction1 = reader.GetInt32(0);
			entity.Name = reader.GetString(1);
		}

		public virtual Type TransferObjectType
		{
			get { return typeof(FactionEntity); }
		}

		public IList<string> VerifySchema()
		{
			return null;
			m_state.ExecuteNonQuery("CREATE TABLE IF NOT EXISTS `faction` ("
				+"`FactionId` int,"
				+"`Name` varchar(510) character set unicode"
				+", primary key `FactionId` (`FactionId`)"
			);
		}

		public FactionDao(MySqlState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			m_state = state;
		}
	}
}
