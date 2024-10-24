// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using HexaGen.Runtime;
using HexaGen.Runtime.COM;

namespace Hexa.NET.XAudio2
{
	/// <summary>	/// -------------------------------------------------------------------------<br/>	/// Description: AudioClient share mode<br/>	/// AUDCLNT_SHAREMODE_SHARED -    The device will be opened in shared mode and use the<br/>	/// WAS format.<br/>	/// AUDCLNT_SHAREMODE_EXCLUSIVE - The device will be opened in exclusive mode and use the<br/>	/// application specified format.<br/>	/// </summary>	[NativeName(NativeNameType.Enum, "_AUDCLNT_SHAREMODE")]
	public enum AudclntSharemode
	{
		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AUDCLNT_SHAREMODE_SHARED")]
		[NativeName(NativeNameType.Value, "0")]
		Shared = unchecked(0),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AUDCLNT_SHAREMODE_EXCLUSIVE")]
		[NativeName(NativeNameType.Value, "1")]
		Exclusive = unchecked(1),

	}

	/// <summary>	/// -------------------------------------------------------------------------<br/>	/// Description: Audio stream categories<br/>	/// ForegroundOnlyMedia     - (deprecated for Win10) Music, Streaming audio<br/>	/// BackgroundCapableMedia  - (deprecated for Win10) Video with audio<br/>	/// Communications          - VOIP, chat, phone call<br/>	/// Alerts                  - Alarm, Ring tones<br/>	/// SoundEffects            - Sound effects, clicks, dings<br/>	/// GameEffects             - Game sound effects<br/>	/// GameMedia               - Background audio for games<br/>	/// GameChat                - In game player chat<br/>	/// Speech                  - Speech recognition<br/>	/// Media                   - Music, Streaming audio<br/>	/// Movie                   - Video with audio<br/>	/// FarFieldSpeech          - Capture of far field speech<br/>	/// UniformSpeech           - Uniform, device agnostic speech processing<br/>	/// VoiceTyping             - Dictation, typing by voice<br/>	/// Other                   - All other streams (default)<br/>	/// </summary>	[NativeName(NativeNameType.Enum, "_AUDIO_STREAM_CATEGORY")]
	public enum AudioStreamCategory
	{
		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Other")]
		[NativeName(NativeNameType.Value, "0")]
		Other = unchecked(0),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_ForegroundOnlyMedia")]
		[NativeName(NativeNameType.Value, "1")]
		ForegroundOnlyMedia = unchecked(1),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Communications")]
		[NativeName(NativeNameType.Value, "3")]
		Communications = unchecked(3),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Alerts")]
		[NativeName(NativeNameType.Value, "4")]
		Alerts = unchecked(4),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_SoundEffects")]
		[NativeName(NativeNameType.Value, "5")]
		SoundEffects = unchecked(5),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_GameEffects")]
		[NativeName(NativeNameType.Value, "6")]
		GameEffects = unchecked(6),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_GameMedia")]
		[NativeName(NativeNameType.Value, "7")]
		GameMedia = unchecked(7),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_GameChat")]
		[NativeName(NativeNameType.Value, "8")]
		GameChat = unchecked(8),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Speech")]
		[NativeName(NativeNameType.Value, "9")]
		Speech = unchecked(9),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Movie")]
		[NativeName(NativeNameType.Value, "10")]
		Movie = unchecked(10),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_Media")]
		[NativeName(NativeNameType.Value, "11")]
		Media = unchecked(11),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_FarFieldSpeech")]
		[NativeName(NativeNameType.Value, "12")]
		FarFieldSpeech = unchecked(12),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_UniformSpeech")]
		[NativeName(NativeNameType.Value, "13")]
		UniformSpeech = unchecked(13),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioCategory_VoiceTyping")]
		[NativeName(NativeNameType.Value, "14")]
		VoiceTyping = unchecked(14),

	}

	/// <summary>	/// -------------------------------------------------------------------------<br/>	/// Description: AudioSession State.<br/>	/// AudioSessionStateInactive - The session has no active audio streams.<br/>	/// AudioSessionStateActive - The session has active audio streams.<br/>	/// AudioSessionStateExpired - The session is dormant.<br/>	/// </summary>	[NativeName(NativeNameType.Enum, "_AudioSessionState")]
	public enum AudioSessionState
	{
		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioSessionStateInactive")]
		[NativeName(NativeNameType.Value, "0")]
		Inactive = unchecked(0),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioSessionStateActive")]
		[NativeName(NativeNameType.Value, "1")]
		Active = unchecked(1),

		/// <summary>		/// To be documented.		/// </summary>		[NativeName(NativeNameType.EnumItem, "AudioSessionStateExpired")]
		[NativeName(NativeNameType.Value, "2")]
		Expired = unchecked(2),

	}

	/// <summary>	/// Used in XAUDIO2_FILTER_PARAMETERS below<br/>	/// </summary>	[NativeName(NativeNameType.Enum, "XAUDIO2_FILTER_TYPE")]
	public enum XAudio2FilterType
	{
		/// <summary>		/// Attenuates frequencies above the cutoff frequency (state-variable filter).<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "LowPassFilter")]
		[NativeName(NativeNameType.Value, "0")]
		LowPassFilter = unchecked(0),

		/// <summary>		/// Attenuates frequencies outside a given range      (state-variable filter).<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "BandPassFilter")]
		[NativeName(NativeNameType.Value, "1")]
		BandPassFilter = unchecked(1),

		/// <summary>		/// Attenuates frequencies below the cutoff frequency (state-variable filter).<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "HighPassFilter")]
		[NativeName(NativeNameType.Value, "2")]
		HighPassFilter = unchecked(2),

		/// <summary>		/// Attenuates frequencies inside a given range       (state-variable filter).<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "NotchFilter")]
		[NativeName(NativeNameType.Value, "3")]
		NotchFilter = unchecked(3),

		/// <summary>		/// Attenuates frequencies above the cutoff frequency (one-pole filter, XAUDIO2_FILTER_PARAMETERS.OneOverQ has no effect)<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "LowPassOnePoleFilter")]
		[NativeName(NativeNameType.Value, "4")]
		LowPassOnePoleFilter = unchecked(4),

		/// <summary>		/// Attenuates frequencies below the cutoff frequency (one-pole filter, XAUDIO2_FILTER_PARAMETERS.OneOverQ has no effect)<br/>		/// </summary>		[NativeName(NativeNameType.EnumItem, "HighPassOnePoleFilter")]
		[NativeName(NativeNameType.Value, "5")]
		HighPassOnePoleFilter = unchecked(5),

	}

}
