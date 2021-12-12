using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboVX.Combos {
	internal static class BLM {
		public const byte JobID = 25;

		public const uint
			Fire = 141,
			Blizzard = 142,
			Thunder = 144,
			Blizzard2 = 146,
			Transpose = 149,
			Fire3 = 152,
			Thunder3 = 153,
			Blizzard3 = 154,
			Scathe = 156,
			Freeze = 159,
			Flare = 162,
			LeyLines = 3573,
			Blizzard4 = 3576,
			Fire4 = 3577,
			BetweenTheLines = 7419,
			Despair = 16505,
			UmbralSoul = 16506,
			Xenoglossy = 16507,
			Paradox = 25797;

		public static class Buffs {
			public const ushort
				Thundercloud = 164,
				LeyLines = 737,
				Firestarter = 165;
		}

		public static class Debuffs {
			public const ushort
				Thunder = 161,
				Thunder3 = 163;
		}

		public static class Levels {
			public const byte
				Fire3 = 35,
				Blizzard3 = 35,
				Freeze = 40,
				Thunder3 = 45,
				Flare = 50,
				Blizzard4 = 58,
				Fire4 = 60,
				BetweenTheLines = 62,
				Despair = 72,
				UmbralSoul = 76,
				Xenoglossy = 80,
				HighFire2 = 82,
				HighBlizzard2 = 82,
				Paradox = 90;
		}
	}

	internal class BlackEnochianFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackEnochianFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Fire4, BLM.Blizzard4 };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Fire4 or BLM.Blizzard4) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.Blizzard4 && gauge.InUmbralIce)
					return BLM.Blizzard4;

				if (level >= BLM.Levels.Fire4)
					return BLM.Fire4;

			}

			return actionID;
		}
	}

	internal class BlackManaFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackManaFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Transpose };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Transpose) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.UmbralSoul && gauge.InUmbralIce && gauge.IsEnochianActive)
					return BLM.UmbralSoul;

			}

			return actionID;
		}
	}

	internal class BlackLeyLinesFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackLeyLinesFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.LeyLines };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.LeyLines) {

				if (level >= BLM.Levels.BetweenTheLines && SelfHasEffect(BLM.Buffs.LeyLines))
					return BLM.BetweenTheLines;

			}

			return actionID;
		}
	}

	internal class BlackBlizzardFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackBlizzardFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Blizzard };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Blizzard) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.Blizzard3 && !gauge.InUmbralIce)
					return BLM.Blizzard3;

			}

			return actionID;
		}
	}

	internal class BlackFreezeFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFreezeFlareFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Freeze, BLM.Flare };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Freeze or BLM.Flare) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.Freeze && gauge.InUmbralIce)
					return BLM.Freeze;
				if (level >= BLM.Levels.Flare)
					return BLM.Flare;

			}

			return actionID;
		}
	}

	internal class BlackFireFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFireFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Fire };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Fire) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.Fire3 && (!gauge.InAstralFire || SelfHasEffect(BLM.Buffs.Firestarter)))
					return BLM.Fire3;
				return OriginalHook(BLM.Fire);
			}


			return actionID;
		}
	}

	internal class BlackScatheFeature: CustomCombo {
		protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackScatheFeature;
		protected internal override uint[] ActionIDs { get; } = new[] { BLM.Scathe };

		protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) {

			if (actionID is BLM.Scathe) {

				BLMGauge gauge = GetJobGauge<BLMGauge>();
				if (level >= BLM.Levels.Xenoglossy && gauge.PolyglotStacks > 0)
					return BLM.Xenoglossy;

			}

			return actionID;
		}
	}
}
