using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.GS;
using DOL.Events;


namespace DOL.SP
{
    public class FakePacketLib : IPacketLib
    {
		private GamePlayer player;
		private GamePlayer main;
		
		public FakePacketLib(GamePlayer player, GamePlayer main)
		{
			this.player = player;
			this.main = main;
		}
		
        #region IPacketLib Members
		
		public void SendGroupInviteCommand(GS.GamePlayer invitingPlayer, string inviteMessage)
        {
			if (invitingPlayer == main)
			{
				GamePlayer groupLeader = invitingPlayer;
				
				if (groupLeader.Group != null)
				{
					if (groupLeader.Group.Leader != groupLeader) return;
	                if (groupLeader.Group.MemberCount >= 6)
					{
						player.Out.SendMessage("The group is full.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
						return;
					}
					groupLeader.Group.AddMember(player);
					GameEventMgr.Notify(GamePlayerEvent.AcceptGroup, player);
					return;
				}

				var group = new Group(groupLeader);
				GroupMgr.AddGroup(group, group);
	
				group.AddMember(groupLeader);
				group.AddMember(player);
	
				GameEventMgr.Notify(GamePlayerEvent.AcceptGroup, player);
			}
        }
		
        public int BowPrepare
        {
            get { return 0; }
        }

        public int BowShoot
        {
            get { return 0; }
        }

        public int OneDualWeaponHit
        {
            get { return 0; }
        }

        public int BothDualWeaponHit
        {
            get { return 0; }
        }

        public byte GetPacketCode(eServerPackets packetCode)
        {
            return 0; 
        }

        public void SendTCP(GSTCPPacketOut packet)
        {
           
        }

        public void SendTCP(byte[] buf)
        {
           
        }

        public void SendTCPRaw(GSTCPPacketOut packet)
        {
           
        }

        public void SendUDP(GSUDPPacketOut packet)
        {
           
        }

        public void SendUDP(byte[] buf)
        {
           
        }

        public void SendUDPRaw(GSUDPPacketOut packet)
        {
           
        }

        public void SendWarlockChamberEffect(GS.GamePlayer player)
        {
           
        }

        public void SendVersionAndCryptKey()
        {
           
        }

        public void SendLoginDenied(eLoginError et)
        {
           
        }

        public void SendLoginGranted()
        {
           
        }

        public void SendLoginGranted(byte color)
        {
           
        }

        public void SendSessionID()
        {
           
        }

        public void SendPingReply(ulong timestamp, ushort sequence)
        {
           
        }

        public void SendRealm(GS.eRealm realm)
        {
           
        }

        public void SendCharacterOverview(GS.eRealm realm)
        {
           
        }

        public void SendDupNameCheckReply(string name, bool nameExists)
        {
           
        }

        public void SendBadNameCheckReply(string name, bool bad)
        {
           
        }

        public void SendAttackMode(bool attackState)
        {
           
        }

        public void SendCharCreateReply(string name)
        {
           
        }

        public void SendCharStatsUpdate()
        {
           
        }

        public void SendCharResistsUpdate()
        {
           
        }

        public void SendRegions()
        {
           
        }

        public void SendGameOpenReply()
        {
           
        }

        public void SendPlayerPositionAndObjectID()
        {
           
        }

        public void SendPlayerJump(bool headingOnly)
        {
           
        }

        public void SendPlayerInitFinished(byte mobs)
        {
           
        }

        public void SendUDPInitReply()
        {
           
        }

        public void SendTime()
        {
           
        }

        public void SendMessage(string msg, eChatType type, eChatLoc loc)
        {
           
        }

        public void SendPlayerCreate(GS.GamePlayer playerToCreate)
        {
           
        }

        public void SendObjectGuildID(GS.GameObject obj, GS.Guild guild)
        {
           
        }

        public void SendPlayerQuit(bool totalOut)
        {
           
        }

        public void SendObjectRemove(GS.GameObject obj)
        {
           
        }

        public void SendObjectCreate(GS.GameObject obj)
        {
           
        }

        public void SendDebugMode(bool on)
        {
           
        }

        public void SendModelChange(GS.GameObject obj, ushort newModel)
        {
           
        }

        public void SendModelAndSizeChange(GS.GameObject obj, ushort newModel, byte newSize)
        {
           
        }

        public void SendModelAndSizeChange(ushort objectId, ushort newModel, byte newSize)
        {
           
        }

        public void SendEmoteAnimation(GS.GameObject obj, eEmote emote)
        {
           
        }

        public void SendNPCCreate(GS.GameNPC npc)
        {
           
        }

        public void SendLivingEquipmentUpdate(GS.GameLiving living)
        {
           
        }

        public void SendRegionChanged()
        {
           
        }

        public void SendUpdatePoints()
        {
           
        }

        public void SendUpdateMoney()
        {
           
        }

        public void SendUpdateMaxSpeed()
        {
           
        }

        public void SendCombatAnimation(GS.GameObject attacker, GS.GameObject defender, ushort weaponID, ushort shieldID, int style, byte stance, byte result, byte targetHealthPercent)
        {
           
        }

        public void SendStatusUpdate()
        {
           
        }

        public void SendStatusUpdate(byte sittingFlag)
        {
           
        }

        public void SendSpellCastAnimation(GS.GameLiving spellCaster, ushort spellID, ushort castingTime)
        {
           
        }

        public void SendSpellEffectAnimation(GS.GameObject spellCaster, GS.GameObject spellTarget, ushort spellid, ushort boltTime, bool noSound, byte success)
        {
           
        }

        public void SendRiding(GS.GameObject rider, GS.GameObject steed, bool dismount)
        {
           
        }

        public void SendFindGroupWindowUpdate(GS.GamePlayer[] list)
        {
           
        }        

        public void SendDialogBox(eDialogCode code, ushort data1, ushort data2, ushort data3, ushort data4, eDialogType type, bool autoWrapText, string message)
        {
           
        }

        public void SendCustomDialog(string msg, CustomDialogResponse callback)
        {
           
        }

        public void SendCheckLOS(GS.GameObject Checker, GS.GameObject Target, CheckLOSResponse callback)
        {
           
        }

        public void SendGuildLeaveCommand(GS.GamePlayer invitingPlayer, string inviteMessage)
        {
           
        }

        public void SendGuildInviteCommand(GS.GamePlayer invitingPlayer, string inviteMessage)
        {
           
        }

        public void SendQuestOfferWindow(GS.GameNPC questNPC, GS.GamePlayer player, GS.Quests.RewardQuest quest)
        {
           
        }

        public void SendQuestRewardWindow(GS.GameNPC questNPC, GS.GamePlayer player, GS.Quests.RewardQuest quest)
        {
           
        }

        public void SendQuestOfferWindow(GS.GameNPC questNPC, GS.GamePlayer player, GS.Quests.DataQuest quest)
        {
           
        }

        public void SendQuestRewardWindow(GS.GameNPC questNPC, GS.GamePlayer player, GS.Quests.DataQuest quest)
        {
           
        }

        public void SendQuestSubscribeCommand(GS.GameNPC invitingNPC, ushort questid, string inviteMessage)
        {
           
        }

        public void SendQuestAbortCommand(GS.GameNPC abortingNPC, ushort questid, string abortMessage)
        {
           
        }

        public void SendGroupWindowUpdate()
        {
           
        }

        public void SendGroupMemberUpdate(bool updateIcons, GS.GameLiving living)
        {
           
        }

        public void SendGroupMembersUpdate(bool updateIcons)
        {
           
        }

        public void SendInventoryItemsUpdate(ICollection<DOL.Database.InventoryItem> itemsToUpdate)
        {
           
        }

        public void SendInventorySlotsUpdate(ICollection<int> slots)
        {
           
        }

        public void SendInventoryItemsUpdate(byte preAction, ICollection<DOL.Database.InventoryItem> itemsToUpdate)
        {
           
        }

        public void SendInventoryItemsUpdate(IDictionary<int, DOL.Database.InventoryItem> updateItems, byte windowType)
        {
           
        }

        public void SendDoorState(GS.Region region, GS.IDoor door)
        {
           
        }

        public void SendMerchantWindow(GS.MerchantTradeItems itemlist, eMerchantWindowType windowType)
        {
           
        }

        public void SendTradeWindow()
        {
           
        }

        public void SendCloseTradeWindow()
        {
           
        }

        public void SendPlayerDied(GS.GamePlayer killedPlayer, GS.GameObject killer)
        {
           
        }

        public void SendPlayerRevive(GS.GamePlayer revivedPlayer)
        {
           
        }

        public void SendUpdatePlayer()
        {
           
        }

        public void SendUpdatePlayerSkills()
        {
           
        }

        public void SendUpdateWeaponAndArmorStats()
        {
           
        }

        public void SendCustomTextWindow(string caption, IList<string> text)
        {
           
        }

        public void SendPlayerTitles()
        {
           
        }

        public void SendPlayerTitleUpdate(GS.GamePlayer player)
        {
           
        }

        public void SendEncumberance()
        {
           
        }

        public void SendAddFriends(string[] friendNames)
        {
           
        }

        public void SendRemoveFriends(string[] friendNames)
        {
           
        }

        public void SendTimerWindow(string title, int seconds)
        {
           
        }

        public void SendCloseTimerWindow()
        {
           
        }

        public void SendChampionTrainerWindow(int type)
        {
           
        }

        public void SendTrainerWindow()
        {
           
        }

        public void SendInterruptAnimation(GS.GameLiving living)
        {
           
        }

        public void SendDisableSkill(GS.Skill skill, int duration)
        {
           
        }

        public void SendUpdateIcons(System.Collections.IList changedEffects, ref int lastUpdateEffectsCount)
        {
           
        }

        public void SendLevelUpSound()
        {
           
        }

        public void SendRegionEnterSound(byte soundId)
        {
           
        }

        public void SendDebugMessage(string format, params object[] parameters)
        {
           
        }

        public void SendDebugPopupMessage(string format, params object[] parameters)
        {
           
        }

        public void SendEmblemDialogue()
        {
           
        }

        public void SendWeather(uint x, uint width, ushort speed, ushort fogdiffusion, ushort intensity)
        {
           
        }

        public void SendPlayerModelTypeChange(GS.GamePlayer player, byte modelType)
        {
           
        }

        public void SendObjectDelete(GS.GameObject obj)
        {
           
        }

        public void SendObjectUpdate(GS.GameObject obj)
        {
           
        }

        public void SendQuestListUpdate()
        {
           
        }

        public void SendQuestUpdate(GS.Quests.AbstractQuest quest)
        {
           
        }

        public void SendConcentrationList()
        {
           
        }

        public void SendUpdateCraftingSkills()
        {
           
        }

        public void SendChangeTarget(GS.GameObject newTarget)
        {
           
        }

        public void SendChangeGroundTarget(GS.Point3D newTarget)
        {
           
        }

        public void SendPetWindow(GS.GameLiving pet, ePetWindowAction windowAction, AI.Brain.eAggressionState aggroState, AI.Brain.eWalkState walkState)
        {
           
        }

        public void SendPlaySound(eSoundType soundType, ushort soundID)
        {
           
        }

        public void SendNPCsQuestEffect(GS.GameNPC npc, GS.eQuestIndicator indicator)
        {
           
        }

        public void SendMasterLevelWindow(byte ml)
        {
           
        }

        public void SendHexEffect(GS.GamePlayer player, byte effect1, byte effect2, byte effect3, byte effect4, byte effect5)
        {
           
        }

        public void SendRvRGuildBanner(GS.GamePlayer player, bool show)
        {
           
        }

        public void SendSiegeWeaponAnimation(GS.GameSiegeWeapon siegeWeapon)
        {
           
        }

        public void SendSiegeWeaponFireAnimation(GS.GameSiegeWeapon siegeWeapon, int timer)
        {
           
        }

        public void SendSiegeWeaponCloseInterface()
        {
           
        }

        public void SendSiegeWeaponInterface(GS.GameSiegeWeapon siegeWeapon, int time)
        {
           
        }

        public void SendLivingDataUpdate(GS.GameLiving living, bool updateStrings)
        {
           
        }

        public void SendSoundEffect(ushort soundId, ushort zoneId, ushort x, ushort y, ushort z, ushort radius)
        {
           
        }

        public void SendKeepInfo(GS.Keeps.AbstractGameKeep keep)
        {
           
        }

        public void SendKeepRealmUpdate(GS.Keeps.AbstractGameKeep keep)
        {
           
        }

        public void SendKeepRemove(GS.Keeps.AbstractGameKeep keep)
        {
           
        }

        public void SendKeepComponentInfo(GS.Keeps.GameKeepComponent keepComponent)
        {
           
        }

        public void SendKeepComponentDetailUpdate(GS.Keeps.GameKeepComponent keepComponent)
        {
           
        }

        public void SendKeepClaim(GS.Keeps.AbstractGameKeep keep, byte flag)
        {
           
        }

        public void SendKeepComponentUpdate(GS.Keeps.AbstractGameKeep keep, bool LevelUp)
        {
           
        }

        public void SendKeepComponentInteract(GS.Keeps.GameKeepComponent component)
        {
           
        }

        public void SendKeepComponentHookPoint(GS.Keeps.GameKeepComponent component, int selectedHookPointIndex)
        {
           
        }

        public void SendClearKeepComponentHookPoint(GS.Keeps.GameKeepComponent component, int selectedHookPointIndex)
        {
           
        }

        public void SendHookPointStore(GS.Keeps.GameKeepHookPoint hookPoint)
        {
           
        }

        public void SendWarmapUpdate(ICollection<GS.Keeps.AbstractGameKeep> list)
        {
           
        }

        public void SendWarmapDetailUpdate(List<List<byte>> fights, List<List<byte>> groups)
        {
           
        }

        public void SendWarmapBonuses()
        {
           
        }

        public void SendHouse(GS.Housing.House house)
        {
           
        }

        public void SendHouseOccupied(GS.Housing.House house, bool flagHouseOccuped)
        {
           
        }

        public void SendRemoveHouse(GS.Housing.House house)
        {
           
        }

        public void SendGarden(GS.Housing.House house)
        {
           
        }

        public void SendGarden(GS.Housing.House house, int i)
        {
           
        }

        public void SendEnterHouse(GS.Housing.House house)
        {
           
        }

        public void SendExitHouse(GS.Housing.House house, ushort unknown = 0)
        {
           
        }

        public void SendFurniture(GS.Housing.House house)
        {
           
        }

        public void SendFurniture(GS.Housing.House house, int i)
        {
           
        }

        public void SendHousePayRentDialog(string title)
        {
           
        }

        public void SendToggleHousePoints(GS.Housing.House house)
        {
           
        }

        public void SendRentReminder(GS.Housing.House house)
        {
           
        }

        public void SendMarketExplorerWindow(IList<DOL.Database.InventoryItem> items, byte page, byte maxpage)
        {
           
        }

        public void SendMarketExplorerWindow()
        {
           
        }

        public void SendConsignmentMerchantMoney(long copper)
        {
           
        }

        public void SendHouseUsersPermissions(GS.Housing.House house)
        {
           
        }

        public void SendStarterHelp()
        {
           
        }

        public void SendPlayerFreeLevelUpdate()
        {
           
        }

        public void SendMovingObjectCreate(GS.GameMovingObject obj)
        {
           
        }

        public void SendSetControlledHorse(GS.GamePlayer player)
        {
           
        }

        public void SendControlledHorse(GS.GamePlayer player, bool flag)
        {
           
        }

        public void CheckLengthHybridSkillsPacket(ref GSTCPPacketOut pak, ref int maxSkills, ref int first)
        {
           
        }

        public void SendNonHybridSpellLines()
        {
           
        }

        public void SendCrash(string str)
        {
           
        }

        public void SendRegionColorSheme()
        {
           
        }

        public void SendRegionColorSheme(byte color)
        {
           
        }

        public void SendVampireEffect(GS.GameLiving living, bool show)
        {
           
        }

        public void SendXFireInfo(byte flag)
        {
           
        }

        public void SendMinotaurRelicMapRemove(byte id)
        {
           
        }

        public void SendMinotaurRelicMapUpdate(byte id, ushort region, int x, int y, int z)
        {
           
        }

        public void SendMinotaurRelicWindow(GS.GamePlayer player, int spell, bool flag)
        {
           
        }

        public void SendMinotaurRelicBarUpdate(GS.GamePlayer player, int xp)
        {
           
        }

        public void SendBlinkPanel(byte flag)
        {
           
        }

        #endregion
    }
}
