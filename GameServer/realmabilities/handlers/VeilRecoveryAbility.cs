using System;
using DOL.Database;
using DOL.GS.PropertyCalc;

namespace DOL.GS.RealmAbilities
{
	public class VeilRecoveryAbility : RAPropertyEnhancer
	{
		public VeilRecoveryAbility(DBAbility dba, int level)
			: base(dba, level, eProperty.Undefined)
		{
		}

		protected override string ValueUnit { get { return "%"; } }

		public override int GetAmountForLevel(int level)
		{
			switch (level)
			{
				case 1: return 10;
				case 2: return 20;
				case 3: return 40;
				case 4: return 60;
				case 5: return 80;
				default: return 0;
			}
		}
	}
}