using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SCICT.NLP.Utility;

namespace SentenceRecognizer
{
    public class ValencyDicManager
    {
        public static  Dictionary<string, List<BaseStructure>> BaseStrucDic;

        public ValencyDicManager()
        {
            BaseStrucDic = new Dictionary<string, List<BaseStructure>>();
        }

        public static ValencyFrame GetValencyFrame(string valencyStr)
        {
            var valencyFrame = new ValencyFrame(new List<ValencySlot>(), "");
            var split = valencyStr.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var valency = split[split.Length - 1].Trim();
            string newValency = valency.Substring(1, valency.Length - 2);
            valencyFrame.ValencyString = newValency;
            var valParts = newValency.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < valParts.Length; i++)
            {
                var valencySlot = new ValencySlot(new NULLSlot(), true);
                string valPart = valParts[i];
                bool obligat = true;
                if (valPart.Contains("("))
                {
                    obligat = false;
                    valPart = valPart.Replace("(", "");
                    valPart = valPart.Replace(")", "");
                }
                string ValValue = valPart;
                var features = "";
                if (valPart.Contains("["))
                {
                    valPart = valPart.Replace("]", "");
                    var splitedVal = valPart.Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    ValValue = splitedVal[0];
                    features = splitedVal[1];
                }
                switch (ValValue)
                {
                    case "فا":
                        valencySlot = new ValencySlot(new Fael(), obligat);
                        break;
                    case "مف":
                        valencySlot = new ValencySlot(new Mafool(features), obligat);
                        break;
                    case "مفح":
                        List<string> prepList =
                            features.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        valencySlot = new ValencySlot(new MafoolHarfeEzafeh(prepList), obligat);
                        break;
                    case "مفن":
                        valencySlot = new ValencySlot(new MafoolNeshanehEzafi(), obligat);
                        break;
                    case "مس":
                        valencySlot = new ValencySlot(new Mosnad(), obligat);
                        break;
                    case "بند":
                        int index = features.IndexOf("مطا");
                        string agreement = features.Substring(index, features.Length - index);
                        string eltezami = features.Substring(0, index);
                        valencySlot = new ValencySlot(new BandMotammemi(agreement, eltezami), obligat);
                        break;
                    case "تم":
                        valencySlot = new ValencySlot(new Tameez(), obligat);
                        break;
                    case "مفد":
                        valencySlot = new ValencySlot(new MafoolDovvom(), obligat);
                        break;
                    case "مق":
                        List<string> advCase =
                            features.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                        valencySlot = new ValencySlot(new MotammemQeidi(advCase), obligat);
                        break;
                }
                valencyFrame.ValencyList.Add(valencySlot);
            }
            return valencyFrame;
        }
        public void CountValencies(string path)
        {
            var reader = new StreamReader(path);
            string sentence;
            var valDic = new Dictionary<string, int>();
            while ((sentence = reader.ReadLine()) != null)
            {
                var valencyFrame = new ValencyFrame(new List<ValencySlot>(), "");
                var split = sentence.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var valency = split[split.Length - 1].Trim();
                string newValency = valency.Substring(1, valency.Length - 2);
                valencyFrame.ValencyString = newValency;
                var valParts = newValency.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < valParts.Length; i++)
                {
                    var valencySlot = new ValencySlot(new Fael(), true);
                    string valPart = valParts[i];
                    bool obligat = true;
                    if (valPart.Contains("("))
                    {
                        obligat = false;
                        valPart = valPart.Replace("(", "");
                        valPart = valPart.Replace(")", "");
                    }
                    string ValValue = valPart;
                    var features = "";
                    if (valPart.Contains("["))
                    {
                        valPart = valPart.Replace("]", "");
                        var splitedVal = valPart.Split("[".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        ValValue = splitedVal[0];
                        features = splitedVal[1];
                    }
                    switch (ValValue)
                    {
                        case "فا":
                            valencySlot = new ValencySlot(new Fael(), obligat);
                            break;
                        case "مف":
                            valencySlot = new ValencySlot(new Mafool(features), obligat);
                            break;
                        case "مفح":
                            List<string> prepList = features.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            valencySlot = new ValencySlot(new MafoolHarfeEzafeh(prepList), obligat);
                            break;
                        case "مفن":
                            valencySlot = new ValencySlot(new MafoolNeshanehEzafi(), obligat);
                            break;
                        case "مس":
                            valencySlot = new ValencySlot(new Mosnad(), obligat);
                            break;
                        case "بند":
                            int index = features.IndexOf("مطا");
                            string agreement = features.Substring(index, features.Length - index);
                            string eltezami = features.Substring(0, index - 1);
                            valencySlot = new ValencySlot(new BandMotammemi(agreement, eltezami), obligat);
                            break;
                        case "تم":
                            valencySlot = new ValencySlot(new Tameez(), obligat);
                            break;
                        case "مفد":
                            valencySlot = new ValencySlot(new MafoolDovvom(), obligat);
                            break;
                        case "مق":
                            List<string> advCase = features.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                            valencySlot = new ValencySlot(new MotammemQeidi(advCase), obligat); break;
                    }
                }
                if (valDic.ContainsKey(valency))
                {
                    valDic[valency]++;
                }
                else
                {
                    valDic.Add(valency, 1);
                }

            }

        }
        public static void RefreshVerbList(string verbpath, string valencePath)
        {
            BaseStrucDic = new Dictionary<string, List<BaseStructure>>(); 
            var verbValDic = new Dictionary<string, VerbPlusValency>();
            var reader = new StreamReader(verbpath);
            var newVerbReader = new StreamReader(valencePath);
            string sentence;
            var mainCatList = new List<List<string>>();
            while ((sentence = reader.ReadLine()) != null)
            {
                sentence = StringUtil.RefineAndFilterPersianWord(sentence);

                mainCatList.Add(sentence.Trim().Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList());
            }
            while ((sentence = newVerbReader.ReadLine()) != null)
            {
                sentence = StringUtil.RefineAndFilterPersianWord(sentence);
                var split = sentence.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string bonMazi = split[0];
                string bonMozare = split[1];
                string prefix = split[2];
                string nonVerbalElement = split[3];
                string preposition = split[4];

                foreach (List<string> list in mainCatList)
                {
                    if (bonMazi == list[2] && bonMozare == list[3] && prefix == list[5] && nonVerbalElement == list[4] && preposition == list[7])
                    {
                        string verbStr = prefix + "#" + bonMazi + "#" + bonMozare + "#" + nonVerbalElement + "#" + preposition;
                        if(prefix=="-")
                            verbStr = bonMazi + "#" + bonMozare + "#" + nonVerbalElement + "#" + preposition;
                        var newVerbPlusVal = new VerbPlusValency(verbStr, new List<ValencyFrame>());
                        if (!verbValDic.ContainsKey(verbStr))
                            verbValDic.Add(verbStr, newVerbPlusVal);

                        verbValDic[verbStr].AddNewFrame(GetValencyFrame(split[5]));

                        if (bonMazi == "کرد" && nonVerbalElement != "-")
                        {
                            string newVerbStr = prefix + "\t" +"\tنمود\tنما\t" +
                                         nonVerbalElement + "\t" +  preposition;
                            if(prefix=="-")
                                newVerbStr = "\t" + "\tنمود\tنما\t" + nonVerbalElement + "\t" + preposition;
                            var newerVerbPlusVal = new VerbPlusValency(newVerbStr, new List<ValencyFrame>());
                            if (!verbValDic.ContainsKey(newVerbStr))
                                verbValDic.Add(newVerbStr, newerVerbPlusVal);
                            verbValDic[newVerbStr].AddNewFrame(GetValencyFrame(split[5]));
                        }

                    }
                }
            }
            int maxTedad = 0;
            foreach (var verbPlusValency in verbValDic)
            {
                int counter = 0;
                int sumTedad = 0;
                foreach (ValencyFrame valencyFrame in verbPlusValency.Value.ValencyFrameList)
                {
                    int tedad = 1;
                    foreach (ValencySlot valencySlot in valencyFrame.ValencyList)
                    {
                        int localTedad = 1;
                        if (valencySlot.ValencyType is Mafool)
                        {
                            var maf = (Mafool)valencySlot.ValencyType;
                            if (maf.RaState == "را+/-")
                                localTedad *= 2;
                        }
                        if (valencySlot.ValencyType is MafoolHarfeEzafeh)
                        {
                            var mafh = (MafoolHarfeEzafeh)valencySlot.ValencyType;
                            localTedad *= mafh.PrepositionList.Count;
                        }
                        if (valencySlot.ValencyType is MotammemQeidi)
                        {
                            var motammemQeidi = (MotammemQeidi)valencySlot.ValencyType;
                            localTedad *= motammemQeidi.AdverbialCase.Count;
                        }
                        if (valencySlot.ValencyType is BandMotammemi)
                        {
                            var band = (BandMotammemi)valencySlot.ValencyType;
                            if (band.Agreement == "مطابقت+/-")
                                localTedad *= 2;
                            if (band.Eltezami == "التزامی+/-")
                                localTedad *= 2;
                        }
                        if (valencySlot.Obligatory == false)
                            localTedad++;
                        tedad *= localTedad;
                    }
                    verbValDic[verbPlusValency.Key].ValencyFrameList[counter].NumOfDifferentFrames = tedad;
                    counter++;
                    sumTedad += tedad;
                }
                verbValDic[verbPlusValency.Key].NumOfBaseStructures = sumTedad;
                if (sumTedad > maxTedad)
                    maxTedad = sumTedad;
            }
            foreach (var verbPlusValency in verbValDic)
            {
                int sumTedad = verbValDic[verbPlusValency.Key].NumOfBaseStructures;
                var structures = new BaseStructure[sumTedad];
                for (int i = 0; i < sumTedad; i++)
                {
                    structures[i] = new BaseStructure();
                }

            }
            var dic = new Dictionary<string, List<string>>();
            var basicBaseStructure = new BaseStructure();
            foreach (var verbPlusValency in verbValDic)
            {
                if (!BaseStrucDic.ContainsKey(verbPlusValency.Key))
                    BaseStrucDic.Add(verbPlusValency.Key, new List<BaseStructure>());
                var baseStructureList = new List<BaseStructure>();
                baseStructureList.Add(basicBaseStructure.Clone());

                foreach (ValencyFrame valencyFrame in verbPlusValency.Value.ValencyFrameList)
                {
                    foreach (ValencySlot valencySlot in valencyFrame.ValencyList)
                    {
                        var newTempList = new List<BaseStructure>();
                        if (!valencySlot.Obligatory)
                        {
                            newTempList.AddRange(baseStructureList.Select(baseStructure => baseStructure.Clone()));
                        }
                        if (valencySlot.ValencyType is Fael)
                        {

                            foreach (BaseStructure baseStructure in baseStructureList)
                            {
                                var newBaseStruct1 = baseStructure.Clone();
                                newBaseStruct1.HasSubject = true;
                                newTempList.Add(newBaseStruct1);
                            }
                        }
                        else if (valencySlot.ValencyType is MafoolNeshanehEzafi)
                        {
                            foreach (BaseStructure baseStructure in baseStructureList)
                            {
                                var newBaseStruct1 = baseStructure.Clone();
                                newBaseStruct1.HasEzafehObject = true;
                                newTempList.Add(newBaseStruct1);
                            }
                        }
                        else if (valencySlot.ValencyType is MafoolDovvom)
                        {
                            foreach (BaseStructure baseStructure in baseStructureList)
                            {
                                var newBaseStruct1 = baseStructure.Clone();
                                newBaseStruct1.HasSecondObject = true;
                                newTempList.Add(newBaseStruct1);
                            }
                        }
                        else if (valencySlot.ValencyType is Mosnad)
                        {
                            foreach (BaseStructure baseStructure in baseStructureList)
                            {
                                var newBaseStruct1 = baseStructure.Clone();
                                newBaseStruct1.HasMosnad = true;
                                newTempList.Add(newBaseStruct1);
                            }
                        }
                        else if (valencySlot.ValencyType is Tameez)
                        {
                            foreach (BaseStructure baseStructure in baseStructureList)
                            {
                                var newBaseStruct1 = baseStructure.Clone();
                                newBaseStruct1.HasTammeez = true;
                                newTempList.Add(newBaseStruct1);
                            }
                        }
                        else if (valencySlot.ValencyType is Mafool)
                        {
                            var maf = (Mafool)valencySlot.ValencyType;
                            if (maf.RaState == "را+/-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var newBaseStruct1 = baseStructure.Clone();
                                    newBaseStruct1.HasObject = true;
                                    newBaseStruct1.HasRa = true;
                                    var newBaseStruct2 = baseStructure.Clone();
                                    newBaseStruct2.HasObject = true;
                                    newBaseStruct2.HasRa = false;
                                    newTempList.Add(newBaseStruct1);
                                    newTempList.Add(newBaseStruct2);
                                }
                            }
                            else if (maf.RaState == "را+")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var newBaseStruct1 = baseStructure.Clone();
                                    newBaseStruct1.HasObject = true;
                                    newBaseStruct1.HasRa = true;
                                    newTempList.Add(newBaseStruct1);
                                }
                            }
                            else
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var newBaseStruct2 = baseStructure.Clone();
                                    newBaseStruct2.HasObject = true;
                                    newBaseStruct2.HasRa = false;
                                    newTempList.Add(newBaseStruct2);
                                }
                            }
                        }

                        else if (valencySlot.ValencyType is MafoolHarfeEzafeh)
                        {
                            var mafh = (MafoolHarfeEzafeh)valencySlot.ValencyType;
                            foreach (string preposition in mafh.PrepositionList)
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var newBaseStruct = baseStructure.Clone();
                                    if (newBaseStruct.HasPrepositionalObject1)
                                    {
                                        newBaseStruct.HasPrepositionalObject2 = true;
                                        newBaseStruct.PrepositionalObjectPreposition2 = preposition;
                                    }
                                    else
                                    {
                                        newBaseStruct.HasPrepositionalObject1 = true;
                                        newBaseStruct.PrepositionalObjectPreposition1 = preposition;
                                    }
                                    newTempList.Add(newBaseStruct);
                                }
                            }
                        }

                        else if (valencySlot.ValencyType is MotammemQeidi)
                        {
                            var motammemQeidi = (MotammemQeidi)valencySlot.ValencyType;
                            foreach (string advCase in motammemQeidi.AdverbialCase)
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var newBaseStruct = baseStructure.Clone();
                                    newBaseStruct.HasMoq = true;
                                    newBaseStruct.MoqType = advCase;
                                    newTempList.Add(newBaseStruct);
                                }
                            }
                        }

                        else if (valencySlot.ValencyType is BandMotammemi)
                        {
                            var band = (BandMotammemi)valencySlot.ValencyType;
                            if (band.Agreement == "مطابقت+/-" && band.Eltezami == "التزامی+/-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                    var baseStruct2 = baseStruct1.Clone();
                                    baseStruct2.HasBandMotemmemiAgreement = false;
                                    newTempList.Add(baseStruct2);
                                    var baseStruct3 = baseStruct1.Clone();
                                    baseStruct3.HasBandMotemmemiEltezami = false;
                                    newTempList.Add(baseStruct3);
                                    var baseStruct4 = baseStruct1.Clone();
                                    baseStruct4.HasBandMotemmemiEltezami = false;
                                    baseStruct4.HasBandMotemmemiAgreement = false;
                                    newTempList.Add(baseStruct4);
                                }
                            }
                            else if (band.Agreement == "مطابقت+/-" && band.Eltezami == "التزامی+")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                    var baseStruct2 = baseStruct1.Clone();
                                    baseStruct2.HasBandMotemmemiAgreement = false;
                                    newTempList.Add(baseStruct2);
                                }
                            }
                            else if (band.Agreement == "مطابقت+/-" && band.Eltezami == "التزامی-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = false;
                                    newTempList.Add(baseStruct1);
                                    var baseStruct2 = baseStruct1.Clone();
                                    baseStruct2.HasBandMotemmemiAgreement = false;
                                    newTempList.Add(baseStruct2);
                                }
                            }
                            else if (band.Agreement == "مطابقت+" && band.Eltezami == "التزامی+/-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                    var baseStruct3 = baseStruct1.Clone();
                                    baseStruct3.HasBandMotemmemiEltezami = false;
                                    newTempList.Add(baseStruct3);
                                }
                            }
                            else if (band.Agreement == "مطابقت+" && band.Eltezami == "التزامی+")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                }
                            }
                            else if (band.Agreement == "مطابقت+" && band.Eltezami == "التزامی-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = true;
                                    baseStruct1.HasBandMotemmemiEltezami = false;
                                    newTempList.Add(baseStruct1);
                                }
                            }
                            else if (band.Agreement == "مطابقت-" && band.Eltezami == "التزامی+/-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = false;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                    var baseStruct3 = baseStruct1.Clone();
                                    baseStruct3.HasBandMotemmemiEltezami = false;
                                    newTempList.Add(baseStruct3);
                                }
                            }
                            else if (band.Agreement == "مطابقت-" && band.Eltezami == "التزامی+")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct1 = baseStructure.Clone();
                                    baseStruct1.HasBandMotammemi = true;
                                    baseStruct1.HasBandMotemmemiAgreement = false;
                                    baseStruct1.HasBandMotemmemiEltezami = true;
                                    newTempList.Add(baseStruct1);
                                }
                            }
                            else if (band.Agreement == "مطابقت-" && band.Eltezami == "التزامی-")
                            {
                                foreach (BaseStructure baseStructure in baseStructureList)
                                {
                                    var baseStruct4 = baseStructure.Clone();
                                    baseStruct4.HasBandMotemmemiEltezami = false;
                                    baseStruct4.HasBandMotemmemiAgreement = false;
                                    newTempList.Add(baseStruct4);
                                }
                            }
                        }
                        if (newTempList.Count > 0)
                            baseStructureList = newTempList;
                    }
                }
                foreach (BaseStructure baseStructure in baseStructureList)
                {
                    if (!BaseStrucDic[verbPlusValency.Key].Contains(baseStructure))
                        BaseStrucDic[verbPlusValency.Key].Add(baseStructure);
                }
            }
        }

		internal static string GetVerbString(ref VerbInflector.VerbBasedSentence vbs, VerbInflector.VerbInSentence verbInSentence)
		{
			var verbBuilder = new StringBuilder();
			verbBuilder.Append(vbs.SentenceTokens[verbInSentence.LightVerbIndex].Lemma);
			if (verbInSentence.NonVerbalElementIndex == -1)
				verbBuilder.Append("#-");
			else
				verbBuilder.Append("#" + vbs.SentenceTokens[verbInSentence.NonVerbalElementIndex].Lemma);
			if (verbInSentence.VerbalPrepositionIndex == -1)
				verbBuilder.Append("#-");
			else
				verbBuilder.Append("#" + vbs.SentenceTokens[verbInSentence.VerbalPrepositionIndex].WordForm);

			return verbBuilder.ToString();
		}
	}
}
