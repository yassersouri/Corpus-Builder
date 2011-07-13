using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerbInflector
{
	[Flags]
	public enum NumberType
	{
		INVALID = 0,
		SINGULAR = 1,
		PLURAL = 2
	}


	[Flags]
	public enum Chasbidegi
	{
		TANHA = 0,
		NEXT = 1,
		PREV = 2
	}
	[Flags]
	public enum ZamirPeyvastehType
	{
		ZamirPeyvasteh_NONE = 1,
		AVALSHAKHS_MOFRAD = 2,
		DOVVOMSHAKHS_MOFRAD = 4,
		SEVVOMSHAKHS_MOFRAD = 8,
		AVALSHAKHS_JAM = 16,
		DOVVOMSHAKHS_JAM = 32,
		SEVVOMSHAKHS_JAM = 64
	}
	[Flags]
	public enum ShakhsType
	{
		Shakhs_NONE = 1,
		AVALSHAKHS_MOFRAD = 2,
		DOVVOMSHAKHS_MOFRAD = 4,
		SEVVOMSHAKHS_MOFRAD = 8,
		AVALSHAKHS_JAM = 16,
		DOVVOMSHAKHS_JAM = 32,
		SEVVOMSHAKHS_JAM = 64
	}
	[Flags]
	public enum TenseFormationType
	{
		TenseFormationType_NONE = 0,
		HAAL_SAADEH_EKHBARI = 1,
		HAAL_ELTEZAMI = 2,
		HAAL_SAADEH = 4,
		AMR = 8,
		GOZASHTEH_SADEH = 32,
		GOZASHTEH_ESTEMRAARI = 64,
		GOZASHTEH_NAGHLI_SADEH = 128,
		GOZASHTEH_NAGHLI_ESTEMRAARI = 256,
		GOZASHTEH_BAEED = 512,
		GOZASHTEH_ELTEZAMI = 1024,
		PAYEH_MAFOOLI = 2048,
		AAYANDEH = 4096,
		GOZASHTEH_ABAD = 8192
	}
	[Flags]
	public enum TensePositivity
	{
		POSITIVE = 1,
		NEGATIVE = 2
	}
	[Flags]
	public enum TensePassivity
	{
		ACTIVE = 1,
		PASSIVE = 2
	}
}
