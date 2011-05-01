using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tagger;

namespace Tagger
{
	class Program
	{
		static void Main(string[] args)
		{
			string word = "شما";

			Tagger t = new Tagger();

			Token token = t.TaggerHelper(word);

			Console.WriteLine(token);
		}
	}
}
