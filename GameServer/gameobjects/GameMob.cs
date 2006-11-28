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
using System.Reflection;
using System.Collections;
using DOL.AI.Brain;
using DOL.Database;
using DOL.GS.PacketHandler;
using DOL.GS.Styles;
using log4net;

namespace DOL.GS
{

	/// <summary>
	/// This class represents a monster in the game!
	/// </summary>
	public class GameMob : GameNPC
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		#region Spell/Style

		#endregion
		#region Database
		/// <summary>
		/// Saves a mob into the db if it exists, it is
		/// updated, else it creates a new object in the DB
		/// </summary>
		public override void SaveIntoDatabase()
		{
			Mob mob = null;
			if (InternalID != null)
				mob = (Mob)GameServer.Database.FindObjectByKey(typeof(Mob), InternalID);
			if (mob == null)
				mob = new Mob();

			mob.Name = Name;
			mob.Guild = GuildName;
			mob.X = X;
			mob.Y = Y;
			mob.Z = Z;
			mob.Heading = Heading;
			mob.Speed = MaxSpeedBase;
			mob.Region = CurrentRegionID;
			mob.Realm = Realm;
			mob.Model = Model;
			mob.Size = Size;
			mob.Level = Level;
			mob.ClassType = this.GetType().ToString();
			mob.Flags = Flags;
			IAggressiveBrain aggroBrain = Brain as IAggressiveBrain;
			if (aggroBrain != null)
			{
				mob.AggroLevel = aggroBrain.AggroLevel;
				mob.AggroRange = aggroBrain.AggroRange;
			}
			mob.EquipmentTemplateID = EquipmentTemplateID;
			if (m_faction != null)
				mob.FactionID = m_faction.ID;
			mob.MeleeDamageType = (int)MeleeDamageType;
			if (NPCTemplate != null)
				mob.NPCTemplateID = NPCTemplate.TemplateId;

			if (InternalID == null)
			{
				GameServer.Database.AddNewObject(mob);
				InternalID = mob.ObjectId;
			}
			else
				GameServer.Database.SaveObject(mob);
		}

		/// <summary>
		/// Loads a merchant from the DB
		/// </summary>
		/// <param name="mobobject">The mob DB object</param>
		public override void LoadFromDatabase(DataObject mobobject)
		{
			base.LoadFromDatabase(mobobject);
			if (!(mobobject is Mob)) return;
			Mob currentMob = (Mob)mobobject;

			m_respawnInterval = currentMob.RespawnInterval * 1000;

			IAggressiveBrain aggroBrain = Brain as IAggressiveBrain;
			if (aggroBrain != null)
			{
				if (currentMob.AggroRange == 0)
				{
					if (Char.IsLower(Name[0]))
					{ // make some default on basic mobs
						aggroBrain.AggroRange = 450;
						aggroBrain.AggroLevel = (Level > 3) ? 30 : 0;
					}
					else if (Realm != 0)
					{
						aggroBrain.AggroRange = 500;
						aggroBrain.AggroLevel = 60;
					}
				}
				else
				{
					aggroBrain.AggroLevel = currentMob.AggroLevel;
					aggroBrain.AggroRange = currentMob.AggroRange;
				}
			}
		}

		/// <summary>
		/// Deletes the mob from the database
		/// </summary>
		public override void DeleteFromDatabase()
		{
			if (InternalID != null)
			{
				Mob mob = (Mob)GameServer.Database.FindObjectByKey(typeof(Mob), InternalID);
				if (mob != null)
					GameServer.Database.DeleteObject(mob);
			}
		}
		#endregion
		#region Combat

		/// <summary>
		/// The time to wait before each mob respawn
		/// </summary>
		protected int m_respawnInterval;
		/// <summary>
		/// A timer that will respawn this mob
		/// </summary>
		protected RegionTimer m_respawnTimer;
		/// <summary>
		/// The sync object for respawn timer modifications
		/// </summary>
		protected readonly object m_respawnTimerLock = new object();
		/// <summary>
		/// The Respawn Interval of this mob in milliseconds
		/// </summary>
		public virtual int RespawnInterval
		{
			get
			{
				if (m_respawnInterval >= 0)
					return m_respawnInterval;

				//Standard 5-8 mins
				if (Level <= 65 || Realm != 0)
				{
					return Util.Random(5 * 60000) + 3 * 60000;
				}
				else
				{
					int minutes = Level - 65 + 15;
					return minutes * 60000;
				}
			}
			set
			{
				m_respawnInterval = value;
			}
		}

		/// <summary>
		/// Is the mob alive
		/// </summary>
		public override bool IsAlive
		{
			get
			{
				bool alive = base.IsAlive;
				if (alive && IsRespawning)
					return false;
				return alive;
			}
		}

		/// <summary>
		/// Is the mob respawning
		/// </summary>
		public bool IsRespawning
		{
			get
			{
				if (m_respawnTimer == null)
					return false;
				return m_respawnTimer.IsAlive;
			}
		}

		/// <summary>
		/// Starts the Respawn Timer
		/// </summary>
		public virtual void StartRespawn()
		{
			if (IsAlive) return;

			if (this.Brain is IControlledBrain)
				return;

			int respawnInt = RespawnInterval;
			if (respawnInt > 0)
			{
				lock (m_respawnTimerLock)
				{
					if (m_respawnTimer == null)
					{
						m_respawnTimer = new RegionTimer(this);
						m_respawnTimer.Callback = new RegionTimerCallback(RespawnTimerCallback);
					}
					else if (m_respawnTimer.IsAlive)
					{
						m_respawnTimer.Stop();
					}
					m_respawnTimer.Start(respawnInt);
				}
			}
		}
		/// <summary>
		/// The callback that will respawn this mob
		/// </summary>
		/// <param name="respawnTimer">the timer calling this callback</param>
		/// <returns>the new interval</returns>
		protected virtual int RespawnTimerCallback(RegionTimer respawnTimer)
		{
			lock (m_respawnTimerLock)
			{
				if (m_respawnTimer != null)
				{
					m_respawnTimer.Stop();
					m_respawnTimer = null;
				}
			}

			//DOLConsole.WriteLine("respawn");
			//TODO some real respawn handling
			if (IsAlive) return 0;
			if (ObjectState == eObjectState.Active) return 0;

			//Heal this mob, move it to the spawnlocation
			Health = MaxHealth;
			Mana = MaxMana;
			Endurance = MaxEndurance;
			int origSpawnX = m_spawnX;
			int origSpawnY = m_spawnY;
			//X=(m_spawnX+Random(750)-350); //new SpawnX = oldSpawn +- 350 coords
			//Y=(m_spawnY+Random(750)-350);	//new SpawnX = oldSpawn +- 350 coords
			X = m_spawnX;
			Y = m_spawnY;
			Z = m_spawnZ;
			AddToWorld();
			m_spawnX = origSpawnX;
			m_spawnY = origSpawnY;
			return 0;
		}

		/// <summary>
		/// Callback timer for health regeneration
		/// </summary>
		/// <param name="selfRegenerationTimer">the regeneration timer</param>
		/// <returns>the new interval</returns>
		protected override int HealthRegenerationTimerCallback(RegionTimer selfRegenerationTimer)
		{
			int period = m_healthRegenerationPeriod;
			if (!InCombat || Util.Chance(50)) // mobs have only 50% chance to heal itself 
			{
				period = base.HealthRegenerationTimerCallback(selfRegenerationTimer);
				BroadcastUpdate();
			}
			return (Health < MaxHealth) ? period : 0;
		}

		/// <summary>
		/// The chance for a critical hit
		/// </summary>
		public override int AttackCriticalChance(InventoryItem weapon)
		{
			return 0;
		}

		/// <summary>
		/// Called when the mob should die
		/// </summary>
		/// <param name="killer">the killer of this mob</param>
		public override void Die(GameObject killer)
		{
			if (IsWorthReward)
				DropLoot(killer);

			base.Die(killer);

			if ((Faction != null) && (killer is GamePlayer))
			{
				GamePlayer player = killer as GamePlayer;
				Faction.KillMember(player);
			}
			StartRespawn();
		}

		/// <summary>
		/// Called to attack a target!
		/// </summary>
		/// <param name="attackTarget">the target to attack</param>
		public override void StartAttack(GameObject attackTarget)
		{
			base.StartAttack(attackTarget);
			if (AttackState)
			{
				if (ActiveWeaponSlot == eActiveWeaponSlot.Distance)
					Follow(attackTarget, 1000, 5000);	// follow at archery range
				else
					Follow(attackTarget, 90, 5000);	// follow at stickrange
			}
		}

		/// <summary>
		/// Stops all attack actions, including following target
		/// </summary>
		public override void StopAttack()
		{
			base.StopAttack();
			StopFollow();
		}

		/// <summary>
		/// This method is called to drop loot after this mob dies
		/// </summary>
		/// <param name="killer">The killer</param>
		public virtual void DropLoot(GameObject killer)
		{
			// TODO: mobs drop "a small chest" sometimes
			ArrayList dropMessages = new ArrayList();
			lock (m_xpGainers.SyncRoot)
			{
				if (m_xpGainers.Keys.Count == 0) return;

				ItemTemplate[] lootTemplates = LootMgr.GetLoot(this, killer);

				foreach (ItemTemplate lootTemplate in lootTemplates)
				{
					GameStaticItem loot;
					if (GameMoney.IsItemMoney(lootTemplate.Name))
					{
						loot = new GameMoney(lootTemplate.Value, this);
						loot.Name = lootTemplate.Name;
						loot.Model = (ushort)lootTemplate.Model;
					}
					else
					{
						loot = new GameInventoryItem(new InventoryItem(lootTemplate));
						loot.X = X;
						loot.Y = Y;
						loot.Z = Z;
						loot.Heading = Heading;
						loot.CurrentRegion = CurrentRegion;
						if (((GameInventoryItem)loot).Item.Id_nb == "aurulite")
						{
							((GameInventoryItem)loot).Item.Count = ((GameInventoryItem)loot).Item.PackSize;
						}
					}

					bool playerAttacker = false;
					foreach (GameObject gainer in m_xpGainers.Keys)
					{
						if (gainer is GamePlayer)
						{
							playerAttacker = true;
							if (loot.Realm == 0)
								loot.Realm = ((GamePlayer)gainer).Realm;
						}
						loot.AddOwner(gainer);
						if (gainer is GameNPC)
						{
							IControlledBrain brain = ((GameNPC)gainer).Brain as IControlledBrain;
							if (brain != null)
							{
								playerAttacker = true;
								loot.AddOwner(brain.Owner);
							}
						}
					}
					if (!playerAttacker) return; // no loot if mob kills another mob

					//Only add money loot if not killing grays
					dropMessages.Add(GetName(0, true) + " drops " + loot.GetName(1, false) + ".");
					loot.AddToWorld();
				}
			}

			if (dropMessages.Count > 0)
			{
				GamePlayer killerPlayer = killer as GamePlayer;
				if (killerPlayer != null)
				{
					foreach (string str in dropMessages)
						killerPlayer.Out.SendMessage(str, eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
				}
				foreach (GamePlayer player in GetPlayersInRadius(WorldMgr.INFO_DISTANCE))
				{
					if (player == killer) continue;
					foreach (string str in dropMessages)
						player.Out.SendMessage(str, eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
				}
			}
		}

		#endregion
		#region GetExamineMessages/GetAggroLevelString
		/// <summary>
		/// How friendly this mob is to player
		/// </summary>
		/// <param name="player">GamePlayer that is examining this object</param>
		/// <param name="firstLetterUppercase"></param>
		/// <returns>aggro state as string</returns>
		public override string GetAggroLevelString(GamePlayer player, bool firstLetterUppercase)
		{
			// "aggressive towards you!", "hostile towards you.", "neutral towards you.", "friendly."
			// TODO: correct aggro strings
			string aggroLevelString = "";
			int aggroLevel;
			if (Faction != null)
			{
				aggroLevel = Faction.GetAggroToFaction(player);
				if (aggroLevel > 75)
					aggroLevelString = "aggressive";
				else if (aggroLevel > 50)
					aggroLevelString = "hostile";
				else if (aggroLevel > 25)
					aggroLevelString = "neutral";
				else
					aggroLevelString = "friendly";
			}
			else
				aggroLevelString = base.GetAggroLevelString(player, firstLetterUppercase);
			return aggroLevelString + " towards you.";
		}

		/// <summary>
		/// Adds messages to ArrayList which are sent when object is targeted
		/// </summary>
		/// <param name="player">GamePlayer that is examining this object</param>
		/// <returns>list with string messages</returns>
		public override IList GetExamineMessages(GamePlayer player)
		{
			IList list = base.GetExamineMessages(player);
			list.Add("You examine " + GetName(0, false) + ".  " + GetPronoun(0, true) + " is " + GetAggroLevelString(player, false));
			return list;
		}
		#endregion
		#region Delete

		/// <summary>
		/// Marks this object as deleted!
		/// </summary>
		public override void Delete()
		{
			lock (m_respawnTimerLock)
			{
				if (m_respawnTimer != null)
				{
					m_respawnTimer.Stop();
					m_respawnTimer = null;
				}
			}
			base.Delete();
		}

		#endregion

		/// <summary>
		/// Constructor to create a new mob
		/// </summary>
		public GameMob()
			: base()
		{
			Level = 1; // health changes when GameNPC.Level changes
			Realm = 0;
			Name = "new Mob";
			Model = 408;
			//Fill the living variables
			//			CurrentSpeed = 0; // cause position addition recalculation
			MaxSpeedBase = 100;
			GuildName = "";
			Size = 50;
			m_healthRegenerationPeriod = 20000;
		}

		/// <summary>
		/// Create a GameMob from a NPC template
		/// </summary>
		/// <param name="template"></param>
		public GameMob(INpcTemplate template)
			: base(template)
		{ }
	}
}
