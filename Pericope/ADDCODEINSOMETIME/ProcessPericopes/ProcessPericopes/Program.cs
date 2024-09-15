
// See https://aka.ms/new-console-template for more information

StreamReader srKJV = new StreamReader("kjvstrongs.csv");
StreamReader srPericopeRanges = new StreamReader("PericopeRanges.csv");
StreamWriter swPericopeRangesVerseText = new StreamWriter("PericopeRangesVerseText.csv");

Dictionary<string, string> dKJV = new Dictionary<string, string>();

swPericopeRangesVerseText.WriteLine("Verse Reference^Verse Text^Pericope 1^Pericope 2^Pericope 3^Pericope 4");

srKJV.ReadLine();

while (!srKJV.EndOfStream)
{
    //1 BookName
    //2 ChapterNumber
    //3 VerseNumber
    //4 VerseText
    int intSplitCounter = 0;

    string strLine = srKJV.ReadLine().Trim();
    string[] strsLine = strLine.Split(',');
    int intArrayLength = strsLine.Length;
    string strTextUncleaned = "";
    string strTextCleaned = "";
    bool bAlmostAdd = false;
    bool bAddText = true;

    for (int intCommaIndex = 3; intCommaIndex < intArrayLength; intCommaIndex++)
    {
        if (strTextUncleaned == "")
        {
            strTextUncleaned = strsLine[intCommaIndex];
        }
        else
        {
            strTextUncleaned += ", " + strsLine[intCommaIndex];
        }
    }

    foreach (char ch in strTextUncleaned.Trim())
    {
        if (bAlmostAdd == true) 
        {
            bAddText = true;
            bAlmostAdd = false;
        }

        if (ch == '{')
        {
            bAddText = false;
        }
        else if (ch == '}')
        {
            bAddText = false;
            bAlmostAdd = true;
        }

        if (bAddText == true)
        {
            if (ch == ' ' && !strTextCleaned.EndsWith(' '))
            {
                strTextCleaned += ch;
            }
            else if (ch == ' ' && strTextCleaned.EndsWith(' '))
            {
                //do nothing
            }
            else if (ch != ' ')
            {
                strTextCleaned += ch;
            }
        }
    }

    dKJV.Add(strsLine[0].ToLower().Trim() + " " + strsLine[1].ToString().Trim() + ":" + strsLine[2].ToString().Trim(), strTextCleaned.Trim());
}

srKJV.Close();

srPericopeRanges.ReadLine();

while (!srPericopeRanges.EndOfStream)
{
    //1 Row ID
    //2 Pericope ID
    //3 Book Name
    //4 Book Number
    //5 Chapter Number
    //6 Verse Number
    //7 Pericope 1
    //8 Pericope 2
    //9 Pericope 3
    //10 Pericope 4

    string strLine = srPericopeRanges.ReadLine().Trim();
    string[] strsLine = strLine.Split('^');
    string strReference = "";
    System.Text.RegularExpressions.Regex rgxStartsWithANumber = new System.Text.RegularExpressions.Regex("[1-3]{1} [A-Za-z ]{1,}");
    
    if (rgxStartsWithANumber.IsMatch(strsLine[2]))
    {
       strReference = strsLine[2].ToLower().Trim().Remove(1, 1) + " " + strsLine[4].Trim() + ":" + strsLine[5].Trim(); //remove the second character, which is a space
    }
    else
    {
        strReference = strsLine[2].ToLower().Trim() + " " + strsLine[4].Trim() + ":" + strsLine[5].Trim();
    }

    string strVerseText = dKJV[strReference];

    swPericopeRangesVerseText.WriteLine(strReference + "^" + strVerseText + "^" + 
        strsLine[6] + "^" + strsLine[7] + "^" + strsLine[8] + "^" + strsLine[9]);
}

swPericopeRangesVerseText.Close();
srPericopeRanges.Close();
