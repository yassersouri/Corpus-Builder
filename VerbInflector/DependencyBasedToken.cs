using System.Text;

namespace VerbInflector
{
	public class DependencyBasedToken
	{
		public DependencyBasedToken(int pos, string word, string lemm, string cpos, string fpos, int head, string depRel, int wCount, MorphoSyntacticFeatures feats)
		{
			Position = pos;
			WordForm = word;
			Lemma = lemm;
			CPOSTag = cpos;
			FPOSTag = fpos;
			HeadNumber = head;
			DependencyRelation = depRel;
			TokenCount = wCount;
			MorphoSyntacticFeats = feats;
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder(200);
			result.Append("word: ").Append(WordForm.ToString()).Append(" | ");
			result.Append("position: ").Append(Position.ToString()).Append(" | ");
			result.Append("lemma: ").Append(Lemma.ToString()).Append(" | ");
			result.Append("cpos: ").Append(CPOSTag.ToString()).Append(" | ");
			result.Append("parent: ").Append(HeadNumber.ToString()).Append(" | ");
			result.Append("count: ").Append(TokenCount.ToString());
			return result.ToString();
		}

		public int Position { set; get; }
		public string WordForm { set; get; }
		public string Lemma { set; get; }
		public string CPOSTag { set; get; }
		public string FPOSTag { set; get; }
		public int HeadNumber { set; get; }
		public string DependencyRelation { set; get; }
		public int TokenCount { set; get; }
		public MorphoSyntacticFeatures MorphoSyntacticFeats { set; get; }
	}
}
