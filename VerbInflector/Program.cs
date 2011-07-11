﻿using System;
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
			

			//testMongo();

			VerbInflector_OneFile(sourceFile, destinationFile, verbDicPath);
		}

		private static void testMongo()
		{
			string connectionString = "mongodb://localhost";
			MongoServer server = MongoServer.Create(connectionString);

			string[] databaseNames = server.GetDatabaseNames().ToArray();

			
			
			MongoDatabase db = server.GetDatabase("crawler");
			string[] collectionNames = db.GetCollectionNames().ToArray();

			MongoCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("verbs");

			BsonDocument seenOn1 = new BsonDocument().Add("article", 4563456663).Add("sentence_index", 13).Add("verb_index", 3);
			BsonDocument seenOn2 = new BsonDocument().Add("article", 345345345).Add("sentence_index", 12).Add("verb_index", 0);
			BsonDocument seenOn3 = new BsonDocument().Add("article", 5656).Add("sentence_index", 34).Add("verb_index", 1);

			BsonArray seenOn = new BsonArray();
			seenOn.Add(seenOn1).Add(seenOn2);

			BsonDocument bd = new BsonDocument { { "verb", "رفت#رو~_~_" }, {"count" , 0}, {"seen_on_sentence", seenOn}};

			//collection.Insert(bd);

			

			QueryComplete qc = Query.EQ("verb", "sdfsdfws");
			
			//UpdateDocument ua = new UpdateDocument("$set", new BsonDocument("count", 2));
			UpdateBuilder ua = Update.Inc("count", 1).Push("seen_on_sentence", seenOn3);

			collection.Update(qc, ua, UpdateFlags.Upsert);

			MongoCursor<BsonDocument> mc = collection.Find(qc);
			BsonDocument[] bds = mc.ToArray<BsonDocument>();

			

			Console.WriteLine("End of monogDB test");
		}
		

		public static void VerbInflector_OneFile(string sourceFile, string destinationFile, string verbDicPath){
			Article currentArticle = ArticleUtils.getArticle(sourceFile);

			Article newArticle = generateNewArticle(currentArticle, verbDicPath);

			ArticleUtils.putArticle(newArticle, destinationFile);

			System.Console.WriteLine(newArticle);
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
			for(int i = 0; i < currentArticle.getSentences().Length; i++) //for each sentence in this article.
			{
				//initialize the new sentence
				newSentence = new Sentence();

				//load the current sentence
				currentSentence = currentArticle.getSentence(i);

				//load info about that sentence
				currentLexemes = currentSentence.getLexemes();
				currentPOSTags = currentSentence.getPOSTags();
				currentLemmas = currentSentence.getLemmas();
				currentFeatures = currentSentence.getFeatures();

				
				VerbBasedSentence currentSentenceVBS = SentenceAnalyzer.MakeVerbBasedSentence(currentLexemes, currentPOSTags, currentLemmas, currentFeatures, verbDicPath);
				List<DependencyBasedToken> list = currentSentenceVBS.SentenceTokens;


				//for each verb in sentence
				foreach(var currentVerbInSentence in currentSentenceVBS.VerbsInSentence)
				{
					//special string representation of the verb
					String currentVerbString = ValencyDicManager.GetVerbString(ref currentSentenceVBS, currentVerbInSentence); 

					if (ValencyDicManager.BaseStrucDic.ContainsKey(currentVerbString))
					{
						List<BaseStructure> baseStructuresForTheCurrentVerb = ValencyDicManager.BaseStrucDic[currentVerbString];
						List<BaseStructure> satisfiedBaseStructures = new List<BaseStructure>();
						foreach (var currentBaseStructure in baseStructuresForTheCurrentVerb)
						{
							if(currentBaseStructure.Satisfy(currentSentenceVBS, currentVerbInSentence))
							{
								satisfiedBaseStructures.Add(currentBaseStructure);
							}
						}
					}
					//counting the verb in sentence
				}

				//fitting one base structure
				//adding the fitted base structure to the database as the main verb
				

				//word index pointer in the current sentence
				int wordIndexInCurrentSentence = 0;
				
				Word currentWord = null;
				Word newWord = null;
				DependencyBasedToken currentDBT;
				for(int j = 0; j < list.Count() ; j++)
				{
					currentDBT = list[j];

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
						
						newWord.tma = currentDBT.MorphoSyntacticFeats.TenseMoodAspect.ToString();
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
				newArticle.addSentence(newSentence);
			}

			return newArticle;
		}

	}
}
