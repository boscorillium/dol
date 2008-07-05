/*
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

//Thanks to Eden (Vico) for creating this - Edited by IST
using System;
using DOL.GS;
using DOL.Events;
using System.Threading;
using DOL.GS.Scripts;
using DOL.GS.PacketHandler;
using log4net;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using DOL.AI.Brain;
using DOL.GS.Movement;

namespace DOL.GS.GameEvents
{
	public static class RegionTimersResynch
	{
		const int UPDATE_INTERVAL = 10 * 1000; //10sec

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		static Timer m_timer;
		static Stopwatch watch;
		static Dictionary<GameTimer.TimeManager, long> old_time = new Dictionary<GameTimer.TimeManager, long>();

		#region Initialization/Teardown

		[ScriptLoadedEvent]
		public static void OnScriptCompiled(DOLEvent e, object sender, EventArgs args)
		{
			if (ServerProperties.Properties.USE_SYNC_UTILITY)
				Init();
		}
		[ScriptUnloadedEvent]
		public static void OnScriptUnloaded(DOLEvent e, object sender, EventArgs args) 
		{
			if (ServerProperties.Properties.USE_SYNC_UTILITY)
				Stop();
		}

		public static void Init()
		{
			watch = new Stopwatch();
			watch.Start();
			foreach (GameTimer.TimeManager mgr in WorldMgr.GetRegionTimeManagers())
				old_time.Add(mgr, 0);

			m_timer = new Timer(new TimerCallback(Resynch), null, 0, UPDATE_INTERVAL);
		}

		public static void Stop()
		{
			if (m_timer != null)
				m_timer.Dispose();
		}

		#endregion

		private static void Resynch(object nullValue)
		{
			log.Info("----- Region Timer Resynch -----");

			long syncTime = watch.ElapsedMilliseconds;

			//Check alive
			foreach (GameTimer.TimeManager mgr in WorldMgr.GetRegionTimeManagers())
			{
				if (old_time.ContainsKey(mgr) && old_time[mgr] == mgr.CurrentTime)
				{
					log.Error(string.Format("----- Found Frozen Region Timer -----\nName: {0} - Current Time: {1}", mgr.Name, mgr.CurrentTime));

					if(mgr.Running)
						mgr.Stop();

					foreach (GameClient clients in WorldMgr.GetAllClients())
					{
						if (clients.Player == null || clients.ClientState == GameClient.eClientState.Linkdead)
						{
							log.Error(string.Format("----- Disconnected Client: {0}", clients.Account.Name));
							if (clients.Player != null)
							{
								clients.Player.SaveIntoDatabase();
								clients.Player.Quit(true);
							}
							clients.Out.SendPlayerQuit(true);
							clients.Disconnect();
							GameServer.Instance.Disconnect(clients);
							WorldMgr.RemoveClient(clients);
						}
					}

					mgr.Start();
					
                    foreach (Region reg in WorldMgr.GetAllRegions())
					{
						if (reg.TimeManager == mgr)
						{							
							foreach (GameObject obj in reg.Objects)
							{
								//Restart Player regen & remove PvP immunity
								if (obj is GamePlayer)
								{
									GamePlayer plr = obj as GamePlayer;
									if (plr.IsAlive)
									{
										plr.StopHealthRegeneration();
										plr.StopPowerRegeneration();
										plr.StopEnduranceRegeneration();
										plr.StartHealthRegeneration();
										plr.StartPowerRegeneration();
										plr.StartEnduranceRegeneration();
										plr.SetPvPInvulnerability(1000, null);
									}
								}
								//Restart Brains & Paths
								if (obj is GameNPC && (obj as GameNPC).Brain != null && !((obj as GameNPC).Brain is BlankBrain))
                                {
									GameNPC npc = obj as GameNPC;
									npc.Brain.Stop();
									if (npc.MaxSpeedBase > 0 && npc.PathID != null && npc.PathID != "" && npc.PathID != "NULL")
									{
										npc.StopMoveOnPath();
										PathPoint path = MovementMgr.LoadPath(npc.PathID);
										npc.CurrentWayPoint = path;
										npc.MoveOnPath(path.MaxSpeed);
									}
									npc.Brain.Start();
								}
							}
						}					
					}

					//RegionTimerUnfrozen(mgr, syncTime);
				}

				if (old_time.ContainsKey(mgr))
					old_time[mgr] = syncTime;
				mgr.CurrentTime = syncTime;
			}
		}

		public delegate void RegionTimerHandler(GameTimer.TimeManager RestartedTimer, long SyncTime);
		public static event RegionTimerHandler RegionTimerUnfrozen;
	}
}