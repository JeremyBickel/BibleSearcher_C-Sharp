using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DeepAIConnection
{
    public class CallLLM
    {
        private const string DATA_DIR = "C:\\Users\\Jeremy\\LLM\\DeepAI\\";
        private const string CONFIG_FILE = DATA_DIR + "config.json";
        private const string INPUT_FILE = DATA_DIR + "PericopeRangesVerseText.csv";
        private const string STATE_FILE = DATA_DIR + "state.txt";
        private const string OUTPUT_FILE = DATA_DIR + "pericope_verse_annotations.txt";
        private static string API_KEY;
        //private const string PROMPT = "Use these terms to individually annotate the words in each sentence in the TEXT:\r\n\r\nClassification, Description, Similarity-Difference, Equality, Ownership, Timing, Order, Location, Priority, Cause-Effect, Means-End, Outcome, If-Then, Change, Connection, Opposition, Despite-Still, Contradiction, Either-Or, Clarification, Listing, Summary, Reference, Amplification, Extension, Possibility-Necessity\r\n\r\nTEXT:\r\n";
        private const string PROMPT = "Semantically annotate this TEXT like in the example.\r\nExample:\r\n\r\nWord: abez\r\n- POS: NOUN\r\n- Synonyms: N/A\r\n- Antonyms: N/A\r\n- Homophones: N/A\r\n- Homographs: N/A\r\n- Meronyms: N/A\r\n- Holonyms: Israel, Canaan\r\n- Hypernyms: location, city\r\n- Hyponyms: N/A\r\n- Troponyms: N/A\r\n- Coordinate Terms: Jerusalem, Bethlehem\r\n\r\nWord: abhor\r\n- POS: VERB\r\n- Synonyms: despise, loathe, detest, hate, abominate\r\n- Antonyms: love, admire, cherish, adore, like\r\n- Homophones: abhor\r\n- Homographs: N/A\r\n- Meronyms: N/A\r\n- Holonyms: N/A\r\n- Hypernyms: feel, dislike\r\n- Hyponyms: loathe, detest\r\n- Troponyms: N/A\r\n- Coordinate Terms: despise, loathe\r\n\r\nTEXT:\r\nAnd after these things I saw four angels standing on the four corners of the earth, holding the four winds of the earth, that the wind should not blow on the earth, nor on the sea, nor on any tree.";
        public void Main()
        {
            API_KEY = LoadApiKey(CONFIG_FILE);

            var (pericopeDict, referenceDict, textDict) = CreateInputDataStructures(INPUT_FILE);
            int startKey = LoadState();

            using (var file = new StreamWriter(OUTPUT_FILE, true))
            {
                foreach (var key in textDict.Keys.OrderBy(k => k))
                {
                    if (key >= startKey)
                    {
                        var result = Call(textDict[key]);
                        file.WriteLine(result);
                        file.WriteLine(); //empty line between records
                        SaveState(key);
                    }
                }
            }
        }

        private static string LoadApiKey(string configFile)
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configFile));
            return config["api_key"];
        }

        public Tuple<Dictionary<int, string>, Dictionary<int, string>, Dictionary<int, string>> CreateInputDataStructures(string inputFilePath)
        {
            var pericopeDict = new Dictionary<int, string>();
            var referenceDict = new Dictionary<int, string>();
            var textDict = new Dictionary<int, string>();

            int pericopeCounter = 1;
            string currentPericope = null;
            string currentStartReference = null;
            string currentEndReference = null;
            var currentText = new List<string>();

            using (var reader = new StreamReader(inputFilePath))
            {
                reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Trim();
                    var parts = line.Split('^');
                    var verseReference = parts[0].Trim();
                    var verseText = parts[1].Trim();
                    var pericope1 = parts[2].Trim();
                    var pericope2 = parts[3].Trim();
                    var pericope3 = parts[4].Trim();
                    var pericope4 = parts[5].Trim();

                    // Simplified chosen pericope selection
                    string chosenPericope = "";

                    if (string.IsNullOrEmpty(pericope4))
                    {
                        if (string.IsNullOrEmpty(pericope3))
                        {
                            if (string.IsNullOrEmpty(pericope2))
                            {
                                if (string.IsNullOrEmpty(pericope1))
                                {
                                    throw new Exception("Missing Pericope in DeepAIConnection.CalLLM");
                                }
                                else
                                {
                                    chosenPericope = pericope1;
                                }
                            }
                            else
                            {
                                chosenPericope = pericope2;
                            }
                        }
                        else
                        {
                            chosenPericope = pericope3;
                        }
                    }
                    else
                    {
                        chosenPericope = pericope4;
                    }
                    
                    if (string.IsNullOrEmpty(chosenPericope))
                    {
                        throw new Exception("Missing Pericope in DeepAIConnection.CalLLM");
                    }

                    if (currentPericope != null && chosenPericope != currentPericope)
                    {
                        pericopeDict[pericopeCounter] = currentPericope;
                        referenceDict[pericopeCounter] = $"{currentStartReference} - {currentEndReference}";
                        textDict[pericopeCounter] = string.Join(" ", currentText);
                        pericopeCounter++;
                        currentText.Clear();
                    }

                    if (!string.IsNullOrEmpty(chosenPericope))
                    {
                        if (currentPericope == null || chosenPericope != currentPericope)
                        {
                            currentPericope = chosenPericope;
                            currentStartReference = verseReference;
                        }
                        currentEndReference = verseReference;
                        currentText.Add(verseText);
                    }
                }
            }

            // Handle the last pericope
            if (currentPericope != null)
            {
                pericopeDict[pericopeCounter] = currentPericope;
                referenceDict[pericopeCounter] = $"{currentStartReference} - {currentEndReference}";
                textDict[pericopeCounter] = string.Join(" ", currentText);
            }

            File.WriteAllText("pericope_dict.txt", JsonConvert.SerializeObject(pericopeDict));
            File.WriteAllText("reference_dict.txt", JsonConvert.SerializeObject(referenceDict));
            File.WriteAllText("text_dict.txt", JsonConvert.SerializeObject(textDict));

            return new Tuple<Dictionary<int, string>, Dictionary<int, string>, Dictionary<int, string>>(pericopeDict, referenceDict, textDict);

        }

        private static void SaveState(int lastProcessedKey)
        {
            File.WriteAllText(STATE_FILE, lastProcessedKey.ToString());
        }

        private static int LoadState()
        {
            return File.Exists(STATE_FILE) ? int.Parse(File.ReadAllText(STATE_FILE)) : 0;
        }

        private static string Call(string text)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", API_KEY);  // Add headers

                var values = new Dictionary<string, string>
                    {
                        { "text", PROMPT } // + "\r\n" + text }
                    };

                var content = new FormUrlEncodedContent(values);  // This will format your content as `application/x-www-form-urlencoded`

                var response = client.PostAsync("https://api.deepai.org/api/text-generator", content).Result;
                    //new StringContent(JsonConvert.SerializeObject(new { text = text }), System.Text.Encoding.UTF8, "application/json")).Result;

                var return_content = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(return_content);
                return result["output"];
            }
        }
    }
}



