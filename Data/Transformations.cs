using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KJVStrongs;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace Data
{
    // Special Note: There are 8 places where a sequence like <sup><clinfo>1</sup></clinfo> occurs, between </grk> and <gloss>. It throws everything off, and I don't know what use it is, so I TRIED TO hand edited it out of the original data, but the modified data was still off - it had wrong clause labels, maybe due to this or maybe something else. So, I'm going to have to work the exception in here and see if the modified data clauses are still off.

    public class Transformations
    {
        public class HeaderData
        {
            public string Chapter { get; set; }
            public string Verse { get; set; }
            public string Book { get; set; }
            public string Class1 { get; set; }
            public string Class2 { get; set; }
            public string Symbol { get; set; }
        }

        public class GrkWord
        {
            public string GrkId { get; set; }
            public string Cl { get; set; }
            public string Sn { get; set; }
            public string Morph { get; set; }
            public string Word { get; set; }
        }

        public class ProcessedData
        {
            public HeaderData hdData { get; set; }
            public List<GrkWord> gwsData { get; set; }
        }
        
        public class DiscourseJSON
        {
            public string Book { get; set; }
            public string Chapter { get; set; }
            public string Verse { get; set; }
            public string Scripture { get; set; }
        }

        public class TransformLevensohnsGNTDiscourseFeatures
        {
            public Regex rgxGreek = new Regex(@"<grk.*?id=\""(?<GrkId>.*?)\"".*?cl='(?<Cl>.*?)'.*?sn='(?<Sn>.*?)'.*?morph='(?<Morph>.*?)'>(?<Word>.*?)<\/grk>", RegexOptions.Compiled);

            public void Go(string strInputFile = @"F:\BibleSearcher\Data\Data\Original\MarvelData-JSON\Data\discourseNT.data_Bible.json")
            {
                JsonTextReader jtrInput = new JsonTextReader(new StreamReader(strInputFile));
                IList<DiscourseJSON> lInput = new List<DiscourseJSON>();
                string jsonl = "";

                jtrInput.SupportMultipleContent = true;

                while (true)
                {
                    if (!jtrInput.Read())
                    {
                        break;
                    }

                    JsonSerializer serializer = new JsonSerializer();
                    DiscourseJSON djInput = serializer.Deserialize<DiscourseJSON>(jtrInput);

                    lInput.Add(djInput);
                }

                foreach (DiscourseJSON djInput in lInput)
                {

                    HeaderData headerData = new HeaderData
                    {
                        Book = djInput.Book,
                        Chapter = djInput.Chapter,
                        Verse = djInput.Verse
                    };

                    var processedData = new ProcessedData()
                    {
                        hdData = headerData,
                        gwsData = ExtractGreekWordsData(djInput.Scripture)
                    };

                    jsonl += JsonConvert.SerializeObject(processedData) + Environment.NewLine;
                }
                
                System.IO.File.WriteAllText("output.jsonl", jsonl);
            }

            public HeaderData ExtractHeader(string strInput)
            {
                string strHeader = "\"Chapter\": (?<Chapter>\\d+),\\s*" +
                "\"Verse\": (?<Verse>\\d+),\\s*" +
                "\"Book\": (?<Book>\\d+),\\s*" +
                "\"Scripture\":.*?class:\\s*\\\"(?<Class1>nt)\\\".*?class:\\s*'(?<Class2>e)'.*?\\\"cl\\('\\d+'\\)\\\":\\s*\\\"(?<Symbol>[^\\\"]+)\\\".*?";

                Match mHeader = Regex.Match(strInput, strHeader);

                HeaderData hdReturn = new HeaderData();

                hdReturn.Book = mHeader.Groups["Book"].Value;
                hdReturn.Chapter = mHeader.Groups["Chapter"].Value;
                hdReturn.Verse = mHeader.Groups["Verse"].Value;
                hdReturn.Class1 = mHeader.Groups["Class1"].Value;
                hdReturn.Class2 = mHeader.Groups["Class2"].Value;
                hdReturn.Symbol = mHeader.Groups["Symbol"].Value;

                return hdReturn;
            }

            public List<GrkWord> ExtractGreekWordsData(string scripture)
            {
                var matches = rgxGreek.Matches(scripture);
                var words = new List<GrkWord>();

                foreach (Match match in matches)
                {
                    words.Add(new GrkWord
                    {
                        GrkId = match.Groups[1].Value,
                        Cl = match.Groups[2].Value,
                        Sn = match.Groups[3].Value,
                        Morph = match.Groups[4].Value,
                        Word = match.Groups[5].Value
                    });
                }
                return words;
            }

            public void Test()
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                //doc.LoadHtml(htmlString);

                foreach (var node in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    Console.WriteLine(node.GetAttributeValue("href", ""));
                }
            }
        }

        public void PhraseReductionPipeline()
        {
            TransformPhrasesNames(@"F:\Phrases.txt", @"F:\biblePeople.data_PEOPLERELATIONSHIP.json", "Names", new Regex("\"name\": \"(?<Names>[^\"]*)\",{0,1}"));
            TransformPhrasesNames(@"F:\PhrasesWithoutNames.txt", @"F:\BibleLocations.txt", "Locations", new Regex("[A-Z] - (?<Locations>.*)"));
        }

        public void TransformPhrasesNames(string strPhrasesFilename, string strSpecialFilename, string strSpecialTypeName, Regex rgxSpecial)
        {
            StreamReader srSpecial = new StreamReader(strSpecialFilename);
            //StreamReader srNames = new StreamReader(@"F:\biblePeople.data_PEOPLERELATIONSHIP.json");

            StreamReader srPhrases = new StreamReader(strPhrasesFilename);
            //StreamReader srPhrases = new StreamReader(@"F:\Phrases.txt");

            int intLastSlashIdx = strSpecialFilename.LastIndexOf("\\");
            if (intLastSlashIdx == strSpecialFilename.Length - 1)
            {
                intLastSlashIdx = strSpecialFilename.Substring(0, strSpecialFilename.Length - 1).LastIndexOf("\\");
            }
            string strDirectory = strSpecialFilename.Substring(0, intLastSlashIdx);

            StreamWriter swSpecial = new StreamWriter(strDirectory + "\\" + strSpecialTypeName + ".txt");
            //StreamWriter swNames = new StreamWriter(@"F:\Names.txt");

            StreamWriter swSpecialWithOut = new StreamWriter(strDirectory + "\\PhrasesWith" + strSpecialTypeName + ".txt");
            //StreamWriter swPhrasesOut = new StreamWriter(@"F:\PhrasesWithNames.txt");

            StreamWriter swSpecialWithoutOut = new StreamWriter(strDirectory + "\\PhrasesWithout" + strSpecialTypeName + ".txt");
            //StreamWriter swPhrases = new StreamWriter(@"F:\PhrasesWithoutNames.txt");

            HashSet<string> hsetSpecial = new HashSet<string>();
            List<string> lPhrases = new List<string>();
            List<string> lSpecialOut = new List<string>();
            List<string> lPhrasesOut = new List<string>();

            //Regex rgxPhraseLine = new Regex("\b(?<name>.*)\b");

            while (!srSpecial.EndOfStream)
            {
                string strLine = srSpecial.ReadLine().ToLower().Trim();

                if (strLine != "")
                {
                    if (rgxSpecial.IsMatch(strLine))
                    {
                        string strNameTemp = rgxSpecial.Match(strLine).Groups[strSpecialTypeName].Value;

                        strNameTemp = Regex.Replace(strNameTemp, @"[^a-z\-]", "").ToLower().Trim();

                        if (strSpecialTypeName == "Locations")
                        {
                            foreach (string strLocation in strNameTemp.Split('\\'))
                            {
                                if (!hsetSpecial.Contains(strLocation))
                                {
                                    hsetSpecial.Add(strLocation);
                                }
                            }
                        }
                        else if (strSpecialTypeName == "Names")
                        {
                            if (!hsetSpecial.Contains(strNameTemp))
                            {
                                hsetSpecial.Add(strNameTemp);
                            }
                        }
                    }
                }
            }
            srSpecial.Close();

            while (!srPhrases.EndOfStream)
            {
                string strLine = srPhrases.ReadLine().ToLower().Trim();

                if (strLine != "")
                {
                    lPhrases.Add(strLine);
                }
            }
            srPhrases.Close();

            lPhrases = lPhrases.Select(p => Regex.Replace(p.ToLower().Trim(), @"[^a-z0-9 ]", "")).ToList();

            foreach (string strSpecialTest in hsetSpecial)
            {
                Regex rgxName = new Regex(@"\b" + strSpecialTest + @"\b", RegexOptions.Compiled);
                var lTestPhrases = lPhrases.Where(p => p.Contains(strSpecialTest)).ToList();

                foreach (string strPhrase in lTestPhrases)
                {
                    if (rgxName.IsMatch(strPhrase)) //if name is in phrase on word boundaries, remove the phrase by putting it in the intermediate list
                    {
                        if (!lPhrasesOut.Contains(strPhrase))
                        {
                            lPhrasesOut.Add(strPhrase);
                        }
                    }
                }
            }

            lPhrases = lPhrases.Except(lPhrasesOut).ToList(); //I've FINALLY found it! .Except produces the set difference

            foreach (string strName in hsetSpecial.OrderBy(a => a))
            {
                swSpecial.WriteLine(strName);
            }
            swSpecial.Close();

            foreach (string strPhrase in lPhrases.OrderBy(a => a))
            {
                swSpecialWithoutOut.WriteLine(strPhrase);
            }
            swSpecialWithoutOut.Close();

            foreach (string strPhrase in lPhrasesOut.OrderBy(a => a))
            {
                swSpecialWithOut.WriteLine(strPhrase);
            }
            swSpecialWithOut.Close();
        }

    }
}
