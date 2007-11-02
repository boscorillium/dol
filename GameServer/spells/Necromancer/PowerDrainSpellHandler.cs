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
using DOL.GS.PacketHandler;
using DOL.AI.Brain;

namespace DOL.GS.Spells
{
	/// <summary>
	/// Handles power drain (conversion of target health to caster
	/// power).
	/// </summary>
	[SpellHandlerAttribute("PowerDrain")]
	public class PowerDrainSpellHandler : DirectDamageSpellHandler
	{
		/// <summary>
		/// Execute direct effect.
		/// </summary>
		/// <param name="target">Target that takes the damage.</param>
		/// <param name="effectiveness">Effectiveness of the spell (0..1, equalling 0-100%).</param>
		public override void OnDirectEffect(GameLiving target, double effectiveness)
		{
			if (target == null) return;
			if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;

			// Calculate damage to the target.

			AttackData ad = CalculateDamageToTarget(target, effectiveness);
			SendDamageMessages(ad);
			DamageTarget(ad, true);
		    DrainPower(ad);
			target.StartInterruptTimer(SPELL_INTERRUPT_DURATION, ad.AttackType, Caster);
		}

		/// <summary>
		/// Use a percentage of the damage to refill caster's power.
		/// </summary>
		/// <param name="ad">Attack data.</param>
		public virtual void DrainPower(AttackData ad)
		{
			if (ad == null || !m_caster.IsAlive) return;

			int power = (ad.Damage + ad.CriticalDamage) * m_spell.LifeDrainReturn / 100;
			power = m_caster.ChangeMana(m_caster, GameLiving.eManaChangeType.Spell, power);
				
			if (power > 0)
				MessageToOwner(String.Format("Your summon channels {0} power to you!", power), eChatType.CT_Spell);
			else
				MessageToOwner("You cannot absorb any more power.", eChatType.CT_SpellResisted);
		}

		/// <summary>
		/// Calculates the base 100% spell damage which is then modified by damage variance factors
		/// </summary>
		/// <returns></returns>
		public override double CalculateDamageBase()
		{
			double spellDamage = Spell.Damage;
			GamePlayer player = Caster as GamePlayer;
			if (player != null && player.CharacterClass.ManaStat != eStat.UNDEFINED)
			{
				int manaStatValue = player.GetModified((eProperty)player.CharacterClass.ManaStat);
				spellDamage *= (manaStatValue + 180) / 250.0;
				if (spellDamage < 0)
					spellDamage = 0;
			}
			return spellDamage;
		}

        /// <summary>
        /// Send message to owner.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        private void MessageToOwner(String message, eChatType chatType)
        {
			GameNPC npc = Caster as GameNPC;
			if (npc != null)
			{
				ControlledNpc brain = npc.Brain as ControlledNpc;
				if (brain != null)
				{
					GamePlayer owner = brain.Owner as GamePlayer;
					if (owner != null)
						owner.Out.SendMessage(message, chatType, eChatLoc.CL_SystemWindow);
				}
			}
        }

		/// <summary>
        /// Create a new handler for the power drain spell.
		/// </summary>
		/// <param name="caster"></param>
		/// <param name="spell"></param>
		/// <param name="line"></param>
		public PowerDrainSpellHandler(GameLiving caster, Spell spell, SpellLine line) 
            : base(caster, spell, line) { }
	}
}