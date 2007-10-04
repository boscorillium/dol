using System.Reflection;
using log4net;
using System;
using System.Collections;
using DOL.GS.Effects;
using DOL.GS.PacketHandler;
using DOL.AI.Brain;
using DOL.GS;
using DOL.GS.Scripts;
using DOL.Events;
using System.Collections.Specialized;

namespace DOL.GS.Spells
{
    #region Sojourner-1
    //Gameplayer - MaxEncumbrance
    #endregion

    #region Sojourner-4
    [SpellHandlerAttribute("UnmakeCrystalseed")]
    public class UnmakeCrystalseedSpellHandler : SpellHandler
    {
        /// <summary>
        /// Execute unmake crystal seed spell
        /// </summary>
        /// <param name="target"></param>
        public override void FinishSpellCast(GameLiving target)
        {
            m_caster.Mana -= CalculateNeededPower(target);
            base.FinishSpellCast(target);
        }

        /// <summary>
        /// execute non duration spell effect on target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="effectiveness"></param>
        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            base.OnDirectEffect(target, effectiveness);
            if (target == null || !target.IsAlive)
                return;

            foreach (GameMine item in target.GetNPCsInRadius((ushort)m_spell.Radius))
            {
                if (item != null)
                {
                    item.Delete();
                }
            }
        }

        public UnmakeCrystalseedSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion

    #region Sojourner-5
    [SpellHandlerAttribute("AncientTransmuter")]
    public class AncientTransmuterSpellHandler : SpellHandler
    {
        private GameMerchant merchant;
        /// <summary>
        /// Execute Acient Transmuter summon spell
        /// </summary>
        /// <param name="target"></param>
        public override void FinishSpellCast(GameLiving target)
        {
            m_caster.Mana -= CalculateNeededPower(target);
            base.FinishSpellCast(target);
        }
        public override void OnEffectStart(GameSpellEffect effect)
        {
            base.OnEffectStart(effect);
            if (effect.Owner == null || !effect.Owner.IsAlive)
                return;

            merchant.AddToWorld();
        }
        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            if (merchant != null) merchant.Delete();
            return base.OnEffectExpires(effect, noMessages);
        }
        public AncientTransmuterSpellHandler(GameLiving caster, Spell spell, SpellLine line)
            : base(caster, spell, line)
        {
            if (caster is GamePlayer)
            {
                GamePlayer casterPlayer = caster as GamePlayer;
                merchant = new GameMerchant();
                //Fill the object variables
                merchant.X = casterPlayer.X + Util.Random(20, 40) - Util.Random(20, 40);
                merchant.Y = casterPlayer.Y + Util.Random(20, 40) - Util.Random(20, 40);
                merchant.Z = casterPlayer.Z;
                merchant.CurrentRegion = casterPlayer.CurrentRegion;
                merchant.Heading = (ushort)((casterPlayer.Heading + 2048) % 4096);
                merchant.Level = 1;
                merchant.Realm = casterPlayer.Realm;
                merchant.Name = "Ancient Transmuter";
                merchant.Model = 993;
                merchant.CurrentSpeed = 0;
                merchant.MaxSpeedBase = 0;
                merchant.GuildName = "";
                merchant.Size = 50;
                merchant.Flags |= (uint)GameNPC.eFlags.PEACE;
                merchant.TradeItems = new MerchantTradeItems("ML_transmuteritems");
            }
        }
    }
    #endregion

    #region Sojourner-6
    [SpellHandlerAttribute("Port")]
    public class Port : MasterlevelHandling
    {
        // constructor
        public Port(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

        public override void FinishSpellCast(GameLiving target)
        {
            base.FinishSpellCast(target);
        }

        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            if (target == null) return;
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;

            GamePlayer player = Caster as GamePlayer;

            if (player != null)
            {
                if (!player.InCombat)
                {
                    SendEffectAnimation(player, 0, false, 1);
                    player.MoveTo((ushort)player.PlayerCharacter.BindRegion, player.PlayerCharacter.BindXpos, player.PlayerCharacter.BindYpos, player.PlayerCharacter.BindZpos, (ushort)player.PlayerCharacter.BindHeading);
                }
            }
        }
    }
    #endregion

    #region Sojourner-8
    [SpellHandlerAttribute("Zephyr")]
    public class FZSpellHandler : MasterlevelHandling
    {
        protected RegionTimer m_expireTimer;
        protected GameNPC m_npc;
        protected GamePlayer m_target;

        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            if (target == null) return;
            GamePlayer player = target as GamePlayer;
            if (player != null && player.IsAlive)
            {
                Zephyr(player);
            }
        }

        public override bool CheckBeginCast(GameLiving target)
        {
            if (target == null)
            {
                MessageToCaster("You must select a target for this spell!", eChatType.CT_SpellResisted);
                return false;
            }

            if (target is GameNPC == true)
                return false;

            if (!GameServer.ServerRules.IsAllowedToAttack(Caster, target, true))
                return false;

            return base.CheckBeginCast(target);
        }

        private void Zephyr(GamePlayer target)
        {
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;
            GameNPC npc = new GameNPC();

            m_npc = npc;

            npc.Realm = Caster.Realm;
            npc.Heading = Caster.Heading;
            npc.Model = 1269;
            npc.Y = Caster.Y;
            npc.X = Caster.X;
            npc.Z = Caster.Z;
            npc.Name = "Forceful Zephyr";
            npc.MaxSpeedBase = 400;
            npc.Level = 55;
            npc.CurrentRegion = Caster.CurrentRegion;
            npc.Flags |= (uint)GameNPC.eFlags.PEACE;
            npc.Flags |= (uint)GameNPC.eFlags.DONTSHOWNAME;
            BlankBrain brain = new BlankBrain();
            npc.SetOwnBrain(brain);
            npc.AddToWorld();
            npc.TempProperties.setProperty("target", target);
            GameEventMgr.AddHandler(npc, GameNPCEvent.ArriveAtTarget, new DOLEventHandler(ArriveAtTarget));
            npc.Follow(target, 10, 1500);

            m_target = target;

            StartTimer();

        }

        protected virtual void StartTimer()
        {
            StopTimer();
            m_expireTimer = new RegionTimer(m_npc, new RegionTimerCallback(ExpiredCallback), 10000);
        }

        protected virtual int ExpiredCallback(RegionTimer callingTimer)
        {
            m_target.IsStunned = false;
            m_target.IsSilenced = false;
            GameEventMgr.RemoveHandler(m_target, GamePlayerEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            m_npc.StopMoving();
            m_npc.RemoveFromWorld();
            return 0;
        }

        protected virtual void StopTimer()
        {

            if (m_expireTimer != null)
            {
                m_expireTimer.Stop();
                m_expireTimer = null;
            }

        }

        private void OnAttack(DOLEvent e, object sender, EventArgs arguments)
        {
            GameLiving living = sender as GameLiving;
            if (living == null) return;
            AttackedByEnemyEventArgs attackedByEnemy = arguments as AttackedByEnemyEventArgs;
            AttackData ad = null;
            if (attackedByEnemy != null)
                ad = attackedByEnemy.AttackData;

            double absorbPercent = 100;
            int damageAbsorbed = (int)(0.01 * absorbPercent * (ad.Damage + ad.CriticalDamage));
            int spellAbsorbed = (int)(0.01 * absorbPercent * Spell.Damage);

            ad.Damage -= damageAbsorbed;
            ad.Damage -= spellAbsorbed;

            MessageToLiving(ad.Target, string.Format("You're in a Zephyr and can't be attacked!"), eChatType.CT_Spell);
            MessageToLiving(ad.Attacker, string.Format("Your target is in a Zephyr and can't be attacked!"), eChatType.CT_Spell);
        }

        private void ArriveAtTarget(DOLEvent e, object obj, EventArgs args)
        {
            GameNPC npc = obj as GameNPC;

            if (npc == null) return;

            GamePlayer target = npc.TempProperties.getObjectProperty("target", null) as GamePlayer;

            if (target == null || !target.IsAlive) return;
            GameEventMgr.RemoveHandler(npc, GameNPCEvent.ArriveAtTarget, new DOLEventHandler(ArriveAtTarget));

            GamePlayer player = target as GamePlayer;
            if (player == null) return;
            if (!player.IsAlive) return;

            player.IsStunned = true;
            player.IsSilenced = true;
            player.StopAttack();
            player.StopCurrentSpellcast();
            player.MountSteed(npc, true);
            GameEventMgr.AddHandler(player, GamePlayerEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));

            player.Out.SendMessage("You are picked up by a forceful zephyr!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            npc.StopFollow();

            if (Caster is GamePlayer)
            {
                //Calculate random target
                IPoint3D loc = GetTargetLoc();
                //Set a gt to the Caster for the Loscheck
                Caster.SetGroundTarget(loc.X, loc.Y, loc.Z);
                //Check los and start the moving or stay on the spot (works on live the same)
                if (Caster.GroundTargetInView)
                    m_npc.WalkTo(loc.X, loc.Y, loc.Z, 100);
            }
        }

        public virtual IPoint3D GetTargetLoc()
        {
            double targetX = m_npc.X + Util.Random(-1500, 1500);
            double targetY = m_npc.Y + Util.Random(-1500, 1500);

            return new Point3D((int)targetX, (int)targetY, m_npc.Z);
        }

        public override int CalculateSpellResistChance(GameLiving target)
        {
            return 0;
        }

        public FZSpellHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion

    #region Sojourner-9
    [SpellHandlerAttribute("Phaseshift")]
    public class PhaseshiftHandler : MasterlevelHandling
    {
        private int endurance;

        public override bool CheckBeginCast(GameLiving selectedTarget)
        {
            endurance = (Caster.MaxEndurance * 50) / 100;

            if (Caster.Endurance < endurance)
            {
                MessageToCaster("You need 50% endurance for this spell!!", eChatType.CT_System);
                return false;
            }

            return base.CheckBeginCast(selectedTarget);
        }

        public override void OnEffectStart(GameSpellEffect effect)
        {
            base.OnEffectStart(effect);
            GameEventMgr.AddHandler(Caster, GamePlayerEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            Caster.Endurance -= endurance;
        }

        private void OnAttack(DOLEvent e, object sender, EventArgs arguments)
        {
            GameLiving living = sender as GameLiving;
            if (living == null) return;
            AttackedByEnemyEventArgs attackedByEnemy = arguments as AttackedByEnemyEventArgs;
            AttackData ad = null;
            if (attackedByEnemy != null)
                ad = attackedByEnemy.AttackData;

            if (ad.Attacker is GamePlayer)
            {
                ad.Damage = 0;
                ad.CriticalDamage = 0;
                GamePlayer player = ad.Attacker as GamePlayer;
                player.Out.SendMessage(living.Name + " is Phaseshifted and can't be attacked!", eChatType.CT_Missed, eChatLoc.CL_SystemWindow);
            }
        }

        public override int OnEffectExpires(GameSpellEffect effect, bool noMessages)
        {
            GameEventMgr.RemoveHandler(Caster, GamePlayerEvent.AttackedByEnemy, new DOLEventHandler(OnAttack));
            return base.OnEffectExpires(effect, noMessages);
        }

        public override bool HasPositiveEffect
        {
            get
            {
                return false;
            }
        }

        public override int CalculateSpellResistChance(GameLiving target)
        {
            return 0;
        }

        // constructor
        public PhaseshiftHandler(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }
    }
    #endregion

    #region Sojourner-10
    [SpellHandlerAttribute("Groupport")]
    public class Groupport : MasterlevelHandling
    {
        public Groupport(GameLiving caster, Spell spell, SpellLine line) : base(caster, spell, line) { }

        public override bool CheckBeginCast(GameLiving selectedTarget)
        {
            if (Caster is GamePlayer && Caster.CurrentRegionID == 51 && ((GamePlayer)Caster).PlayerCharacter.BindRegion == 51)
            {
                if (Caster.CurrentRegionID == 51)
                {
                    MessageToCaster("You can't use this Ability here", eChatType.CT_SpellResisted);
                    return false;
                }
                else
                {
                    MessageToCaster("Bind in another Region to use this Ability", eChatType.CT_SpellResisted);
                    return false;
                }
            }
            return base.CheckBeginCast(selectedTarget);
        }

        public override void FinishSpellCast(GameLiving target)
        {
            base.FinishSpellCast(target);
        }

        public override void OnDirectEffect(GameLiving target, double effectiveness)
        {
            if (target == null) return;
            if (!target.IsAlive || target.ObjectState != GameLiving.eObjectState.Active) return;

            GamePlayer player = Caster as GamePlayer;
            if ((player != null) && (player.PlayerGroup != null))
            {
                if (player.PlayerGroup.IsGroupInCombat())
                {
                    player.Out.SendMessage("You can't teleport a group that is in combat!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                    return;
                }
                else
                {
                    foreach (GamePlayer pl in player.PlayerGroup.GetPlayersInTheGroup())
                    {
                        if (pl != null)
                        {
                            SendEffectAnimation(pl, 0, false, 1);
                            pl.MoveTo((ushort)player.PlayerCharacter.BindRegion, player.PlayerCharacter.BindXpos, player.PlayerCharacter.BindYpos, player.PlayerCharacter.BindZpos, (ushort)player.PlayerCharacter.BindHeading);
                        }
                    }
                }
            }
            else
            {
                player.Out.SendMessage("You are not a part of a group!", eChatType.CT_System, eChatLoc.CL_SystemWindow);
            }
        }
    }
    #endregion
}
