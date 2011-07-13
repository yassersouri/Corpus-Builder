using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VerbInflector;
using SentenceRecognizer;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace VerbInflector
{
	class Program
	{
		static void Main(string[] args)
		{
			string verbDicPath = "../../VerbList.txt";

			string sourceDir = "D:\\sample\\corpus_tagged\\";
			string destinationDir = sourceDir + "again\\";
			string file = "1088245.txt";

			string sourceFile = sourceDir + file;
			string destinationFile = destinationDir + file;
			
			//load the corpus
			//this line takes time
			ValencyDicManager.RefreshVerbList("../../VerbList.txt", "../../valency list.txt");

			//VerbInflector_OneFile(sourceFile, destinationFile, verbDicPath);
			VerbInflector_All(sourceDir, destinationDir, verbDicPath);
		}

		public static void VerbInflector_OneFile(string sourceFile, string destinationFile, string verbDicPath){
			Article currentArticle = ArticleUtils.getArticle(sourceFile);

			Article newArticle = generateNewArticle(currentArticle, verbDicPath);
			newArticle.setArticleNumber(currentArticle.getArticleNumber());

			ArticleUtils.putArticle(newArticle, destinationFile);

			System.Console.WriteLine(newArticle.getArticleNumber());
		}

		public static void VerbInflector_All(string sourceDirectory, string destinationDirectory, string verbDicPath)
		{
			string[] files = Directory.GetFiles(sourceDirectory);
			Directory.CreateDirectory(destinationDirectory);

			string destinationFile;
			string sourceFile;
			int lastIndex;
			string fileName;
			int len;

			for (int i = 0; i < files.Length; i++)
			{
				sourceFile = files[i];

				lastIndex = sourceFile.LastIndexOf('\\');
				len = sourceFile.Length;
				fileName = sourceFile.Substring(lastIndex + 1, len - lastIndex - 1);
				destinationFile = destinationDirectory + fileName;

				VerbInflector_OneFile(sourceFile, destinationFile, verbDicPath);
			}
		}

		private static Article generateNewArticle(Article currentArticle, string verbDicPath)
		{
			Article newArticle = new Article();

			Sentence currentSentence = null;
			Sentence newSentence = null;
			String[] currentLexemes = null;
			String[] currentPOSTags = null;
			String[] currentLemmas = null;
			MorphoSyntacticFeatures[] currentFeatures = null;

			//generating new Sentence
			for(int sentence_index = 0; sentence_index < currentArticle.getSentences().Length; sentence_index++) //for each sentence in this article.
			{
				//initialize the new sentence
				newSentence = new Sentence();

				//load the current sentence
				currentSentence = currentArticle.getSentence(sentence_index);

				//load info about that sentence
				currentLexemes = currentSentence.getLexemes();
				currentPOSTags = currentSentence.getPOSTags();
				currentLemmas = currentSentence.getLemmas();
				currentFeatures = currentSentence.getFeatures();

				
				
				VerbBasedSentence currentSentenceVBS = SentenceAnalyzer.MakeVerbBasedSentence(currentLexemes, currentPOSTags, currentLemmas, currentFeatures, verbDicPath);
				List<DependencyBasedToken> list = currentSentenceVBS.SentenceTokens;
				Random randomNumberGenerator = new Random();
				Dictionary<VerbInSentence, BaseStructure> pickedBasedStructures = new Dictionary<VerbInSentence,BaseStructure>();

				//for each verb in sentence
				foreach(var currentVerbInSentence in currentSentenceVBS.VerbsInSentence)
				{
					List<BaseStructure> satisfiedBaseStructuresOfCurrentVerb = new List<BaseStructure>();
					//special string representation of the verb
					String currentVerbString = ValencyDicManager.GetVerbString(ref currentSentenceVBS, currentVerbInSentence);
					if (ValencyDicManager.BaseStrucDic.ContainsKey(currentVerbString))
					{
						List<BaseStructure> baseStructuresForTheCurrentVerb = ValencyDicManager.BaseStrucDic[currentVerbString];
						foreach (var currentBaseStructure in baseStructuresForTheCurrentVerb)
						{
							if(currentBaseStructure.Satisfy(currentSentenceVBS, currentVerbInSentence))
							{
								satisfiedBaseStructuresOfCurrentVerb.Add(currentBaseStructure);
							}
						}
					}

					List<BaseStructure> candidateBaseStructuresOfCurrentVerb;

					#region select one base structure of this verb

					bool finishedChoosingPickedBaseStructure = false;

					#region HasPrepositionalObject2
					candidateBaseStructuresOfCurrentVerb = new List<BaseStructure>();

					if(!finishedChoosingPickedBaseStructure)
					{
						foreach (var currentSatisfiedBaseStructure in satisfiedBaseStructuresOfCurrentVerb)
						{
							if(currentSatisfiedBaseStructure.HasPrepositionalObject2)
								candidateBaseStructuresOfCurrentVerb.Add(currentSatisfiedBaseStructure);
						}
						if(candidateBaseStructuresOfCurrentVerb.Count > 1)
						{
							int randomIndex = randomNumberGenerator.Next(0, candidateBaseStructuresOfCurrentVerb.Count);
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[randomIndex]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if(candidateBaseStructuresOfCurrentVerb.Count == 1)
						{
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[0]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if(candidateBaseStructuresOfCurrentVerb.Count == 0)
						{
							finishedChoosingPickedBaseStructure = false;
						}
						else
						{
							throw new Exception("Error in picking BaseStructures for Verbs");
						}
					}
					#endregion

					#region HasPrepositionalObject1
					candidateBaseStructuresOfCurrentVerb = new List<BaseStructure>();

					if (!finishedChoosingPickedBaseStructure)
					{
						foreach (var currentSatisfiedBaseStructure in satisfiedBaseStructuresOfCurrentVerb)
						{
							if (currentSatisfiedBaseStructure.HasPrepositionalObject1)
								candidateBaseStructuresOfCurrentVerb.Add(currentSatisfiedBaseStructure);
						}
						if (candidateBaseStructuresOfCurrentVerb.Count > 1)
						{
							int randomIndex = randomNumberGenerator.Next(0, candidateBaseStructuresOfCurrentVerb.Count);
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[randomIndex]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 1)
						{
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[0]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 0)
						{
							finishedChoosingPickedBaseStructure = false;
						}
						else
						{
							throw new Exception("Error in picking BaseStructures for Verbs");
						}
					}
					#endregion

					#region HasRa
					candidateBaseStructuresOfCurrentVerb = new List<BaseStructure>();

					if (!finishedChoosingPickedBaseStructure)
					{
						foreach (var currentSatisfiedBaseStructure in satisfiedBaseStructuresOfCurrentVerb)
						{
							if (currentSatisfiedBaseStructure.HasRa)
								candidateBaseStructuresOfCurrentVerb.Add(currentSatisfiedBaseStructure);
						}
						if (candidateBaseStructuresOfCurrentVerb.Count > 1)
						{
							int randomIndex = randomNumberGenerator.Next(0, candidateBaseStructuresOfCurrentVerb.Count);
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[randomIndex]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 1)
						{
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[0]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 0)
						{
							finishedChoosingPickedBaseStructure = false;
						}
						else
						{
							throw new Exception("Error in picking BaseStructures for Verbs");
						}
					}
					#endregion

					#region HasBandMotammemi
					candidateBaseStructuresOfCurrentVerb = new List<BaseStructure>();

					if (!finishedChoosingPickedBaseStructure)
					{
						foreach (var currentSatisfiedBaseStructure in satisfiedBaseStructuresOfCurrentVerb)
						{
							if (currentSatisfiedBaseStructure.HasBandMotammemi || currentSatisfiedBaseStructure.HasBandMotemmemiAgreement || currentSatisfiedBaseStructure.HasBandMotemmemiEltezami)
								candidateBaseStructuresOfCurrentVerb.Add(currentSatisfiedBaseStructure);
						}
						if (candidateBaseStructuresOfCurrentVerb.Count > 1)
						{
							int randomIndex = randomNumberGenerator.Next(0, candidateBaseStructuresOfCurrentVerb.Count);
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[randomIndex]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 1)
						{
							pickedBasedStructures.Add(currentVerbInSentence, candidateBaseStructuresOfCurrentVerb[0]);
							finishedChoosingPickedBaseStructure = true;
						}
						else if (candidateBaseStructuresOfCurrentVerb.Count == 0)
						{
							finishedChoosingPickedBaseStructure = false;
						}
						else
						{
							throw new Exception("Error in picking BaseStructures for Verbs");
						}
					}
					#endregion

					#region Choose Randomly

					int randomNumberIndex = randomNumberGenerator.Next(0, satisfiedBaseStructuresOfCurrentVerb.Count);
					pickedBasedStructures.Add(currentVerbInSentence, satisfiedBaseStructuresOfCurrentVerb[randomNumberIndex]);
					finishedChoosingPickedBaseStructure = true;

					#endregion

					#endregion

				}


				#region select one base structure for the whole sentence from pickedBaseStructures

				bool finishedChoosingBaseStructure = false;

				List<KeyValuePair<VerbInSentence, BaseStructure>> candidatesOfCurrentSentence;
				KeyValuePair<VerbInSentence, BaseStructure> SelectedKVP;

				#region choose HasPrepositionalObject2

				if(!finishedChoosingBaseStructure){
					candidatesOfCurrentSentence = new List<KeyValuePair<VerbInSentence,BaseStructure>>();

					foreach(var currentKeyValuePair in pickedBasedStructures)
					{
						if(currentKeyValuePair.Value.HasPrepositionalObject2)
							candidatesOfCurrentSentence.Add(currentKeyValuePair);
					}
					
					if(candidatesOfCurrentSentence.Count > 1)
					{
						//select the one which is at the end of the sentence
						int maxLightVerbIndex = 0;
						foreach(var currentKeyValuePair in candidatesOfCurrentSentence)
						{
							if(currentKeyValuePair.Key.LightVerbIndex >= maxLightVerbIndex){
								maxLightVerbIndex = currentKeyValuePair.Key.LightVerbIndex;
								SelectedKVP = currentKeyValuePair;
							}
						}
						finishedChoosingBaseStructure = true;
					}
					else if(candidatesOfCurrentSentence.Count == 1)
					{
						SelectedKVP = candidatesOfCurrentSentence[0];
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 0)
					{
						finishedChoosingBaseStructure = false;
					}
					else
					{
						throw new Exception("Error in picking BaseStructures for Verbs");
					}
				
				}
				#endregion

				#region choose HasPrepositionalObject1

				if (!finishedChoosingBaseStructure)
				{
					candidatesOfCurrentSentence = new List<KeyValuePair<VerbInSentence, BaseStructure>>();

					foreach (var currentKeyValuePair in pickedBasedStructures)
					{
						if (currentKeyValuePair.Value.HasPrepositionalObject1)
							candidatesOfCurrentSentence.Add(currentKeyValuePair);
					}

					if (candidatesOfCurrentSentence.Count > 1)
					{
						//select the one which is at the end of the sentence
						int maxLightVerbIndex = 0;
						foreach (var currentKeyValuePair in candidatesOfCurrentSentence)
						{
							if (currentKeyValuePair.Key.LightVerbIndex >= maxLightVerbIndex)
							{
								maxLightVerbIndex = currentKeyValuePair.Key.LightVerbIndex;
								SelectedKVP = currentKeyValuePair;
							}
						}
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 1)
					{
						SelectedKVP = candidatesOfCurrentSentence[0];
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 0)
					{
						finishedChoosingBaseStructure = false;
					}
					else
					{
						throw new Exception("Error in picking BaseStructures for Verbs");
					}

				}
				#endregion

				#region choose HasRa

				if (!finishedChoosingBaseStructure)
				{
					candidatesOfCurrentSentence = new List<KeyValuePair<VerbInSentence, BaseStructure>>();

					foreach (var currentKeyValuePair in pickedBasedStructures)
					{
						if (currentKeyValuePair.Value.HasRa)
							candidatesOfCurrentSentence.Add(currentKeyValuePair);
					}

					if (candidatesOfCurrentSentence.Count > 1)
					{
						//select the one which is at the end of the sentence
						int maxLightVerbIndex = 0;
						foreach (var currentKeyValuePair in candidatesOfCurrentSentence)
						{
							if (currentKeyValuePair.Key.LightVerbIndex >= maxLightVerbIndex)
							{
								maxLightVerbIndex = currentKeyValuePair.Key.LightVerbIndex;
								SelectedKVP = currentKeyValuePair;
							}
						}
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 1)
					{
						SelectedKVP = candidatesOfCurrentSentence[0];
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 0)
					{
						finishedChoosingBaseStructure = false;
					}
					else
					{
						throw new Exception("Error in picking BaseStructures for Verbs");
					}

				}
				#endregion

				#region choose HasBandMotammemi

				if (!finishedChoosingBaseStructure)
				{
					candidatesOfCurrentSentence = new List<KeyValuePair<VerbInSentence, BaseStructure>>();

					foreach (var currentKeyValuePair in pickedBasedStructures)
					{
						if (currentKeyValuePair.Value.HasBandMotammemi || currentKeyValuePair.Value.HasBandMotemmemiAgreement || currentKeyValuePair.Value.HasBandMotemmemiEltezami)
							candidatesOfCurrentSentence.Add(currentKeyValuePair);
					}

					if (candidatesOfCurrentSentence.Count > 1)
					{
						//select the one which is at the end of the sentence
						int maxLightVerbIndex = 0;
						foreach (var currentKeyValuePair in candidatesOfCurrentSentence)
						{
							if (currentKeyValuePair.Key.LightVerbIndex >= maxLightVerbIndex)
							{
								maxLightVerbIndex = currentKeyValuePair.Key.LightVerbIndex;
								SelectedKVP = currentKeyValuePair;
							}
						}
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 1)
					{
						SelectedKVP = candidatesOfCurrentSentence[0];
						finishedChoosingBaseStructure = true;
					}
					else if (candidatesOfCurrentSentence.Count == 0)
					{
						finishedChoosingBaseStructure = false;
					}
					else
					{
						throw new Exception("Error in picking BaseStructures for Verbs");
					}

				}
				#endregion

				#region choose the last verb

				if(!finishedChoosingBaseStructure)
				{
					int maxLightVerbIndex = 0;
					foreach(var currentKeyValuePair in pickedBasedStructures)
					{
						if(currentKeyValuePair.Key.LightVerbIndex >= maxLightVerbIndex)
						{
							SelectedKVP = currentKeyValuePair;
							maxLightVerbIndex = currentKeyValuePair.Key.LightVerbIndex;
						}
					}
					finishedChoosingBaseStructure = true;
				}

				#endregion

				#endregion

				//fitting one base structure
				SelectedKVP.Value.FitIntoBaseStructure(ref currentSentenceVBS, SelectedKVP.Key);
				

				//word index pointer in the current sentence
				int wordIndexInCurrentSentence = 0;
				
				Word currentWord = null;
				Word newWord = null;
				DependencyBasedToken currentDBT;

				#region upgrading the current sentence to a new sentence.
				for(int word_index = 0; word_index < list.Count() ; word_index++)
				{
					currentDBT = list[word_index];

					newWord = new Word();

					newWord.num = currentDBT.Position;
					newWord.lexeme = currentDBT.WordForm;
					if(currentDBT.Lemma != "_")
					{
						//if lemma is changed
						newWord.lemma = currentDBT.Lemma;
						newWord.cpos = currentDBT.CPOSTag;
						newWord.fpos = currentDBT.FPOSTag;

						if(currentDBT.MorphoSyntacticFeats.Person != ShakhsType.Shakhs_NONE)
							newWord.person = currentDBT.MorphoSyntacticFeats.Person.ToString();
						else
							newWord.person = "_";

						if(currentDBT.MorphoSyntacticFeats.Number != NumberType.INVALID)
							newWord.number = currentDBT.MorphoSyntacticFeats.Number.ToString();
						else
							newWord.number = "_";

						if(currentDBT.MorphoSyntacticFeats.TenseMoodAspect != TenseFormationType.TenseFormationType_NONE)
							newWord.tma = currentDBT.MorphoSyntacticFeats.TenseMoodAspect.ToString();
						else
							newWord.tma = "_";
					}
					else{
						//lemma is unchanged
						currentWord = currentSentence.getWord(wordIndexInCurrentSentence);

						newWord.lemma = currentWord.lemma;
						newWord.cpos = currentWord.cpos;
						newWord.fpos = currentWord.fpos;
						newWord.person = currentWord.person;
						newWord.number = currentWord.number;
						newWord.tma = "_";
					}

					//are changed any way
					newWord.parentId = currentDBT.HeadNumber;
					newWord.parentRelation = currentDBT.DependencyRelation;

					newSentence.addWord(newWord);
					wordIndexInCurrentSentence += currentDBT.TokenCount;
				}
				//sentence is now upgraded and more filled with information
				#endregion


				//adding the fitted base structure to the database as the main verb
				mongoSaveMainVerb();

				////////////////////////
				//// Counting the Verbs
				////    -----------

				List<VerbInSentence> verbsInSentence = currentSentenceVBS.VerbsInSentence;
				for(int verb_index = 0; verb_index < verbsInSentence.Count ; verb_index++)
				{
					string verbStringRepresentation = getVerbStringRepresentation(verbsInSentence[verb_index], newSentence);
					long article = currentArticle.getArticleNumber();
					//sentence_index is already set
					//verb_index is already set

					//if (verbStringRepresentation.Equals("شدند~_~_"))
					//{
					//    mongoCountVerb(verbStringRepresentation, article, sentence_index, verb_index);
					//}
					mongoCountVerb(verbStringRepresentation, article, sentence_index, verb_index);
				}

				////
				////////////////////////
				newArticle.addSentence(newSentence);
			}

			return newArticle;
		}

		private static void mongoSaveMainVerb()
		{
			throw new NotImplementedException();
		}

		private static string getVerbStringRepresentation(VerbInSentence currentVerbInSentence, Sentence currentSentence)
		{
			string representation = "";
			Word[] words = currentSentence.getWords();
			string LightVerbIndexLemma = words[currentVerbInSentence.LightVerbIndex].lemma;
			string NonVerbalElementLexeme = (currentVerbInSentence.NonVerbalElementIndex != -1) ? words[currentVerbInSentence.NonVerbalElementIndex].lexeme : "_";
			string VerbalPreposiotionLexeme = (currentVerbInSentence.VerbalPrepositionIndex != -1) ? words[currentVerbInSentence.VerbalPrepositionIndex].lexeme : "_";

			representation = LightVerbIndexLemma + "~" + NonVerbalElementLexeme + "~" + VerbalPreposiotionLexeme;
			return representation;
		}

		private static void mongoCountVerb(string verbStringRepresentation, long article, int sentence_index, int verb_index)
		{
			string connectionString = "mongodb://localhost";
			string databaseName = "crawler";
			string collectionName = "verbs";

			MongoServer server = MongoServer.Create(connectionString);
			MongoDatabase database = server.GetDatabase(databaseName);
			MongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

			BsonDocument seenOn = new BsonDocument().Add("article", article).Add("sentence_index", sentence_index).Add("verb_index", verb_index);
			QueryComplete whereQuery = Query.EQ("verb", verbStringRepresentation);
			UpdateBuilder updateQuery = Update.Inc("count", 1).Push("seen_on_sentence", seenOn);
			collection.Update(whereQuery, updateQuery, UpdateFlags.Upsert);

			server.Disconnect();
		}

	}
}
