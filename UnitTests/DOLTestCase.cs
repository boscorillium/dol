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
using System.IO;
using System.Net.Sockets;
using DOL.GS.Database;
using DOL.GS.PacketHandler;
using NHibernate.Expression;
using DOL.GS;
using NUnit.Framework;

namespace DOL.Tests
{	
	public class DOLTestCase
	{
		public DOLTestCase()
		{
		}

		protected GamePlayer CreateMockGamePlayer()
		{
			Account account = (Account) GameServer.Database.FindObjectByKey(typeof(Account),"d");
			
			Assert.IsNotNull(account);

            GamePlayer character = (GamePlayer)GameServer.Database.SelectObject(typeof(GamePlayer), Expression.Eq("AccountID", account.AccountID));
			
			Assert.IsNotNull(character);
			

			GameClient client = new GameClient(GameServer.Instance);
			client.Version = GameClient.eClientVersion.Version168;
			client.Out = new PacketLib168(client);
			client.Socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			client.Account = account;

			WorldMgr.CreateSessionID(client);
			GamePlayer player = new GamePlayer();
			client.Player = player;
			client.ClientState = GameClient.eClientState.CharScreen;
			return player;
		}

		[TestFixtureSetUp] public virtual void Init()
		{
			Directory.SetCurrentDirectory("../../../debug");
			Console.WriteLine(Directory.GetCurrentDirectory());
			if(GameServer.Instance==null)
			{
				FileInfo configFile = new FileInfo("../debug/config/serverconfig.xml");
				GameServerConfiguration config = new GameServerConfiguration();
				if(!configFile.Exists)
					config.SaveToXMLFile(configFile);
				else
					config.LoadFromXMLFile(configFile);
				GameServer.CreateInstance(config);
			}
			if (!GameServer.Instance.IsRunning)
			{
				Console.WriteLine("Starting GameServer");
				if (!GameServer.Instance.Start())
				{
					Console.WriteLine("Error init GameServer");
				}
			}
			else
			{
				Console.WriteLine("GameServer already running, skip init of Gameserver...");
			}
		}

		[TestFixtureTearDown] public virtual void Dispose()
		{
			// At the moment we do not stop GameServer after each test to let it be reused by all tests.
			// TODO Find a way to Startup/Stop GameServer once for all TestCases...
			// It could have been done with TestSuits, but they are now removed from NUnit, if I'm right.
			
			/*
			if (GameServer.IsRunning) 
			{
				GameServer.Instance.Stop();
				Console.WriteLine("GameServer stopped");
			} 
			else
			{
				Console.WriteLine("GameServer is not running, skip stop of Gameserver...");
			}
			*/
			
		}

		#region Watch

		static long gametick;		

		/// <summary>
		/// use startWatch to start taking the time
		/// </summary>
		public static void StartWatch()
		{
			//Tickcount is more accurate than gametimer ticks :)
			gametick = Environment.TickCount;
			Console.WriteLine("StartWatch: "+gametick);
		}

		/// <summary>
		/// stop watch will count the Gamticks since last call of startWatch
		/// 
		/// Note: This value does not represent the time it will take on a 
		/// actual server since we have no actual user load etc...
		/// </summary>
		public static void StopWatch()
		{	
			Console.WriteLine("Stop watch: "+Environment.TickCount);
			long elapsed = Environment.TickCount - gametick;
			Console.WriteLine(elapsed+" ticks(ms) elapsed");
		}
	
		#endregion
	}
}
