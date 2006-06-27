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
#define NOENCRYPTION
using System;
using System.Reflection;
using System.Collections;
using DOL.AI.Brain;
using DOL.GS.Effects;
using DOL.GS.Styles;
using DOL.GS.PlayerTitles;
using DOL.GS.Database;
using log4net;

namespace DOL.GS.PacketHandler
{
	[PacketLib(182, GameClient.eClientVersion.Version182)]
	public class PacketLib182 : PacketLib181
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructs a new PacketLib for Version 1.82 clients
		/// </summary>
		/// <param name="client">the gameclient this lib is associated with</param>
		public PacketLib182(GameClient client)
			: base(client)
		{
		}

		protected override void SendInventorySlotsUpdateBase(ICollection slots, byte preAction)
		{
			GSTCPPacketOut pak = new GSTCPPacketOut(GetPacketCode(ePackets.InventoryUpdate));
			pak.WriteByte((byte)(slots == null ? 0 : slots.Count));
			pak.WriteByte((byte)((m_gameClient.Player.IsCloakHoodUp ? 0x01 : 0x00) | (int)m_gameClient.Player.ActiveQuiverSlot)); //bit0 is hood up bit4 to 7 is active quiver
			pak.WriteByte((byte)m_gameClient.Player.VisibleActiveWeaponSlots);
			pak.WriteByte(preAction); //preAction (0x00 - Do nothing)
			if (slots != null)
			{
				foreach (int updatedSlot in slots)
				{
					pak.WriteByte((byte) updatedSlot);
					
					GenericItem item = m_gameClient.Player.Inventory.GetItem((eInventorySlot) updatedSlot);
					if (item == null)
					{
						pak.Fill(0x00, 19);
						continue;
					}

					int value1 = 0; // some object types use this field to display count
					int value2 = 0; // some object types use this field to display count
					int handNeeded = 0;
					int damageType = 0;
					int condition = 0;
					int durabiliy = 0; 
					int quality = 0; 
					int bonus = 0; 
					int modelExtension = 0;
					int color = 0;
					int effect = 0;
					int count = 0;

					if(item is StackableItem)
					{
						value1 = ((StackableItem)item).Count;
						if(item is Ammunition)
						{
							value2 = ((byte)((Ammunition)item).Damage) | (((byte)((Ammunition)item).Range) << 2) | (((byte)((Ammunition)item).Precision) << 4);//item.SPD_ABS;
						}
						count = ((StackableItem)item).Count;
					}
					else if(item is ThrownWeapon)
					{
						value1 = ((ThrownWeapon)item).DamagePerSecond;
						value2 = 1; // TODO Thrown stack
					}
					else if(item is Instrument)
					{
						value1 = (byte)((Instrument)item).Type;
					}
					else if (item is Shield)
					{
						value1 = (byte)((Shield)item).Size;
						value2 = ((Shield)item).DamagePerSecond;
					}
					else if(item is Weapon)
					{
						value1 = ((Weapon)item).DamagePerSecond;
                        value2 = (byte)(((Weapon)item).Speed / 100);
					}
					else if(item is Armor)
					{
						value1 = ((Armor)item).ArmorFactor;
						value2 = ((Armor)item).Absorbtion;
					}

					if(item is VisibleEquipment)
					{
						color = ((VisibleEquipment)item).Color;
					}

					if(item is Weapon)
					{
						handNeeded = (byte)((Weapon)item).HandNeeded;
						damageType = (byte)((Weapon)item).DamageType;
						effect = ((Weapon)item).GlowEffect;
					}
					else if(item is Armor) 
					{
						modelExtension = ((Armor)item).ModelExtension;
					}
					else if(item is Ammunition)
					{
						damageType = (byte)((Ammunition)item).DamageType;
					}

					if(item is EquipableItem)
					{
						condition = (byte)((EquipableItem)item).Condition;
						durabiliy = ((EquipableItem)item).Durability; 
						quality = ((EquipableItem)item).Quality; 
						bonus = ((EquipableItem)item).Bonus;
					}
					else if(item is SpellCraftGem)
					{
						quality = ((SpellCraftGem)item).Quality; 
					}
					/*
					pak.WriteByte((byte)value1);
					pak.WriteByte((byte)value2);

					pak.WriteByte((byte)(item.Hand << 6));
					pak.WriteByte((byte)((item.Type_Damage > 3 ? 0 : item.Type_Damage << 6) + item.Object_Type));
					pak.WriteShort((ushort)item.Weight);
					pak.WriteByte(item.ConditionPercent); // % of con
					pak.WriteByte(item.DurabilityPercent); // % of dur
					pak.WriteByte((byte)item.Quality); // % of qua
					pak.WriteByte((byte)item.Bonus); // % bonus
					pak.WriteShort((ushort)item.Model);
					pak.WriteByte((byte)item.Extension);
					if (item.Emblem != 0)
						pak.WriteShort((ushort)item.Emblem);
					else
						pak.WriteShort((ushort)item.Color);
					*/

					pak.WriteByte(item.Level);
					pak.WriteByte((byte) value1);
					pak.WriteByte((byte) value2);
					pak.WriteByte((byte) (handNeeded << 6));
					pak.WriteByte((byte) ((damageType > 3 ? 0 : damageType << 6) + (byte)item.ObjectType));//damageType << 6) + (byte)item.ObjectType));
					pak.WriteShort((ushort) item.Weight);
					pak.WriteByte((byte) condition); // % of con
					pak.WriteByte((byte) durabiliy); // % of dur
					pak.WriteByte((byte) quality); // % of qua
					pak.WriteByte((byte) bonus); // % bonus
					pak.WriteShort((ushort) item.Model);
					pak.WriteByte((byte)modelExtension);
					pak.WriteShort((ushort) color);
					byte flag = 0;
					   if (condition < 100 && DOL.GS.Repair.IsAllowedToBeginWork(m_gameClient.Player, (EquipableItem)item, 50))
							flag |= 0x01; // enable repair button ? (always enabled if condition < 100)
					   flag |= 0x02; // enable salvage button
					   AbstractCraftingSkill skill = CraftingMgr.getSkillbyEnum(m_gameClient.Player.CraftingPrimarySkill);
					   if (skill != null && skill is AdvancedCraftingSkill/* && ((AdvancedCraftingSkill)skill).IsAllowedToCombine(m_gameClient.Player, item)*/)
							flag |= 0x04; // enable craft button
					ushort icon1 = 0;
					ushort icon2 = 0;
					string spell_name1 = "";
					string spell_name2 = "";
					if (((EquipableItem)item).ChargeSpellID > 0/* && item.Charges > 0*/)
					{
						SpellLine chargeEffectsLine = SkillBase.GetSpellLine(GlobalSpellsLines.Item_Effects);
						if (chargeEffectsLine != null)
						{
							IList spells = SkillBase.GetSpellList(chargeEffectsLine.KeyName);
							if (spells != null)
							{
								foreach (Spell spl in spells)
								{
									if (spl.ID == ((EquipableItem)item).ChargeSpellID)
									{
										flag |= 0x08;
										icon1 = spl.Icon;
										spell_name1 = spl.SpellType; // or best spl.Name ?
										break;
									}
								}
							}
						}
					}
					if (((EquipableItem)item).ChargeSpellID > 0/* && item.Charges > 0*/)
					{
						SpellLine chargeEffectsLine = SkillBase.GetSpellLine(GlobalSpellsLines.Item_Effects);
						if (chargeEffectsLine != null)
						{
							IList spells = SkillBase.GetSpellList(chargeEffectsLine.KeyName);
							if (spells != null)
							{
								foreach (Spell spl in spells)
								{
									if (spl.ID == ((EquipableItem)item).ChargeSpellID)
									{
										flag |= 0x10;
										icon2 = spl.Icon;
										spell_name2 = spl.SpellType; // or best spl.Name ?
										break;
									}
								}
							}
						}
					}
					pak.WriteByte((byte)flag);
					if ((flag & 0x08) == 0x08)
					{
						pak.WriteShort((ushort)icon1);
						pak.WritePascalString(spell_name1);
					}
					if ((flag & 0x10) == 0x10)
					{
						pak.WriteShort((ushort)icon2);
						pak.WritePascalString(spell_name2);
					}
					pak.WriteByte((byte)effect);
					// pak.WriteShort((ushort) item.Effect);
					if (count > 1)
						pak.WritePascalString(count + " " + item.Name);
					else
						pak.WritePascalString(item.Name);
				}
			}
			SendTCP(pak);
		}
	}
}
					/*
					pak.WriteByte((byte)updatedSlot);
					GenericItem item = null;
					item = m_gameClient.Player.Inventory.GetItem((eInventorySlot)updatedSlot);

					if (item == null)
					{
						pak.Fill(0x00, 19);
						continue;
					}

					pak.WriteByte((byte)item.Level);

					int value1; // some object types use this field to display count
					int value2; // some object types use this field to display count
					switch (item.ObjectType)
					{
						case (int)eObjectType.Arrow:
						case (int)eObjectType.Bolt:
						case (int)eObjectType.Poison:
						case (int)eObjectType.GenericItem:
							value1 = (StackableItem)item.Count;
							value2 = item.SPD_ABS;
							break;
						case (int)eObjectType.Thrown:
							value1 = item.DPS_AF;
							value2 = item.Count;
							break;
						case (int)eObjectType.Instrument:
							value1 = (item.DPS_AF == 2 ? 0 : item.DPS_AF);
							value2 = 0;
							break; // unused
						case (int)eObjectType.Shield:
							value1 = item.Type_Damage;
							value2 = item.DPS_AF;
							break;
						case (int)eObjectType.AlchemyTincture:
						case (int)eObjectType.SpellcraftGem:
							value1 = 0;
							value2 = 0;
							/*
							must contain the quality of gem for spell craft and think same for tincture
							
							break;
						case (int)eObjectType.GardenObject:
							value1 = 0;
							value2 = 0;
							/*
							Value2 byte sets the width, only lower 4 bits 'seem' to be used (so 1-15 only)

							The byte used for "Hand" (IE: Mini-delve showing a weapon as Left-Hand
							usabe/TwoHanded), the lower 4 bits store the height (1-15 only)
							*							break;

						default:
							value1 = item.DPS_AF;
							value2 = item.SPD_ABS;
							break;
					}
					pak.WriteByte((byte)value1);
					pak.WriteByte((byte)value2);

					pak.WriteByte((byte)(item.Hand << 6));
					pak.WriteByte((byte)((item.Type_Damage > 3 ? 0 : item.Type_Damage << 6) + item.Object_Type));
					pak.WriteShort((ushort)item.Weight);
					pak.WriteByte(item.ConditionPercent); // % of con
					pak.WriteByte(item.DurabilityPercent); // % of dur
					pak.WriteByte((byte)item.Quality); // % of qua
					pak.WriteByte((byte)item.Bonus); // % bonus
					pak.WriteShort((ushort)item.Model);
					pak.WriteByte((byte)item.Extension);
					if (item.Emblem != 0)
						pak.WriteShort((ushort)item.Emblem);
					else
						pak.WriteShort((ushort)item.Color);
					byte flag = 0;
					   if (item.ConditionPercent < 100 && DOL.GS.Repair.IsAllowedToBeginWork(m_gameClient.Player, item, 50))
							flag |= 0x01; // enable repair button ? (always enabled if condition < 100)
					   flag |= 0x02; // enable salvage button
					   AbstractCraftingSkill skill = CraftingMgr.getSkillbyEnum(m_gameClient.Player.CraftingPrimarySkill);
					   if (skill != null && skill is AdvancedCraftingSkill/* && ((AdvancedCraftingSkill)skill).IsAllowedToCombine(m_gameClient.Player, item))
							flag |= 0x04; // enable craft button
					ushort icon1 = 0;
					ushort icon2 = 0;
					string spell_name1 = "";
					string spell_name2 = "";
					if (item.SpellID > 0/* && item.Charges > 0)
					{
						SpellLine chargeEffectsLine = SkillBase.GetSpellLine(GlobalSpellsLines.Item_Effects);
						if (chargeEffectsLine != null)
						{
							IList spells = SkillBase.GetSpellList(chargeEffectsLine.KeyName);
							if (spells != null)
							{
								foreach (Spell spl in spells)
								{
									if (spl.ID == item.SpellID)
									{
										flag |= 0x08;
										icon1 = spl.Icon;
										spell_name1 = spl.SpellType; // or best spl.Name ?
										break;
									}
								}
							}
						}
					}
					if (item.SpellID > 0/* && item.Charges > 0)
					{
						SpellLine chargeEffectsLine = SkillBase.GetSpellLine(GlobalSpellsLines.Item_Effects);
						if (chargeEffectsLine != null)
						{
							IList spells = SkillBase.GetSpellList(chargeEffectsLine.KeyName);
							if (spells != null)
							{
								foreach (Spell spl in spells)
								{
									if (spl.ID == item.SpellID)
									{
										flag |= 0x10;
										icon2 = spl.Icon;
										spell_name2 = spl.SpellType; // or best spl.Name ?
										break;
									}
								}
							}
						}
					}
					pak.WriteByte((byte)flag);
					if ((flag & 0x08) == 0x08)
					{
						pak.WriteShort((ushort)icon1);
						pak.WritePascalString(spell_name1);
					}
					if ((flag & 0x10) == 0x10)
					{
						pak.WriteShort((ushort)icon2);
						pak.WritePascalString(spell_name2);
					}
					pak.WriteByte((byte)item.Effect);
					//                                      pak.WriteShort((ushort) item.Effect);
					if (item.Count > 1)
						pak.WritePascalString(item.Count + " " + item.Name);
					else
						pak.WritePascalString(item.Name);
				}
			}
			SendTCP(pak);
		}
	}
}*/
