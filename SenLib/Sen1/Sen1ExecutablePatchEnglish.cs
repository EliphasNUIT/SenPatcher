﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenLib.Sen1 {
	public class Sen1ExecutablePatchEnglish : Sen1ExecutablePatchInterface {
		public long GetAddressButtonBattleAnimationAutoSkip() {
			return 0x4d7c59;
		}

		public long GetAddressButtonBattleResultsAutoSkip() {
			return 0x4f2247;
		}

		public long GetAddressButtonTurboMode() {
			return 0x48300a;
		}

		public long GetAddressJumpBattleAnimationAutoSkip() {
			return 0x4d7c61;
		}

		public long GetAddressJumpBattleResultsAutoSkip() {
			return 0x4f224f;
		}

		public long GetAddressJumpR2NotebookOpen() {
			return 0x5b812f;
		}

		public long GetAddressJumpR2NotebookSettings() {
			return 0x6dfaf0;
		}

		public long GetRomAddressThorMasterQuartzTextureIdTypo() {
			return 0x73adba;
		}

		public List<FileFix> GetFileFixes() {
			var f = new List<FileFix>();
			f.Add(new FileFixes.scripts_scena_dat_us_t1000_dat());
			f.Add(new FileFixes.text_dat_us_t_magic_tbl());
			f.Add(new FileFixes.se_wav_ed8m2123_wav());
			f.Add(new FileFixes.se_wav_ed8m2150_wav());
			f.Add(new FileFixes.se_wav_ed8m4097_wav());
			f.Add(new FileFixes.se_wav_ed8m4217_wav());
			f.Add(new FileFixes.scripts_scena_dat_us_r0600_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0032_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t3500_dat());
			f.Add(new FileFixes.scripts_talk_dat_us_tk_heinrich_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_c0110_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0000_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t1500_dat());
			f.Add(new FileFixes.scripts_talk_dat_us_tk_beryl_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_c0100_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_a0006_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_r0800_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0010_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0031_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0050_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t0080_dat());
			f.Add(new FileFixes.scripts_scena_dat_us_t1010_dat());
			f.Add(new FileFixes.scripts_talk_dat_us_tk_edel_dat());
			f.Add(new FileFixes.scripts_talk_dat_us_tk_laura_dat());
			f.Add(new FileFixes.scripts_talk_dat_us_tk_vandyck_dat());
			return f;
		}
	}
}
