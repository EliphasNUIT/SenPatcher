﻿using HyoutaUtils;
using System.Collections.Generic;
using System.IO;

namespace SenLib.Sen1.FileFixes {
	public class text_dat_us_t_voice_tbl : FileMod {
		public string GetDescription() {
			return "Fix a few incorrect voice clips (Alisa in Chapter 3, Sara in Finale) and add missing lipsync for the extra PC voices.";
		}

		public IEnumerable<FileModResult> TryApply(FileStorage storage) {
			var voicetable = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xdc8fa92820abc1b4ul, 0x6a646b4d75ba5d23ul, 0x9bd22ee9u));
			var saraclip = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0x0037f27d49910859ul, 0x38a613aae6000493ul, 0x68768874u));
			var alisaclip = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xe9bd9a1d50cf6170ul, 0x728942e9fead6f2bul, 0xa635fcc6u));
			if (voicetable == null || saraclip == null || alisaclip == null) {
				return null;
			}

			var data = new VoiceTable(voicetable);
			data.Entries.Find(x => x.Index == 64300).Name = "pc8v10299";
			data.Entries.Find(x => x.Index == 61752).Name += "_a"; // alternate take of the same line; PS4 uses this one and I agree, it's the better take in that context

			MemoryStream newvoicetable = new MemoryStream();
			data.WriteToStream(newvoicetable, EndianUtils.Endianness.LittleEndian);

			// We use the PS4 timings for here because several lines (particularly in the Prologue) were re-recorded between PS3 and PC
			// and the PS4 timings should match that better.
			// Note that the PC version by default uses timings that exactly match the PS3 version, so they were not updated for that release.
			// NOTE: I originally thought I needed to copy e8v04585 from 3 to 4 since we use the PS3 line there
			// (Emma's "class spirit" line in the final dungeon) but it turns out those have the same timings in both versions,
			// so that's not actually necessary.
			//var voicetiming3 = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0x4530f8f99feffdd6ul, 0xbdb3b6e43ab769d0ul, 0xa0e9d5d8u));
			var voicetiming4 = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0x47b8e01924115c99ul, 0x14f4a5fa65873309ul, 0xc5825a41u));
			if (voicetiming4 == null) {
				return null;
			}

			return new FileModResult[] {
				new FileModResult("data/text/dat_us/t_voice.tbl", newvoicetable),
				new FileModResult("data/text/dat_us/t_vctiming.tbl", voicetiming4),
				new FileModResult("data/voice/wav/pc8v10299.wav", saraclip),
				new FileModResult("data/voice/wav/pc8v02551.wav", alisaclip),
			};
		}

		public IEnumerable<FileModResult> TryRevert(FileStorage storage) {
			var voicetable = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xdc8fa92820abc1b4ul, 0x6a646b4d75ba5d23ul, 0x9bd22ee9u));
			var voicetiming = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xf4b9ff78474452aaul, 0xc44f4b0c07c5a3ccul, 0x1ce27359u));
			var alisaclip = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0x6d43ad75d01d9acdul, 0x887826db59961c3eul, 0x925ccc02u));
			if (voicetable == null || voicetiming == null || alisaclip == null) {
				return null;
			}

			return new FileModResult[] {
				new FileModResult("data/text/dat_us/t_voice.tbl", voicetable),
				new FileModResult("data/text/dat_us/t_vctiming.tbl", voicetiming),
				new FileModResult("data/voice/wav/pc8v10299.wav", null),
				new FileModResult("data/voice/wav/pc8v02551.wav", alisaclip),
			};
		}
	}
}
