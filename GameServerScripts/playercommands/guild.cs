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
using System.Collections;
using System.Reflection;
using DOL.Database;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS.Scripts
{
	[Cmd(
		"&gc",
		new string[] {"&guildcommand"},
		(uint) ePrivLevel.Player,
		"Guild command (use /gc help for options)",
		"/gc <option>")]
	public class GuildCommandHandler : ICommandHandler
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int OnCommand(GameClient client, string[] args)
		{
			try
			{
				if (args.Length == 1)
				{
					if (client.Account.PrivLevel > 1)
					{
						client.Out.SendMessage("Game Master commands:", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendMessage("'/gc create <Name> <player>' Create a new guild with player as leader", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendMessage("'/gc purge <Name>' Purge a guild completely", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendMessage("'/gc rename <OldName> to <NewName' Rename guild from OldName to NewName", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendMessage("'/gc addplayer <player> to <guild>' to add player to guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						client.Out.SendMessage("'/gc removeplayer <player> from <guild>' to remove player from a guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}

					client.Out.SendMessage("Member commands:", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc form <Name>' to create a new guilde with all player of group", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc info' to show information on the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc cancel <option> [value]' to cancel all command done before", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc decline' to decline the enter in guild ", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc claim' to claim a keep", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc quit' to leave the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc motd <text>' to set the message of the day of the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc amotd <text>' to set the message of the day of the alliance's guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc omotd <text>' to set the message of the day of the officier's guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc promote <rank#> [name]' to promote player to a superior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc demote <rank#> [name]' to set the rank from 0 to 10 of the player to an inferior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc remove <playername>' to remove the player from the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc emblem' to set emblem", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc edit' to show list of all option in guild edit", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc leader [name]' to set leader successor", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc accept ' to accept invite to guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc invite [name]' to invite targeted player to join the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc who' to show all player in your guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc list' to show all guild in your realm", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc aaccept' to accept an alliance invitation", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc acancel' to cancel an alliance invitation", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc adecline' to decline an alliance invitation", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc ainvite' to invite another guild to join your alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc aremove' to removes your entire guild from an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc aremove alliance [#]' to Remove the specified guild (listed by number) from the alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);


					return 1;
				}
				string message;

				switch (args[1])
				{
						// --------------------------------------------------------------------------------
						// CREATE
						// --------------------------------------------------------------------------------
					case "create":
						{
							if (client.Account.PrivLevel == (uint) ePrivLevel.Player)
								return 1;
							if (args.Length != 4)
							{
								client.Out.SendMessage("Syntax error! /gc create guildname leadername with no long name", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}
							if (!GuildMgr.DoesGuildExist(args[2]))
							{
								GameClient guildLeader = WorldMgr.GetClientByPlayerName(args[3], false, false);
								if (guildLeader == null)
								{
									client.Out.SendMessage("Player \"" + args[3] + "\"  not found.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
									return 0;
								}
								Guild newGuild = GuildMgr.CreateGuild(client.Player, args[2]);
								if (newGuild == null)
								{
									message = "Unable to create guild \"" + args[2] + "\".";
									client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
								}
								else
								{
									newGuild.AddPlayer(guildLeader.Player);
									guildLeader.Player.GuildRank = guildLeader.Player.Guild.GetRankByID(0);
									message = "Create guild \"" + args[2] + "\" with player " + args[3] + " as leader!";
									client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
								}
								return 1;
							}
							else
							{
								message = "Guild " + args[2] + " cannot be created because it already exsists!";
								client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// PURGE
						// --------------------------------------------------------------------------------
					case "purge":
						{
							if (client.Account.PrivLevel == (uint) ePrivLevel.Player)
								return 1;

							if (args.Length < 3)
							{
								client.Out.SendMessage("Syntax error! /gc purge guildname", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}
							string guildname = String.Join(" ", args, 2, args.Length - 2);
							if (!GuildMgr.DoesGuildExist(guildname))
							{
								client.Out.SendMessage("Guild does not exist!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (GuildMgr.DeleteGuild(guildname))
								client.Out.SendMessage(guildname + " purged!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// RENAME
						// --------------------------------------------------------------------------------
					case "rename":
						{
							if (client.Account.PrivLevel == (uint) ePrivLevel.Player)
								return 1;

							if (args.Length < 5)
							{
								client.Out.SendMessage("Syntax error! /gc rename guildname to newguildname", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}
							int i;
							for (i = 2; i < args.Length; i++)
							{
								if (args[i] == "to")
									break;
							}

							string oldguildname = String.Join(" ", args, 2, i - 2);
							string newguildname = String.Join(" ", args, i + 1, args.Length - i - 1);
							if (!GuildMgr.DoesGuildExist(oldguildname))
							{
								client.Out.SendMessage("Guild does not exist!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							Guild myguild = GuildMgr.GetGuildByName(oldguildname);
							myguild.theGuildDB.GuildName = newguildname;
							myguild.Name = newguildname;
							GuildMgr.AddGuild(myguild);
							foreach (GamePlayer ply in  myguild.ListOnlineMembers())
							{
								ply.GuildName = newguildname;
							}
							Character[] objs = (Character[]) GameServer.Database.SelectObjects(typeof (Character), "GuildName = '" + GameServer.Database.Escape(oldguildname) + "'");
							foreach (Character chr in objs)
							{
								chr.GuildName = newguildname;
								GameServer.Database.SaveObject(chr);
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// ADDPLAYER
						// --------------------------------------------------------------------------------
					case "addplayer":
						{
							if (client.Account.PrivLevel == (uint) ePrivLevel.Player)
								return 1;

							if (args.Length < 5)
							{
								client.Out.SendMessage("Syntax error! /gc addplayer playername to guildname", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}

							int i;
							for (i = 2; i < args.Length; i++)
							{
								if (args[i] == "to")
									break;
							}

							string playername = String.Join(" ", args, 2, i - 2);
							string guildname = String.Join(" ", args, i + 1, args.Length - i - 1);

							GuildMgr.GetGuildByName(guildname).AddPlayer(WorldMgr.GetClientByPlayerName(playername, true, false ).Player);
						}
						break;
						// --------------------------------------------------------------------------------
						// REMOVEPLAYER
						// --------------------------------------------------------------------------------
					case "removeplayer":
						{
							if (client.Account.PrivLevel == (uint) ePrivLevel.Player)
								return 1;

							if (args.Length < 5)
							{
								client.Out.SendMessage("Syntax error! /gc removeplayer playername from guildname", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}

							int i;
							for (i = 2; i < args.Length; i++)
							{
								if (args[i] == "from")
									break;
							}

							string playername = String.Join(" ", args, 2, i - 2);
							string guildname = String.Join(" ", args, i + 1, args.Length - i - 1);

							if (!GuildMgr.DoesGuildExist(guildname))
							{
								client.Out.SendMessage("Guild does not exist!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							GuildMgr.GetGuildByName(guildname).RemovePlayer("gamemaster", WorldMgr.GetClientByPlayerName(playername, true, false).Player);
						}
						break;

/****************************************guild member command***********************************************/
						// --------------------------------------------------------------------------------
						// INVITE
						// --------------------------------------------------------------------------------
					case "invite":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Invite))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (args.Length > 2)
							{
								GameClient temp = WorldMgr.GetClientByPlayerName(args[2], true, true);
								if (temp != null)
									obj = temp.Player;
							}
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj == client.Player)
							{
								client.Out.SendMessage("You can't invite your self.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							if (obj.Guild != null)
							{
								client.Out.SendMessage("Target is already member of a guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!obj.Alive)
							{
								client.Out.SendMessage("You cannot invite a dead member to your guild.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!GameServer.ServerRules.IsAllowedToGroup(client.Player, obj, true))
							{
								client.Out.SendMessage("You cannot invite this member", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							obj.Out.SendGuildInviteCommand(client.Player, client.Player.Name + " has invited you to join\n" + client.Player.GetPronoun(1, false) + " guild, [" + client.Player.Guild.Name + "].\n" + "Do you wish to join?");
							client.Out.SendMessage("You have invited " + obj.Name + " to join your guild.", eChatType.CT_System, eChatLoc.CL_SystemWindow);

						}
						break;
						// --------------------------------------------------------------------------------
						// ACCEPT
						// --------------------------------------------------------------------------------
						/*case "accept":
				{//TODO
					//it must be other player guild not player who accept
					GameObject TargetObject = WorldMgr.GetObjectByIDFromRegion(client.Player.CurrentRegionID,data1);
					client.Player.Guild.AddPlayer(client.Player.Name, (GamePlayer)obj);
				}	break;*/
						//need to know what is send to close window ans say yes
						// --------------------------------------------------------------------------------
						// DECLINE
						// --------------------------------------------------------------------------------
						/*case "decline":
				{//TODO
					//it must be other player guild not player who accept
					GameObject TargetObject = WorldMgr.GetObjectByIDFromRegion(client.Player.CurrentRegionID,data1);
					client.Player.Guild.AddPlayer(client.Player.Name, (GamePlayer)obj);
				}	break;*/
						//need to know what is send to close window ans say no

						// --------------------------------------------------------------------------------
						// REMOVE
						// --------------------------------------------------------------------------------
					case "remove":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Remove))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							if (args.Length < 3)
							{
								client.Out.SendMessage("Usage: /guild remove <PlayerName>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							string playername = String.Join(" ", args, 2, args.Length - 2);
							GameClient myclient = WorldMgr.GetClientByPlayerName(playername, true, false);
							if (myclient == null)
							{
								client.Out.SendMessage("This player name does not exist", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (myclient == null)
							{
								client.Out.SendMessage("This player name does not exist", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (myclient.Player.GuildName != client.Player.GuildName)
							{
								client.Out.SendMessage("Player is not a member of your guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							string message1 = "[Guild]: " + client.Player.Name + " has removed " + myclient.Player.Name + " from the guild";
							foreach (GamePlayer ply in client.Player.Guild.ListOnlineMembers())
							{
								ply.Out.SendMessage(message1, eChatType.CT_Guild, eChatLoc.CL_ChatWindow);
							}
							client.Player.Guild.RemovePlayer(client.Player.Name, myclient.Player);
						}
						break;

						// --------------------------------------------------------------------------------
						// INFO
						// --------------------------------------------------------------------------------
					case "info":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							client.Out.SendMessage("info of guild :", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							string mesg = client.Player.Guild.Name + "  " + client.Player.Guild.MemberOnlineCount + " members ";
							client.Out.SendMessage(mesg, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							client.Out.SendMessage("MOTD :" + client.Player.Guild.theGuildDB.Motd, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							foreach (DBRank rank in client.Player.Guild.theGuildDB.Ranks)
							{
								client.Out.SendMessage("RANK :" + rank.RankLevel.ToString() + "Name :" + rank.Title, eChatType.CT_System, eChatLoc.CL_SystemWindow);
								client.Out.SendMessage("AcHear :" + (rank.AcHear ? "y" : "n") + " AcSpeak :" + (rank.AcSpeak ? "y" : "n"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								client.Out.SendMessage("OcHear :" + (rank.OcHear ? "y" : "n") + " OcSpeak :" + (rank.OcSpeak ? "y" : "n"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								client.Out.SendMessage("GcHear :" + (rank.GcHear ? "y" : "n") + " GcSpeak :" + (rank.GcSpeak ? "y" : "n"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								client.Out.SendMessage("Emblem :" + (rank.Emblem ? "y" : "n") + " Promote :" + (rank.Promote ? "y" : "n"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
								client.Out.SendMessage("Remove :" + (rank.Remove ? "y" : "n") + " View :" + (rank.View ? "y" : "n"), eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// LIST
						// --------------------------------------------------------------------------------
					case "list":
						{
							ICollection guildList = GuildMgr.ListGuild();
							lock (guildList.SyncRoot)
							{
								foreach (Guild gui in guildList)
								{
									string mesg = gui.Name + "  " + gui.MemberOnlineCount + " members ";
									client.Out.SendMessage(mesg, eChatType.CT_System, eChatLoc.CL_SystemWindow);
								}
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// EDIT
						// --------------------------------------------------------------------------------
					case "edit":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							SetCmd(client, args);
						}
						break;
						// --------------------------------------------------------------------------------
						// FORM
						// --------------------------------------------------------------------------------
					case "form":
					//Fooljam fix begin. Players can not create guild names with other characters than Alpha anymore.
					{
					foreach(char c in args[2])
					{
						if( (int)c<65 || ((int)c>90 && (int)c<97) || (int)c>122)
						{
							client.Out.SendMessage("You can not create a guild with a such name. Only A-Z,a-z characters are allowed!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 0;
						}
					}
					//Fooljam fix end. Players can not create guild names with other characters than Alpha anymore.
							if (args.Length < 3)
							{
								client.Out.SendMessage("Syntax error! /gc form <guildname>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}
							string guildname = String.Join(" ", args, 2, args.Length - 2);
							guildname = GameServer.Database.Escape(guildname);
							if (!GuildMgr.DoesGuildExist(guildname))
							{
								PlayerGroup group = client.Player.PlayerGroup;

								if (group == null)
								{
									client.Out.SendMessage("You must have a group to create a guild.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
									return 0;
								}

								lock (group)
								{
									if (group.PlayerCount < PlayerGroup.MAX_GROUP_SIZE)
									{
										client.Out.SendMessage(PlayerGroup.MAX_GROUP_SIZE + " members must be in group to create a guild.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
										return 0;
									}

									// a member of group have a guild already, so quit!
									foreach (GamePlayer ply in group)
									{
										if (ply.Guild != null)
										{
											client.Player.PlayerGroup.SendMessageToGroupMembers(ply.Name + " is already member of a guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
											return 0;
										}
									}

									Guild newGuild = GuildMgr.CreateGuild(client.Player, guildname);
									if (newGuild == null)
									{
										message = "Unable to create guild \"" + guildname + "\" with player " + client.Player.Name + " as leader!";
										client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
									}
									else
									{
										foreach (GamePlayer ply in group)
										{
											newGuild.AddPlayer(ply);
										}
										client.Player.GuildRank = client.Player.Guild.GetRankByID(0); //creator is leader
										message = "Create guild \"" + guildname + "\" with player " + client.Player.Name + " as leader!";
										client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
									}
								}
							}
							else
							{
								message = "Guild " + guildname + " cannot be created because it already exsists!";
								client.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// QUIT
						// --------------------------------------------------------------------------------
					case "quit":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							client.Out.SendGuildLeaveCommand(client.Player, "Do you really want to leave\n" + client.Player.Guild.Name + "?");
						}
						break;
						// --------------------------------------------------------------------------------
						// PROMOTE
						// --------------------------------------------------------------------------------
					case "promote":
						{
							if (args.Length < 3)
							{
								client.Out.SendMessage("Syntax error! /gc promote <newrank> [name]", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}

							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Promote))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (args.Length > 3)
							{
								GameClient temp = WorldMgr.GetClientByPlayerName(args[3], true, false);
								if (temp != null && GameServer.ServerRules.IsAllowedToGroup(client.Player, temp.Player, true))
									obj = temp.Player;
							}
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj.Guild != client.Player.Guild)
							{
								client.Out.SendMessage(obj.Name + " is not in your guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							int newrank = obj.GuildRank.RankLevel;
							try
							{
								newrank = Convert.ToInt32( args[2] );
							}
							catch
							{
								client.Out.SendMessage("You must promote to a number : /gc promote <ranklevel>", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							}
							if ((newrank > obj.GuildRank.RankLevel) || (newrank < 1))
							{
								client.Out.SendMessage("You can promote to a inferior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							obj.GuildRank = obj.Guild.GetRankByID(newrank);
							obj.SaveIntoDatabase();
							obj.Out.SendMessage("You are promoted to " + newrank.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// DEMOTE
						// --------------------------------------------------------------------------------
					case "demote":
						{
							if (args.Length < 3)
							{
								client.Out.SendMessage("Syntax error! /gc demote <newrank> [name]", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 0;
							}

							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Demote))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (args.Length > 3)
							{
								GameClient temp = WorldMgr.GetClientByPlayerName(args[3], true, false);
								if (temp != null && GameServer.ServerRules.IsAllowedToGroup(client.Player, temp.Player, true))
									obj = temp.Player;
							}
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj.Guild != client.Player.Guild)
							{
								client.Out.SendMessage(obj.Name + " is not in your guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							ushort newrank = Convert.ToUInt16(args[2]);
							if (newrank < obj.GuildRank.RankLevel || newrank > 10)
							{
								client.Out.SendMessage("You can demote to a superior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							obj.GuildRank = obj.Guild.GetRankByID(newrank);
							obj.SaveIntoDatabase();
							obj.Out.SendMessage("You are demoted to " + newrank.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// WHO
						// --------------------------------------------------------------------------------
					case "who":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							int ind = 0;
							if (args.Length == 3 )
							{
								if (args[2] == "alliance" || args[2] == "a" )
								{
									foreach( Guild guild in client.Player.Guild.alliance.Guilds )
									{
										foreach (GamePlayer ply in guild.ListOnlineMembers())
										{
											if (ply.Client.IsPlaying && !ply.IsAnonymous)
											{
												ind++;
												string zoneName = (ply.CurrentZone == null ? "(null)" : ply.CurrentZone.Description);
												string mesg = ind + ") " + ply.Name + " <guild=" + guild.Name + "> the Level " + ply.Level + " " + ply.CharacterClass.Name + " in " + zoneName;
												client.Out.SendMessage(mesg, eChatType.CT_System, eChatLoc.CL_SystemWindow);
											}
										}
									}
								}
								else
									client.Out.SendMessage("alliance list usage: /gc who alliance or /gc who a ", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							
							foreach (GamePlayer ply in client.Player.Guild.ListOnlineMembers())
							{
								if (ply.Client.IsPlaying && !ply.IsAnonymous)
								{
									ind++;
									string zoneName = (ply.CurrentZone == null ? "(null)" : ply.CurrentZone.Description);
									string mesg = ind + ") " + ply.Name + " <rank=" + ply.GuildRank.RankLevel.ToString() + "> the Level " + ply.Level + " " + ply.CharacterClass.Name + " in " + zoneName;
									client.Out.SendMessage(mesg, eChatType.CT_System, eChatLoc.CL_SystemWindow);
								}
							}
							client.Out.SendMessage("total member online:        " + ind.ToString(),eChatType.CT_System,eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// LEADER
						// --------------------------------------------------------------------------------
					case "leader":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (args.Length > 2)
							{
								GameClient temp = WorldMgr.GetClientByPlayerName(args[2], true, false);
								if (temp != null && GameServer.ServerRules.IsAllowedToGroup(client.Player, temp.Player, true))
									obj = temp.Player;
							}
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj.Guild != client.Player.Guild)
							{
								client.Out.SendMessage(obj.Name + " is not in your guild!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}

							obj.GuildRank = obj.Guild.GetRankByID(0);
							obj.SaveIntoDatabase();
							obj.Out.SendMessage("You are now the leader of " + obj.GuildName, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							string message1 = "[Guild]: " + client.Player.Name + " has set guild member " + obj.Name + " to leader of the guild!";
							foreach (GamePlayer ply in client.Player.Guild.ListOnlineMembers())
							{
								ply.Out.SendMessage(message1, eChatType.CT_Guild, eChatLoc.CL_ChatWindow);
							}
						}
						break;
						// --------------------------------------------------------------------------------
						// EMBLEM
						// --------------------------------------------------------------------------------
					case "emblem":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
							{
								client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (client.Player.Guild.theGuildDB.Emblem != 0)
							{
								if (client.Player.TargetObject is EmblemNPC == false)
								{
									client.Out.SendMessage("Your guild already has an emblem but you may change it for a hefty fee of 200 gold. You must select the EmblemNPC again for this proceedure to happen.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
									return 1;
								}
								client.Out.SendCustomDialog("Would you like to re-emblem your guild for 200 gold?", new CustomDialogResponse(EmblemChange));
								return 1;
							}
							if (client.Player.TargetObject is EmblemNPC == false)
							{
								client.Out.SendMessage("You must select the EmblemNPC", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							client.Out.SendEmblemDialogue();
						}
						break;
						// --------------------------------------------------------------------------------
						// MOTD
						// --------------------------------------------------------------------------------
					case "motd":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
							{
								client.Out.SendMessage("You must be the guild master to set the motd", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							message = String.Join(" ", args, 2, args.Length - 2);
							client.Player.Guild.theGuildDB.Motd = message;
							GameServer.Database.SaveObject(client.Player.Guild.theGuildDB);
							client.Out.SendMessage("You have set the motd", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// AMOTD
						// --------------------------------------------------------------------------------
					case "amotd":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
							{
								client.Out.SendMessage("You must be the guild master to set the motd", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							message = String.Join(" ", args, 2, args.Length - 2);
							client.Player.Guild.alliance.Dballiance.Motd = message;
							GameServer.Database.SaveObject(client.Player.Guild.alliance.Dballiance);
							client.Out.SendMessage("You have set the motd of alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
						// --------------------------------------------------------------------------------
						// OMOTD
						// --------------------------------------------------------------------------------
					case "omotd":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
							{
								client.Out.SendMessage("You must be the guild master to set the motd", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							message = String.Join(" ", args, 2, args.Length - 2);
							client.Player.Guild.theGuildDB.oMotd = message;
							GameServer.Database.SaveObject(client.Player.Guild.theGuildDB);
						}
						break;
						// --------------------------------------------------------------------------------
						// AINVITE
						// --------------------------------------------------------------------------------
					case "ainvite":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Alli))
							{
								client.Out.SendMessage("You must be the guild master to invite a guild to an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj.GuildRank.RankLevel != 0)
							{
								client.Out.SendMessage("You must select a GM of guild you want to invite", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (obj.Guild.alliance != null)
							{
								client.Out.SendMessage("The guild has already an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							obj.TempProperties.setProperty("allianceinvite", client.Player); //finish that
							client.Out.SendMessage("you invite a guild to join your alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							obj.Out.SendMessage("you have been invited to join the alliance of " + client.Player.Guild.Name, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						// --------------------------------------------------------------------------------
						// AINVITE
						// --------------------------------------------------------------------------------
					case "aaccept":
						{
							AllianceInvite(client.Player, 0x01);
							return 1;
						}
						// --------------------------------------------------------------------------------
						// AINVITE
						// --------------------------------------------------------------------------------
					case "acancel":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Alli))
							{
								client.Out.SendMessage("You do not have enough priviledge to cancel an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer obj = client.Player.TargetObject as GamePlayer;
							if (obj == null)
							{
								client.Out.SendMessage("You must select a player!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer inviter = client.Player.TempProperties.getObjectProperty("allianceinvite", null) as GamePlayer;
							if (inviter == client.Player)
								obj.TempProperties.removeProperty("allianceinvite");
							client.Out.SendMessage("The alliance answer has been cancelled", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							obj.Out.SendMessage("The alliance answer has been cancelled", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						// --------------------------------------------------------------------------------
						// ADECLINE
						// --------------------------------------------------------------------------------
					case "adecline":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Alli))
							{
								client.Out.SendMessage("You have not enough priviledge to do that", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							GamePlayer inviter = client.Player.TempProperties.getObjectProperty("allianceinvite", null) as GamePlayer;
							client.Player.TempProperties.removeProperty("allianceinvite");
							client.Out.SendMessage("You have decline the alliance's proposal", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							inviter.Out.SendMessage("The alliance's proposal have been declined", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						// --------------------------------------------------------------------------------
						// AREMOVE
						// --------------------------------------------------------------------------------
					case "aremove":
						{
							if (client.Player.Guild == null)
							{
								client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Alli))
							{
								client.Out.SendMessage("You do not have enough priviledge to do that", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (client.Player.Guild.alliance == null)
							{
								client.Out.SendMessage("You have to be a member of an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (client.Player.Guild.theGuildDB == client.Player.Guild.alliance.Dballiance.DBguildleader)
							{
								client.Out.SendMessage("You have to be the leader guild of the alliance , before you can remove a guild from the alliance!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
								return 1;
							}
							if (args.Length > 3)
							{
								if (args[2] == "alliance")
								{
									try
									{
										int index = Convert.ToInt32(args[3]);
										Guild myguild = (Guild) client.Player.Guild.alliance.Guilds[index];
										if (myguild != null)
											client.Player.Guild.alliance.RemoveGuild(myguild);
									}
									catch
									{
										client.Player.Out.SendMessage("the alliance index is not a valide number", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
									}

								}
								client.Player.Out.SendMessage("Usage of /gc aremove:", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
								client.Player.Out.SendMessage("'/gc aremove' to removes your entire guild from an alliance", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
								client.Player.Out.SendMessage("'/gc aremove alliance [#]' to Remove the specified guild (listed by number) from the alliance", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
								return 1;
							}
							else
							{
								GamePlayer obj = client.Player.TargetObject as GamePlayer;
								if (obj == null)
								{
									client.Player.Out.SendMessage("You must select a player", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
									return 1;
								}
								if (obj.Guild == null)
								{
									client.Player.Out.SendMessage("You must select a player from the same alliance", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
									return 1;
								}
								if (obj.Guild.alliance != client.Player.Guild.alliance)
								{
									client.Player.Out.SendMessage("You must select a playerfrom the same alliance", PacketHandler.eChatType.CT_System, PacketHandler.eChatLoc.CL_SystemWindow);
									return 1;
								}
								client.Player.Guild.alliance.RemoveGuild(obj.Guild);
							}
							return 1;
						}
						// --------------------------------------------------------------------------------
						//ClAIM
						// --------------------------------------------------------------------------------
					case "claim":
					{
						if (client.Player.Guild == null)
						{
							client.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						AbstractGameKeep keep = KeepMgr.getKeepCloseToSpot(client.Player.CurrentRegionID,client.Player,WorldMgr.VISIBILITY_DISTANCE);
						if (keep == null)
						{
							client.Out.SendMessage("You have to be near the keep to claim it.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (keep.CheckForClaim(client.Player))
						{
							keep.Claim(client.Player);
						}
						client.Player.Guild.SendMessageToGuildMembers("Your guild have claim "+ keep.Name,eChatType.CT_Guild,eChatLoc.CL_SystemWindow);
						return 1;
					}
						// --------------------------------------------------------------------------------
						//RELEASE
						// --------------------------------------------------------------------------------
					case "release":
					{
						if(client.Player.Guild == null)
						{
							client.Out.SendMessage("You must have a guild to release a claimed keep.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (client.Player.Guild.ClaimedKeep == null)
						{
							client.Out.SendMessage("You must have a keep to release.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Release))
						{
							client.Out.SendMessage("You have not enough priviledge to do that.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.ClaimedKeep.Release();
						return 1;
					}
						// --------------------------------------------------------------------------------
						//UPGRADE
						// --------------------------------------------------------------------------------
					case "upgrade":
					{
						if(client.Player.Guild == null)
						{
							client.Out.SendMessage("You must have a guild to release a claimed keep.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (client.Player.Guild.ClaimedKeep == null)
						{
							client.Out.SendMessage("You must have a keep to upgrate it.",eChatType.CT_System,eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Upgrade))
						{
							client.Out.SendMessage("You have not enough priviledge to do that.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						if (args.Length != 3)
						{
							client.Out.SendMessage("You must specify a level to target for upgrade.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						int targetlevel = 0;
						try
						{
							targetlevel = Convert.ToInt32(args[2]);
							if (targetlevel > 10 || targetlevel < 1)
								return 0;
						}
						catch
						{
							client.Out.SendMessage("the 2e argument must be a number", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 0;
						}
						client.Player.Guild.ClaimedKeep.Upgrade(targetlevel);
						return 1;
					}
					default:
						{
							client.Out.SendMessage("Unknown command \"" + args[1] + "\", please type /gc for a command list.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						}
						break;
				} //switch
				return 1;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("error in /gc script, " + args[1] + " command: " + e.ToString());
				if (client.Account.PrivLevel > 1)
				{
					client.Out.SendMessage("Game Master commands:", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc create <Name> <player>' Create a new guild with player as leader", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc purge <Name>' Purge a guild completely", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc rename <NewName> to <OldName' Rename guild from OldName to NewName", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc addplayer <player> to <guild>' to add player to guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					client.Out.SendMessage("'/gc removeplayer <player> from <guild>' to remove player from a guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				}

				client.Out.SendMessage("Member commands:", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc form <Name>' to create a new guilde with all player of group", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc info' to show information on the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc cancel <option> [value]' to cancel all command done before", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc decline' to decline the enter in guild ", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc claim' to claim a keep", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc quit' to leave the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc amotd <text>' to set the message of the day of the alliance's channel", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc omotd <text>' to set the message of the day of the officier's channel", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc motd <text>' to set the message of the day of the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc promote <rank#>' to promote player to a superior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc demote <rank#>' to set the rank of the player to an inferior rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc remove <playername>' to remove the player from the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc emblem' to set emblem", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit' to show list of all option in guild edit", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc leader' to set leader successor", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc accept ' to accept invite to guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc invite ' to invite targeted player to join the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc who' to show all player in your guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc list' to show all guild in your realm", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}
		}

		protected void AllianceInvite(GamePlayer player, byte response)
		{
			if (response != 0x01)
				return; //declined

			GamePlayer inviter = player.TempProperties.getObjectProperty("allianceinvite", null) as GamePlayer;

			if (player.Guild == null)
			{
				player.Out.SendMessage("You have to be a member of a guild, before you can use any of the commands!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			if (!player.Guild.GotAccess(player, eGuildRank.Alli))
			{
				player.Out.SendMessage("You have not enough privilege to accept an alliance", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			player.TempProperties.removeProperty("allianceinvite");

			if (inviter.Guild.alliance == null)
			{
				//create alliance
				Alliance alli = new Alliance();
				DBAlliance dballi = new DBAlliance();
				dballi.AllianceName = inviter.Guild.Name;
				dballi.DBguildleader = null;
				dballi.Motd = "";
				alli.Dballiance = dballi;
				alli.Guilds.Add(inviter.Guild);
				inviter.Guild.alliance = alli;
				inviter.Guild.theGuildDB.AllianceID = inviter.Guild.alliance.Dballiance.ObjectId;
			}
			inviter.Guild.alliance.AddGuild(player.Guild);
			inviter.Guild.alliance.SaveIntoDatabase();
		}

		public static void EmblemChange(GamePlayer player, byte response)
		{
			if (response != 0x01)
				return;
			if (player.TargetObject is EmblemNPC == false)
			{
				player.Out.SendMessage("You must select the EmblemNPC", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			if (player.GetCurrentMoney() < GuildMgr.COST_RE_EMBLEM) //200 gold to re-emblem
			{
				player.Out.SendMessage("You must have 200 gold to re emblem your guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return;
			}
			player.Out.SendEmblemDialogue();
		}

		public int SetCmd(GameClient client, string[] args)
		{
			if (args.Length < 4)
			{
				client.Out.SendMessage("not enough argument", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("usage : ", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> title <text>' sets the title for a specific rank <#> for this guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> ranklevel <level>'  used to set the rank hierarchy", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> emblem <y/n>' used the edit the rank of members who are allowed to wear the guild emblem on their cloak or shield.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> gchear <y/n>' edits this rank to be able to hear guild chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> gcspeak <y/n>' edits this rank to be able to speak in guild chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> ochear <y/n>' edits this rank to be able to hear Officer chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> ocspeak <y/n>' edits this rank to be able to speak in Officer chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> achear <y/n>' edits this rank to be able to hear Alliance chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> acspeak <y/n>' edits this rank to be able to speak in Alliance chat", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> invite <y/n>' edits this rank with the ability to invite members into the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> promote <y/n>' edits this rank with the ability to promote/demote lower in rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> remove <y/n>' edits this rank with the ability to remove those below him in rank", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> view <y/n>'  edits this rank with the ability to display the /gc info screen", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> alli <y/n>'  to allow a rank to enter/leave alliances (create a diplomatic officer)", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> claim <y/n>'  to allow a rank to enter/leave alliances (create a diplomatic officer)", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> upgrade <y/n>'  to allow a rank to enter/leave alliances (create a diplomatic officer)", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				client.Out.SendMessage("'/gc edit <ranknum> release <y/n>'  to set release the keep you have claim before", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}

			bool reponse = true;
			if (args.Length > 4)
			{
				if (args[4].StartsWith("y"))
					reponse = true;
				else if (args[4].StartsWith("n"))
					reponse = false;
				else if (args[3] != "title" && args[3] != "ranklevel")
				{
					client.Out.SendMessage("you must format your command like '/gc edit <#> cmd <y or n>'", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					return 1;
				}
			}
			byte number;
			try
			{
				number = Convert.ToByte(args[2]);
				if (number > 9 || number < 0)
					return 0;
			}
			catch
			{
				client.Out.SendMessage("the 3e argument must be a number", eChatType.CT_System, eChatLoc.CL_SystemWindow);
				return 0;
			}

			switch (args[3])
			{
				case "title":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						string message = String.Join(" ", args, 4, args.Length - 4);
						client.Player.Guild.GetRankByID(number).Title = message;
						client.Out.SendMessage("you have set the title of the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "ranklevel":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						byte lvl = Convert.ToByte(args[4]);
						client.Player.Guild.GetRankByID(number).RankLevel = lvl;
						client.Out.SendMessage("you have change the level of the rank " + number.ToString() + " to " + lvl.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;

				case "emblem":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Emblem = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the emblem of the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "gchear":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).GcHear = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the hear of guild channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "gcspeak":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}

						client.Player.Guild.GetRankByID(number).GcSpeak = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the speak on guild channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "ochear":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).OcHear = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the hear of officier channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "ocspeak":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).OcSpeak = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the speak on the officier channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "achear":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).AcHear = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the hear of alliance channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "acspeak":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).AcSpeak = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the speak on alliance channel for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "invite":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Invite = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the rank " + number.ToString() + "to invite player to the guild", eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "promote":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Promote = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the promote for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "remove":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Remove = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the remove for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "alli":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Leader))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Alli = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " enter/leave alliance for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "view":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.View))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).View = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the view of info for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "claim":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Claim))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Claim = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " the claim of keep for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "upgrade":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Upgrade))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Upgrade = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " to upgrade a keep for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				case "release":
					{
						if (!client.Player.Guild.GotAccess(client.Player, eGuildRank.Release))
						{
							client.Out.SendMessage("You dont have the priviledges for that!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
							return 1;
						}
						client.Player.Guild.GetRankByID(number).Release = reponse;
						client.Out.SendMessage("you have " + (reponse ? "allow" : "unallow") + " to release a claimed keep for the rank " + number.ToString(), eChatType.CT_System, eChatLoc.CL_SystemWindow);
					}
					break;
				default:
					{
						return 0;
					}
			} //switch
			GameServer.Database.SaveObject(client.Player.Guild.GetRankByID(number));
			return 1;
		}
	}
}