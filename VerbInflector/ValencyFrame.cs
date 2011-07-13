using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentenceRecognizer
{
	public class ValencyFrame : IComparable
	{
		public List<ValencySlot> ValencyList { set; get; }
		public int NumOfDifferentFrames { set; get; }
		public string ValencyString { set; get; }
		public ValencyFrame(List<ValencySlot> valList, string valStr)
		{
			ValencyList = valList;
			ValencyString = valStr;
		}

		public int CompareTo(object obj)
		{
			var newObj = (ValencyFrame)obj;
			if (newObj.Equals(obj))
				return 0;
			return GetHashCode().CompareTo(newObj.GetHashCode());

		}

		public override bool Equals(object obj)
		{

			var newObj = (ValencyFrame)obj;
			if (newObj.ValencyString == ValencyString)
				return true;
			//if (newObj.ValencyList.Count == ValencyList.Count)
			//{

			//    return ValencyList.All(valencySlot => newObj.ValencyList.Contains(valencySlot));
			//}
			return false;
		}

		public override int GetHashCode()
		{
			return ValencyList.Sum(valencySlot => valencySlot.GetHashCode());
		}
	}

	public class ValencySlot : IComparable
	{
		public bool Obligatory { set; get; }
		public ValencySlotType ValencyType { set; get; }

		public ValencySlot(ValencySlotType valType, bool oblig)
		{
			ValencyType = valType;
			Obligatory = oblig;
		}

		public int CompareTo(object obj)
		{
			return GetHashCode().CompareTo(obj.GetHashCode());
		}
		public override bool Equals(object obj)
		{
			var newObj = (ValencySlot)obj;
			if (ValencyType.Equals(newObj.ValencyType) && Obligatory == newObj.Obligatory)
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Obligatory.GetHashCode() + ValencyType.GetHashCode();
		}
	}

	public abstract class ValencySlotType : IComparable
	{
		public string Name()
		{
			return "";
		}

		public string FullValence()
		{
			return Name();
		}

		public int CompareTo(object obj)
		{
			var newObj = (ValencySlotType)obj;
			if (GetType() == obj.GetType() && FullValence() == newObj.FullValence())
				return 0;
			return FullValence().CompareTo(newObj.FullValence());
		}

		public override bool Equals(object obj)
		{
			var newObj = (ValencySlotType)obj;
			return FullValence() == newObj.FullValence();
		}

		public override int GetHashCode()
		{
			return FullValence().GetHashCode();
		}
	}

	public class Fael : ValencySlotType
	{
		public new string Name()
		{
			return "فا";
		}
		public new string FullValence()
		{
			return Name();
		}
	}
	public class NULLSlot : ValencySlotType
	{
		public new string Name()
		{
			return "∅";
		}
		public new string FullValence()
		{
			return Name();
		}
	}
	public class Mafool : ValencySlotType
	{
		public string RaState { set; get; }

		public Mafool(string rState)
		{
			RaState = rState;
		}
		public new string Name()
		{
			return "مف";
		}
		public new string FullValence()
		{
			return Name() + "[" + RaState + "]";
		}
	}

	public class MafoolNeshanehEzafi : ValencySlotType
	{
		public new string Name()
		{
			return "مفن";
		}
		public new string FullValence()
		{
			return Name();
		}
	}

	public class MafoolHarfeEzafeh : ValencySlotType
	{
		public List<string> PrepositionList { set; get; }

		public MafoolHarfeEzafeh(List<string> plist)
		{
			PrepositionList = plist;
		}
		public new string Name()
		{
			return "مفح";
		}
		public new string FullValence()
		{
			string fullVal = Name() + "[";
			fullVal = PrepositionList.Aggregate(fullVal, (current, vals) => current + (vals + "|"));
			fullVal = fullVal.Remove(fullVal.Length - 1, 1);
			fullVal += "]";
			return fullVal;
		}
	}

	public class Mosnad : ValencySlotType
	{
		public new string Name()
		{
			return "مس";
		}
		public new string FullValence()
		{
			return Name();
		}
	}

	public class MotammemQeidi : ValencySlotType
	{
		public List<string> AdverbialCase { set; get; }

		public MotammemQeidi(List<string> clist)
		{
			AdverbialCase = clist;
		}
		public new string Name()
		{
			return "مق";
		}
		public new string FullValence()
		{
			return Name();
		}
	}

	public class BandMotammemi : ValencySlotType
	{
		public string Agreement { set; get; }
		public string Eltezami { set; get; }

		public BandMotammemi(string agr, string eltz)
		{
			Agreement = agr;
			Eltezami = eltz;
		}
		public new string Name()
		{
			return "بند";
		}
		public new string FullValence()
		{
			return Name() + "[" + Agreement + "," + Eltezami + "]";
		}
	}

	public class Tameez : ValencySlotType
	{
		public new string Name()
		{
			return "تم";
		}
		public new string FullValence()
		{
			return Name();
		}
	}

	public class MafoolDovvom : ValencySlotType
	{
		public new string Name()
		{
			return "مفد";
		}
		public new string FullValence()
		{
			return Name();
		}
	}
}
