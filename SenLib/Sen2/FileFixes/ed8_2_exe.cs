﻿using HyoutaUtils;
using System.Collections.Generic;
using System.IO;

namespace SenLib.Sen2.FileFixes {
	public class ed8_2_exe : FileMod {
		bool IsJp;
		bool RemoveTurboSkip;
		bool PatchAudioThread;
		int AudioThreadDivisor;
		bool PatchBgmQueueing;
		bool CorrectLanguageVoiceTables;
		bool DisableMouseCapture;
		bool DisablePauseOnFocusLoss;

		public ed8_2_exe(
			bool jp,
			bool removeTurboSkip,
			bool patchAudioThread,
			int audioThreadDivisor,
			bool patchBgmQueueing,
			bool correctLanguageVoiceTables,
			bool disableMouseCapture,
			bool disablePauseOnFocusLoss
		) {
			IsJp = jp;
			RemoveTurboSkip = removeTurboSkip;
			PatchAudioThread = patchAudioThread;
			AudioThreadDivisor = audioThreadDivisor;
			PatchBgmQueueing = patchBgmQueueing;
			CorrectLanguageVoiceTables = correctLanguageVoiceTables;
			DisableMouseCapture = disableMouseCapture;
			DisablePauseOnFocusLoss = disablePauseOnFocusLoss;
		}

		public string GetDescription() {
			return string.Format("Main executable code changes for {0} language.", IsJp ? "Japanese" : "English");
		}

		private HyoutaUtils.Checksum.SHA1 GetExecutableHash() {
			return IsJp ? new HyoutaUtils.Checksum.SHA1(0x7d1db7e0bb91ab77ul, 0xa3fd1eba53b0ed25ul, 0x806186c1u) : new HyoutaUtils.Checksum.SHA1(0xb08ece4ee38e6e3aul, 0x99e58eb11cffb45eul, 0x49704f86u);
		}

		public IEnumerable<FileModResult> TryApply(FileStorage storage) {
			var s = storage.TryGetDuplicate(GetExecutableHash());
			if (s == null) {
				return null;
			}

			MemoryStream ms = s.CopyToMemoryAndDispose();

			Sen2ExecutablePatchState state = new Sen2ExecutablePatchState(IsJp);
			state.InitCodeSpaceIfNeeded(ms);

			if (RemoveTurboSkip) {
				Sen2ExecutablePatches.PatchJumpBattleAnimationAutoSkip(ms, state);
				Sen2ExecutablePatches.PatchJumpBattleStartAutoSkip(ms, state);
				Sen2ExecutablePatches.PatchJumpBattleSomethingAutoSkip(ms, state);
				Sen2ExecutablePatches.PatchJumpBattleResultsAutoSkip(ms, state);
			}
			if (PatchAudioThread) {
				Sen2ExecutablePatches.PatchMusicFadeTiming(ms, state, AudioThreadDivisor <= 0 ? 1000 : (uint)AudioThreadDivisor);
			}
			if (PatchBgmQueueing) {
				Sen2ExecutablePatches.PatchMusicQueueingOnSoundThreadSide(ms, state);
			}
			if (CorrectLanguageVoiceTables) {
				Sen2ExecutablePatches.PatchLanguageAppropriateVoiceTables(ms, state);
			}
			if (DisableMouseCapture) {
				Sen2ExecutablePatches.PatchDisableMouseCapture(ms, state);
			}
			if (DisablePauseOnFocusLoss) {
				Sen2ExecutablePatches.PatchDisablePauseOnFocusLoss(ms, state);
			}

			// add indicator to the title screen that we're running a modified executable
			ms.Position = state.Mapper.MapRamToRom(state.PushAddressVersionString);
			uint addressVersionString = ms.ReadUInt32(EndianUtils.Endianness.LittleEndian);
			ms.Position = state.Mapper.MapRamToRom(addressVersionString);
			string versionString = ms.ReadAsciiNullterm();
			string newVersionString = versionString + "  SenPatcher " + Version.SenPatcherVersion;
			MemoryStream newVersionStringStream = new MemoryStream();
			newVersionStringStream.WriteAsciiNullterm(newVersionString);
			byte[] newVersionStringBytes = newVersionStringStream.CopyToByteArrayAndDispose();
			var regionStrings = state.RegionScriptCompilerFunctionStrings;
			uint addressNewVersionString = regionStrings.Address;
			ms.Position = state.Mapper.MapRamToRom(regionStrings.Address);
			ms.Write(newVersionStringBytes);
			regionStrings.TakeToAddress(state.Mapper.MapRomToRam(ms.Position), "SenPatcher version string");
			ms.Position = state.Mapper.MapRamToRom(state.PushAddressVersionString);
			ms.WriteUInt32(addressNewVersionString, EndianUtils.Endianness.LittleEndian);
			ms.Position = state.Mapper.MapRamToRom(state.PushAddressVersionStringTTY);
			ms.WriteUInt32(addressNewVersionString, EndianUtils.Endianness.LittleEndian);
			ms.Position = state.Mapper.MapRamToRom(state.PushAddressVersionCrashRpt);
			ms.WriteUInt32(addressNewVersionString + (uint)(versionString.Length - 5), EndianUtils.Endianness.LittleEndian);

			return new FileModResult[] { new FileModResult(IsJp ? "bin/Win32/ed8_2_PC_JP.exe" : "bin/Win32/ed8_2_PC_US.exe", ms) };
		}

		public IEnumerable<FileModResult> TryRevert(FileStorage storage) {
			// if we have both v1.4.1s revert to that, since that likely means that was the original input
			var us141 = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xd5c333b4cd517d43ul, 0xe3868e159fbec37dul, 0xba4122d6u));
			var jp141 = storage.TryGetDuplicate(new HyoutaUtils.Checksum.SHA1(0xb8158fb59e43c02eul, 0x904f813150d84133ul, 0x6d1a13e5u));
			if (us141 != null && jp141 != null) {
				return new FileModResult[] { new FileModResult(IsJp ? "bin/Win32/ed8_2_PC_JP.exe" : "bin/Win32/ed8_2_PC_US.exe", IsJp ? jp141 : us141) };
			}

			// if we don't, v1.4.2 definitely was the input, so revert to that
			var s = storage.TryGetDuplicate(GetExecutableHash());
			if (s == null) {
				return null;
			}
			return new FileModResult[] { new FileModResult(IsJp ? "bin/Win32/ed8_2_PC_JP.exe" : "bin/Win32/ed8_2_PC_US.exe", s) };
		}
	}
}
