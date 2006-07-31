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
using DOL.GS.Housing;
using DOL.GS.PacketHandler;

namespace DOL.GS.PacketHandler.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, 0x00, "Handles housing menu requests")]
	public class HousingBuyRequestHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int housenumber = packet.ReadShort();
			int menuid = packet.ReadByte();
			int flag = packet.ReadByte();
//			client.Out.SendDebugMessage("CtoS_0x00 (houseNumber:0x{0:X4} menuid:{1} flag:0x{2:X2})", housenumber, menuid, flag);

			House house = (House)HouseMgr.GetHouse(client.Player.CurrentRegionID,housenumber);
			if(house == null) { return 1; }

			if (client.Player == null) return 1;
			if (!house.IsOwner(client.Player)) return 1;

			client.Player.CurrentHouse = house;

			switch(menuid)
			{
				case 0: // Exterior decoration (Garden)
					client.Player.Out.SendMerchantWindow(HouseTemplateMgr.OutdoorShopItems, eMerchantWindowType.HousingOutsideShop);
					break;

				case 1: // Interior decoration
					client.Player.Out.SendMerchantWindow(HouseTemplateMgr.IndoorShopItems, eMerchantWindowType.HousingInsideShop);
					break;

				case 2: // Exterior menu
					client.Player.Out.SendMerchantWindow(HouseTemplateMgr.OutdoorMenuItems, eMerchantWindowType.HousingOutsideMenu);
					break;

				case 7:
					house.SendHouseInfo(client.Player);
					break;

				case 8: // Interior menu (flag = 0x00 - roof, 0xFF - floor or wall)
					client.Player.Out.SendMerchantWindow(HouseTemplateMgr.IndoorMenuItems, eMerchantWindowType.HousingInsideMenu);
					break;

				default:
					client.Out.SendMessage("Invalid menu id "+menuid+" (hookpoint?).",eChatType.CT_System,eChatLoc.CL_SystemWindow);
					break;
			}
			return 1;
		}
	}
}
