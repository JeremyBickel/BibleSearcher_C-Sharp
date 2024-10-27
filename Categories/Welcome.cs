using System.Net;

namespace Categories
{
    public class Welcome
    {   
        public string strWordFilename;
        public string strPhraseFilename;
        public string strCategory;
        public Dictionary<string, List<string>> dWordCategories; //D<category, L<words>>
        public Dictionary<string, List<string>> dPhraseCategories; //D<category, L<phrases>>

        //animals, exclamations, realative comparatives, people, things, places, action verbs,
        //money, times, weights, measures, qualities, numbers, food, uncategorized

        public Welcome(string strWordCategoriesFilename, string strPhraseCategoriesFilename)
        {
            strWordFilename = strWordCategoriesFilename;
            strPhraseFilename = strPhraseCategoriesFilename;

            strCategory = "";
            dWordCategories = new();
            dPhraseCategories = new();

            LoadWords();
            LoadPhrases();
        }

        public void LoadWords()
        {
            StreamReader srWordCategories = new(strWordFilename);

            dWordCategories.Clear();

            while (!srWordCategories.EndOfStream)
            {
                string strLine = srWordCategories.ReadLine();

                if (strLine.Trim() != "")
                {
                    string strWord = "";
                    string strCount = "";

                    if (strLine.StartsWith('{')) //group name
                    {
                        if (strLine.Contains('('))
                        {
                            int intFirstParenthesis = strLine.IndexOf('(');

                            strCategory = strLine.Substring(1, intFirstParenthesis - 2).Trim().ToLower();
                        }
                        else
                        {
                            strCategory = strLine.TrimStart('{').TrimEnd('}').ToLower();
                        }
                    }
                    else //word ^ count
                    {
                        strWord = strLine.Split('^')[0].Trim().ToLower();
                    }

                    AddWord(strWord, strCategory);
                }
            }

            srWordCategories.Close();

            RemoveEmptyWords(); //cleans out "" from each category
        }

        public void AddWord(string strWord, string strCategory)
        {
            if (strWord != "" && strCategory != "")
            {
                if (!dWordCategories.ContainsKey(strCategory))
                {
                    dWordCategories.Add(strCategory, new List<string>());
                }

                if (!dWordCategories[strCategory].Contains(strWord))
                {
                    dWordCategories[strCategory].Add(strWord);
                }
            }
        }

        public void RemoveWord(string strWord, string strCategory)
        {
            if (dWordCategories.ContainsKey(strCategory))
            {
                if (dWordCategories[strCategory].Contains(strWord))
                {
                    dWordCategories[strCategory].Remove(strWord);
                }
            }

            if (!dWordCategories["uncategorized"].Contains(strWord))
            {
                dWordCategories["uncategorized"].Add(strWord);
            }
        }

        public void RemoveEmptyWords()
        {
            //remove all empty strings
            foreach (string strWordCategory in dWordCategories.Keys)
            {
                if (dWordCategories.ContainsKey(strWordCategory))
                {
                    if (dWordCategories[strWordCategory].Contains(""))
                    {
                        dWordCategories[strWordCategory].Remove("");
                    }
                }
            }
        }

        public string NextWord()
        {
            if (dWordCategories["uncategorized"].Count > 0)
            {
                string strNextWord = dWordCategories["uncategorized"].First();

                return strNextWord;
            }
            else
            {
                return "!!YOU'RE DONE WITH THE WORDS!!";
            }
        }

        public void WriteWords()
        {
            StreamWriter swSemanticWords = new(strWordFilename);

            foreach (string strCategory in dWordCategories.Keys.OrderBy(a=>a))
            {
                swSemanticWords.WriteLine("{" + strCategory + "}");

                foreach (string strWord in dWordCategories[strCategory])
                {
                    swSemanticWords.WriteLine(strWord);
                }
            }

            swSemanticWords.Close();
        }

        public void LoadPhrases()
        {
            StreamReader srPhraseCategories = new(strPhraseFilename);

            dPhraseCategories.Clear();

            while (!srPhraseCategories.EndOfStream)
            {
                string strLine = srPhraseCategories.ReadLine();

                if (strLine.Trim() != "")
                {
                    string strPhrase = "";

                    if (strLine.StartsWith('{')) //group name
                    {
                        {
                            strCategory = strLine.TrimStart('{').TrimEnd('}').ToLower();
                        }
                    }
                    else //phrase
                    {
                        strPhrase = strLine.Trim().ToLower();
                    }

                    AddPhrase(strPhrase, strCategory);
                }
            }

            srPhraseCategories.Close();

            RemoveEmptyPhrases();
        }

        public void AddPhrase(string strPhrase, string strCategory)
        {
            if (strPhrase != "" && strCategory != "")
            {
                if (!dPhraseCategories.ContainsKey(strCategory))
                {
                    dPhraseCategories.Add(strCategory, new List<string>());
                }

                if (!dPhraseCategories[strCategory].Contains(strPhrase))
                {
                    dPhraseCategories[strCategory].Add(strPhrase);
                }
            }
        }

        public void RemovePhrase(string strPhrase, string strCategory)
        {
            if (dPhraseCategories.ContainsKey(strCategory))
            {
                if (dPhraseCategories[strCategory].Contains(strPhrase))
                {
                    dPhraseCategories[strCategory].Remove(strPhrase);
                }
            }

            if (!dPhraseCategories["uncategorized"].Contains(strPhrase))
            {
                dPhraseCategories["uncategorized"].Add(strPhrase);
            }
        }

        public void RemoveEmptyPhrases()
        {
            //remove all empty strings
            foreach (string strPhraseCategory in dWordCategories.Keys)
            {
                if (dPhraseCategories.ContainsKey(strPhraseCategory))
                {
                    if (dPhraseCategories[strPhraseCategory].Contains(""))
                    {
                        dPhraseCategories[strPhraseCategory].Remove("");
                    }
                }
            }
        }

        public string NextPhrase()
        {
            if (dPhraseCategories["uncategorized"].Count > 0)
            {
                string strNextWord = dPhraseCategories["uncategorized"].First();

                return strNextWord;
            }
            else
            {
                return "!!YOU'RE DONE WITH THE PHRASES!!";
            }
        }

        public void WritePhrases()
        {
            StreamWriter swSemanticPhrases = new(strPhraseFilename);

            foreach (string strCategory in dPhraseCategories.Keys)
            {
                swSemanticPhrases.WriteLine("{" + strCategory + "}");

                foreach (string strPhrase in dPhraseCategories[strCategory])
                {
                    swSemanticPhrases.WriteLine(strPhrase);
                }
            }

            swSemanticPhrases.Close();
        }

        //adds semantically tagged words and phrases from KJVStrongs to a file,
        //except from the "uncategorized" category
        public void AddSemanticTaggingToKJVStrongsPhrases(ref Dictionary<int, string> dWords,
            ref StreamWriter swHandTagged, List<string> lPhrases)
        {
            SemanticTagging st = new();

            st.WordTag(ref dWordCategories, ref dWords, ref swHandTagged);
            st.PhraseTag(ref dPhraseCategories, ref lPhrases, ref swHandTagged);
        }
    }
}