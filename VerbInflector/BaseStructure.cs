using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VerbInflector;

namespace SentenceRecognizer
{
	public class BaseStructure : IComparable
	{
		/// <summary>
		/// shows if the base structure needs prepositional object
		/// </summary>
		public bool HasPrepositionalObject1 { set; get; }

		/// <summary>
		/// shows if the base structure needs the second prepositional object
		/// </summary>
		public bool HasPrepositionalObject2 { set; get; }

		/// <summary>
		/// refers to the type of preposition in the prepositional object
		/// </summary>
		public string PrepositionalObjectPreposition1 { set; get; }

		/// <summary>
		/// refers to the type of preposition in the second prepositional object
		/// </summary>
		public string PrepositionalObjectPreposition2 { set; get; }

		/// <summary>
		/// shows if the base structure needs object
		/// </summary>
		public bool HasObject { set; get; }

		/// <summary>
		/// shows if the object needs an postposition ("را")
		/// </summary>
		public bool HasRa { set; get; }

		/// <summary>
		/// shows if the base structure needs subject
		/// </summary>
		public bool HasSubject { set; get; }

		/// <summary>
		/// shows if the base structure needs Mottammem Qeydi ("متمم قیدی")
		/// </summary>
		public bool HasMoq { set; get; }

		/// <summary>
		/// refers to the type of Mottammem Qeidi
		/// </summary>
		public string MoqType { set; get; }

		/// <summary>
		/// shows if the base structure needs Tameez ("تمییز")
		/// </summary>
		public bool HasTammeez { set; get; }

		/// <summary>
		/// shows if the base structure needs Ezafeh Object ("مفعول نشانۀ اضافه‌ای")
		/// </summary>
		public bool HasEzafehObject { set; get; }

		/// <summary>
		/// shows if the base structure needs Mosnad ("مسند")
		/// </summary>
		public bool HasMosnad { set; get; }

		/// <summary>
		/// shows if the base structure needs Band Motammemi ("بند متممی")
		/// </summary>
		public bool HasBandMotammemi { set; get; }

		/// <summary>
		/// refers to the type of agreement between the central verb and band motammemi
		/// </summary>
		public bool HasBandMotemmemiAgreement { set; get; }

		/// <summary>
		/// refers to the type of band (Eltezami vs. all type) in band motammemi
		/// </summary>
		public bool HasBandMotemmemiEltezami { set; get; }

		/// <summary>
		/// shows if the base structure needs the second object
		/// </summary>
		public bool HasSecondObject { set; get; }

		/// <summary>
		/// by default a base structure only has subject
		/// </summary>
		public BaseStructure()
		{
			HasBandMotammemi = false;
			HasBandMotemmemiAgreement = false;
			HasBandMotemmemiEltezami = false;
			HasEzafehObject = false;
			HasMoq = false;
			HasMosnad = false;
			HasObject = false;
			HasObject = false;
			HasPrepositionalObject1 = false;
			HasPrepositionalObject2 = false;
			HasSubject = false;
			HasTammeez = false;
			HasSecondObject = false;
		}

		/// <summary>
		/// change motammem qeidi condition (default: no motammem qeidi)
		/// </summary>
		/// <param name="hasBand">HasBandMotammemi</param>
		/// <param name="agreement">HasBandMotemmemiAgreement</param>
		/// <param name="hasEltezami">HasBandMotemmemiEltezami</param>
		public void ChangeBandMotammeiState(bool hasBand, bool agreement, bool hasEltezami)
		{
			HasBandMotammemi = hasBand;
			HasBandMotemmemiAgreement = agreement;
			HasBandMotemmemiEltezami = hasEltezami;
		}

		/// <summary>
		///changes the subject Structure in BS
		/// </summary>
		/// <param name="hasSub">HasSubject</param>
		public void ChangeSubjectState(bool hasSub)
		{
			HasSubject = hasSub;
		}

		/// <summary>
		/// changes the direct object Structure state in BS
		/// </summary>
		/// <param name="hasObj">HasObject</param>
		/// <param name="hasRa">HasRa</param>
		public void ChangeObjectState(bool hasObj, bool hasRa)
		{
			HasObject = hasObj;
			HasRa = hasRa;
		}

		/// <summary>
		/// changes the first prepositional object Structure in BS
		/// </summary>
		/// <param name="hasPrep">HasPrepositionalObject1</param>
		/// <param name="p">PrepositionalObjectPreposition1</param>
		public void ChangeFirstPrepositionalObjectState(bool hasPrep, string p)
		{
			HasPrepositionalObject1 = hasPrep;
			PrepositionalObjectPreposition1 = p;
		}

		/// <summary>
		/// changes the second prepositional object Structure in BS
		/// </summary>
		/// <param name="hasPrep">HasPrepositionalObject2</param>
		/// <param name="p">PrepositionalObjectPreposition2</param>
		public void ChangeSecondPrepositionalObjectState(bool hasPrep, string p)
		{
			HasPrepositionalObject2 = hasPrep;
			PrepositionalObjectPreposition2 = p;
		}

		/// <summary>
		/// changes the second object Structure state in BS
		/// </summary>
		/// <param name="has2Obj">HasSecondObject</param>
		public void ChangeSecondObjectState(bool has2Obj)
		{
			HasSecondObject = has2Obj;
		}

		/// <summary>
		/// changes tameez state in BS
		/// </summary>
		/// <param name="hasTameez">HasTammeez</param>
		public void ChangeTameezState(bool hasTameez)
		{
			HasTammeez = hasTameez;
		}

		/// <summary>
		/// changes Mosnad state in BS
		/// </summary>
		/// <param name="hasMos">HasTammeez</param>
		public void ChangeMosnadState(bool hasMos)
		{
			HasMosnad = hasMos;
		}

		/// <summary>
		/// changes the moq state in the BS
		/// </summary>
		/// <param name="hasMoq">HasMoq</param>
		/// <param name="moqType">MoqType</param>
		public void ChangeMoqState(bool hasMoq, string moqType)
		{
			HasMoq = hasMoq;
			MoqType = moqType;
		}

		/// <summary>
		/// changes Ezafeh Object state in the BS
		/// </summary>
		/// <param name="hasEzafehObj">HasEzafehObject</param>
		public void ChangeEzafehObjectType(bool hasEzafehObj)
		{
			HasEzafehObject = hasEzafehObj;
		}


		public BaseStructure Clone()
		{
			var newStruct = new BaseStructure();
			newStruct.HasBandMotammemi = HasBandMotammemi;
			newStruct.HasBandMotemmemiAgreement = HasBandMotemmemiAgreement;
			newStruct.HasBandMotemmemiEltezami = HasBandMotemmemiEltezami;
			newStruct.HasEzafehObject = HasEzafehObject;
			newStruct.HasMoq = HasMoq;
			newStruct.HasMosnad = HasMosnad;
			newStruct.HasObject = HasObject;
			newStruct.HasPrepositionalObject1 = HasPrepositionalObject1;
			newStruct.HasPrepositionalObject2 = HasPrepositionalObject2;
			newStruct.HasRa = HasRa;
			newStruct.HasSecondObject = HasSecondObject;
			newStruct.HasSubject = HasSubject;
			newStruct.HasTammeez = HasTammeez;
			newStruct.PrepositionalObjectPreposition1 = PrepositionalObjectPreposition1;
			newStruct.PrepositionalObjectPreposition2 = PrepositionalObjectPreposition2;
			newStruct.MoqType = MoqType;
			return newStruct;
		}

		public bool Satisfy(VerbBasedSentence verbBasedSentence, VerbInSentence verbInSentence)
		{
			bool satisfy = false;
			if (HasPrepositionalObject1)
			{
				satisfy = false;
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if ((dependencyBasedToken.CPOSTag == "P" || dependencyBasedToken.CPOSTag == "POSTP") && dependencyBasedToken.DependencyRelation != "VPRT")
					{
						if (dependencyBasedToken.WordForm == PrepositionalObjectPreposition1)
						{
							satisfy = true;
							break;
						}
					}
				}
				if (!satisfy)
					return false;
			}
			if (HasPrepositionalObject2)
			{
				satisfy = false;
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if ((dependencyBasedToken.CPOSTag == "P" || dependencyBasedToken.CPOSTag == "POSTP") && dependencyBasedToken.DependencyRelation != "VPRT")
					{
						if (dependencyBasedToken.WordForm == PrepositionalObjectPreposition2)
						{
							satisfy = true;
							break;
						}
					}
				}
				if (!satisfy)
					return false;
			}

			if (HasObject && HasRa)
			{
				satisfy = false;
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if (dependencyBasedToken.WordForm == "را" &&
						dependencyBasedToken.Position < verbInSentence.LightVerbIndex)
					{
						satisfy = true;
						break;
					}
				}
				if (!satisfy)
					return false;
			}

			if (HasBandMotammemi)
			{
				satisfy = false;
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if (dependencyBasedToken.CPOSTag == "V" &&
						dependencyBasedToken.Position > verbInSentence.LightVerbIndex)
					{
						if (HasBandMotemmemiAgreement)
						{
							if (dependencyBasedToken.MorphoSyntacticFeats.Person ==
								verbBasedSentence.SentenceTokens[verbInSentence.LightVerbIndex].MorphoSyntacticFeats.
									Person)
							{
								if (HasBandMotemmemiEltezami)
								{
									if (dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
										TenseFormationType.HAAL_ELTEZAMI ||
										dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
										TenseFormationType.GOZASHTEH_ESTEMRAARI)
									{
										satisfy = true;
										break;
									}
								}
							}
						}
						else
						{
							if (HasBandMotemmemiEltezami)
							{
								if (dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
									TenseFormationType.HAAL_ELTEZAMI ||
									dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
									TenseFormationType.GOZASHTEH_ESTEMRAARI)
								{
									satisfy = true;
									break;
								}
							}
							else
							{
								satisfy = true;
								break;
							}
						}
					}
				}
				if (!satisfy)
					return false;
			}
			return true;
		}

		public void FitIntoBaseStructure(ref VerbBasedSentence verbBasedSentence, VerbInSentence verbInSentence)
		{
			if (HasPrepositionalObject1)
			{
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if ((dependencyBasedToken.CPOSTag == "P" || dependencyBasedToken.CPOSTag == "POSTP") && dependencyBasedToken.DependencyRelation != "VPRT")
					{
						if (dependencyBasedToken.WordForm == PrepositionalObjectPreposition1)
						{
							if (verbInSentence.NonVerbalElementIndex != -1 && verbInSentence.VerbalPrepositionIndex == -1)
							{
								dependencyBasedToken.HeadNumber = verbInSentence.NonVerbalElementIndex;
								dependencyBasedToken.DependencyRelation = "VPP";
							}
							else
							{
								dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
								dependencyBasedToken.DependencyRelation = "VPP";
							}
							if (dependencyBasedToken.CPOSTag == "P")
							{
								for (int i = dependencyBasedToken.Position; i < verbBasedSentence.SentenceTokens.Count; i++)
								{
									if (verbBasedSentence.SentenceTokens[i].CPOSTag == "PR" || verbBasedSentence.SentenceTokens[i].CPOSTag == "N")
									{
										verbBasedSentence.SentenceTokens[i].DependencyRelation = "POSDEP";
										verbBasedSentence.SentenceTokens[i].HeadNumber = dependencyBasedToken.Position;
										break;
									}
								}
							}
							else
							{
								for (int i = dependencyBasedToken.Position - 2; i >= 0; i--)
								{
									if ((verbBasedSentence.SentenceTokens[i].CPOSTag == "PR" || verbBasedSentence.SentenceTokens[i].CPOSTag == "N") && (i == 0 || verbBasedSentence.SentenceTokens[i - 1].CPOSTag != "N"))
									{
										verbBasedSentence.SentenceTokens[i].DependencyRelation = "PREDEP";
										verbBasedSentence.SentenceTokens[i].HeadNumber = dependencyBasedToken.Position;
										break;
									}
								}
							}
							break;
						}
					}
				}
			}
			if (HasPrepositionalObject2)
			{
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if ((dependencyBasedToken.CPOSTag == "P" || dependencyBasedToken.CPOSTag == "POSTP") && dependencyBasedToken.DependencyRelation != "VPRT")
					{
						if (dependencyBasedToken.WordForm == PrepositionalObjectPreposition2)
						{
							if (verbInSentence.NonVerbalElementIndex != -1 && verbInSentence.VerbalPrepositionIndex == -1)
							{
								dependencyBasedToken.HeadNumber = verbInSentence.NonVerbalElementIndex;
								dependencyBasedToken.DependencyRelation = "VPP";
							}
							else
							{
								dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
								dependencyBasedToken.DependencyRelation = "VPP";
							}
							if (dependencyBasedToken.CPOSTag == "P")
							{
								for (int i = dependencyBasedToken.Position; i < verbBasedSentence.SentenceTokens.Count; i++)
								{
									if (verbBasedSentence.SentenceTokens[i].CPOSTag == "PR" || verbBasedSentence.SentenceTokens[i].CPOSTag == "N")
									{
										verbBasedSentence.SentenceTokens[i].DependencyRelation = "POSDEP";
										verbBasedSentence.SentenceTokens[i].HeadNumber = dependencyBasedToken.Position;
										break;
									}
								}
							}
							else
							{
								for (int i = dependencyBasedToken.Position - 2; i >= 0; i--)
								{
									if ((verbBasedSentence.SentenceTokens[i].CPOSTag == "PR" || verbBasedSentence.SentenceTokens[i].CPOSTag == "N") && (i == 0 || verbBasedSentence.SentenceTokens[i - 1].CPOSTag != "N"))
									{
										verbBasedSentence.SentenceTokens[i].DependencyRelation = "PREDEP";
										verbBasedSentence.SentenceTokens[i].HeadNumber = dependencyBasedToken.Position;
										break;
									}
								}
							}
							break;
						}
					}
				}
			}

			if (HasObject && HasRa)
			{
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if (dependencyBasedToken.WordForm == "را" &&
						dependencyBasedToken.Position < verbInSentence.LightVerbIndex)
					{
						dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
						dependencyBasedToken.DependencyRelation = "OBJ";
						for (int i = dependencyBasedToken.Position - 2; i >= 0; i--)
						{
							if ((verbBasedSentence.SentenceTokens[i].CPOSTag == "PR" || verbBasedSentence.SentenceTokens[i].CPOSTag == "N") && (i == 0 || verbBasedSentence.SentenceTokens[i - 1].CPOSTag != "N"))
							{
								verbBasedSentence.SentenceTokens[i].DependencyRelation = "PREDEP";
								verbBasedSentence.SentenceTokens[i].HeadNumber = dependencyBasedToken.Position;
								break;
							}
						}
						break;
					}
				}
			}

			if (HasBandMotammemi)
			{
				foreach (var dependencyBasedToken in verbBasedSentence.SentenceTokens)
				{
					if (dependencyBasedToken.CPOSTag == "V" &&
						dependencyBasedToken.Position > verbInSentence.LightVerbIndex)
					{
						bool hasConnector = false;
						if (HasBandMotemmemiAgreement)
						{
							if (dependencyBasedToken.MorphoSyntacticFeats.Person ==
								verbBasedSentence.SentenceTokens[verbInSentence.LightVerbIndex].MorphoSyntacticFeats.
									Person)
							{
								if (HasBandMotemmemiEltezami)
								{
									if (dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
										TenseFormationType.HAAL_ELTEZAMI ||
										dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
										TenseFormationType.GOZASHTEH_ESTEMRAARI)
									{

										for (int i = verbInSentence.LightVerbIndex - 1; i < dependencyBasedToken.Position && i > 0; i++)
										{
											if (verbBasedSentence.SentenceTokens[i].WordForm == "که" || verbBasedSentence.SentenceTokens[i].WordForm == "تا")
											{
												hasConnector = true;
												dependencyBasedToken.HeadNumber =
													verbBasedSentence.SentenceTokens[i].Position;
												dependencyBasedToken.DependencyRelation = "PRD";
												verbBasedSentence.SentenceTokens[i].HeadNumber =
													verbInSentence.LightVerbIndex;
												verbBasedSentence.SentenceTokens[i].DependencyRelation = "VCL";
												break;
											}
											if (!hasConnector)
											{
												dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
												dependencyBasedToken.DependencyRelation = "VCL";
											}
										}
										break;
									}
								}
							}
						}
						else
						{
							if (HasBandMotemmemiEltezami)
							{
								if (dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
									TenseFormationType.HAAL_ELTEZAMI ||
									dependencyBasedToken.MorphoSyntacticFeats.TenseMoodAspect ==
									TenseFormationType.GOZASHTEH_ESTEMRAARI)
								{
									for (int i = verbInSentence.LightVerbIndex - 1; i < dependencyBasedToken.Position && i > 0; i++)
									{
										if (verbBasedSentence.SentenceTokens[i].WordForm == "که" || verbBasedSentence.SentenceTokens[i].WordForm == "تا")
										{
											hasConnector = true;
											dependencyBasedToken.HeadNumber =
												verbBasedSentence.SentenceTokens[i].Position;
											dependencyBasedToken.DependencyRelation = "PRD";
											verbBasedSentence.SentenceTokens[i].HeadNumber =
												verbInSentence.LightVerbIndex;
											verbBasedSentence.SentenceTokens[i].DependencyRelation = "VCL";
											break;
										}
										if (!hasConnector)
										{
											dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
											dependencyBasedToken.DependencyRelation = "VCL";
										}
									}
									break;
								}
							}
							else
							{
								for (int i = verbInSentence.LightVerbIndex - 1; i < dependencyBasedToken.Position && i > 0; i++)
								{
									if (verbBasedSentence.SentenceTokens[i].WordForm == "که" || verbBasedSentence.SentenceTokens[i].WordForm == "تا")
									{
										hasConnector = true;
										dependencyBasedToken.HeadNumber =
											verbBasedSentence.SentenceTokens[i].Position;
										dependencyBasedToken.DependencyRelation = "PRD";
										verbBasedSentence.SentenceTokens[i].HeadNumber =
											verbInSentence.LightVerbIndex;
										verbBasedSentence.SentenceTokens[i].DependencyRelation = "VCL";
										break;
									}
									if (!hasConnector)
									{
										dependencyBasedToken.HeadNumber = verbInSentence.LightVerbIndex;
										dependencyBasedToken.DependencyRelation = "VCL";
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		public int CompareTo(object obj)
		{
			var newObj = (BaseStructure)obj;
			if (newObj.Equals(obj))
				return 0;
			return CompareTo(newObj);
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			var newObj = (BaseStructure)obj;
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			if (HasBandMotammemi == newObj.HasBandMotammemi &&
				HasBandMotemmemiAgreement == newObj.HasBandMotemmemiAgreement &&
				HasBandMotemmemiEltezami == newObj.HasBandMotemmemiEltezami && HasEzafehObject == newObj.HasEzafehObject &&
				HasMoq == newObj.HasMoq && HasMosnad == newObj.HasMosnad && HasObject == newObj.HasObject &&
				HasPrepositionalObject1 == newObj.HasPrepositionalObject1 &&
				HasPrepositionalObject2 == newObj.HasPrepositionalObject2 && HasRa == newObj.HasRa &&
				HasSecondObject == newObj.HasSecondObject && HasSubject == newObj.HasSubject &&
				HasTammeez == newObj.HasTammeez &&
				PrepositionalObjectPreposition1 == newObj.PrepositionalObjectPreposition1 &&
				PrepositionalObjectPreposition2 == newObj.PrepositionalObjectPreposition2 && MoqType == newObj.MoqType)
			{
				return true;
			}
			return false;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return HasBandMotammemi.GetHashCode() +
				   HasBandMotemmemiAgreement.GetHashCode() +
				   HasBandMotemmemiEltezami.GetHashCode() + HasEzafehObject.GetHashCode() +
				   HasMoq.GetHashCode() + HasMosnad.GetHashCode() + HasObject.GetHashCode() +
				   HasPrepositionalObject1.GetHashCode() +
				   HasPrepositionalObject2.GetHashCode() + HasRa.GetHashCode() +
				   HasSecondObject.GetHashCode() + HasSubject.GetHashCode() +
				   HasTammeez.GetHashCode() +
				   PrepositionalObjectPreposition1.GetHashCode() +
				   PrepositionalObjectPreposition2.GetHashCode() + MoqType.GetHashCode();
		}
	}
}
