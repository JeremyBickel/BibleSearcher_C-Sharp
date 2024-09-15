using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Categories
{
    public class SemanticTagging
    {
        public void WordTag(ref Dictionary<string, List<string>> dWordCategories,
            ref Dictionary<int, string> dWords, ref StreamWriter swHandTagged)
        {
            foreach (int intWordID in dWords.Keys.OrderBy(a => a))
            {
                StringBuilder sbCategories = new StringBuilder();

                foreach (string strWordCategory in dWordCategories.Keys.OrderBy(a=>a))
                {
                    if (strWordCategory != "uncategorized")
                    {
                        if (dWordCategories[strWordCategory].Contains(dWords[intWordID]))
                        {
                            sbCategories.Append(strWordCategory + ", ");
                        }
                    }
                }

                if (sbCategories.Length > 0)
                {
                    sbCategories.Remove(sbCategories.Length - 2, 2);

                    swHandTagged.WriteLine(dWords[intWordID] + " ^ " + sbCategories.ToString());
                }
            }

            //swHandTagged.Close(); - deferred until after PhraseTag finishes
        }

        public void PhraseTag(ref Dictionary<string, List<string>> dPhraseCategories,
            ref List<string> lPhrases, ref StreamWriter swHandTagged)
        {
            foreach (string strPhrase in lPhrases.OrderBy(a => a))
            {
                StringBuilder sbCategories = new StringBuilder();

                foreach (string strPhraseCategory in dPhraseCategories.Keys.OrderBy(a=>a))
                {
                    if (strPhraseCategory != "uncategorized")
                    {
                        if (dPhraseCategories[strPhraseCategory].Contains(strPhrase))
                        {
                            sbCategories.Append(strPhraseCategory + ", ");
                        }
                    }
                }

                if (sbCategories.Length > 0)
                {
                    sbCategories.Remove(sbCategories.Length - 2, 2);

                    swHandTagged.WriteLine(strPhrase + " ^ " + sbCategories.ToString());
                }
            }

            swHandTagged.Close();
        }
    }
}
