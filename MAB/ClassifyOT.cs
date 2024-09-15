using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MAB
{
    public class ClassifyMABOT
    {
        public void TransformMABOTXML(ref FileStream fsMABOT, ref StreamWriter swMABOT)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(verses));
            verses versesSerializer;
            
            List<string> lCKind = new();
            List<string> lCTyp = new();
            List<string> lPd = new();
            List<string> lPt = new();
            List<string> lPu = new();

            versesSerializer = (verses)serializer.Deserialize(fsMABOT);

            if (versesSerializer.verse != null) //.verse is a collection of verses
            {
                foreach (versesVerse vv in versesSerializer.verse)
                {
                    //Items and ItemsElementName have the same number
                    //of elements, and they correspond with each other.

                    //Always check heb items for null, and note that 
                    //hbint can contain square-bracketted notes besides
                    //the English translation (ie. "god [pl.]" and "[object marker]").

                    //NOTE: MAYBE ANTONYMS WILL BE USEFULL AS RICH CONNECTORS MAKING
                    ////DEEPER TRUTH OBVIOUS (IE. INDIVIDUALITY VS.PERSONALITY)
                    
                    string strVerseID = "";
                    string strCLDivClass = "";
                    string strCLDivDivClass = "";
                    string strCLDivDivID = "";
                    string strEnglishWord = "";
                    int intCCounter = 0;
                    List<int> lintCKind = new();
                    List<int> lintCTyp = new();
                    string strCKind = "";
                    string strCTyp = "";
                    List<int> lintBHPDiv = new();
                    string strWordParse = "";
                    string strCLDivDivDivID = "";
                    string strCLDivDivDivPFunction = "";
                    Dictionary<int, string> dCLDivDivDivPtypText = new();
                    string strCLDivDivDivPtypDet = "";
                    string strCLDivDivDivPtypUndet = "";

                    strVerseID = vv.vid.id;

                    swMABOT.Write(strVerseID);

                    foreach (versesVerseCL cl in vv.cl)
                    {
                        versesVerseCLDiv cldiv = cl.div;
                        strCLDivClass = cldiv.@class;

                        foreach (versesVerseCLDivDiv cldiv2 in cldiv.div) //chunks
                        {
                            strCLDivDivClass = cldiv2.@class;
                            strCLDivDivID = cldiv2.id;

                            //If strCLDivDivClass == cdesc, then look at Items with Names
                            //other than "ref". These Items give good information
                            //about the chunk, including ckind (ie. "Verbal clauses,")
                            //and ctyp (ie. "x-qatal-X clause")
                            if (strCLDivDivClass == "cdesc")
                            {
                                intCCounter = 0;
                                lintCKind.Clear();
                                lintCTyp.Clear();

                                swMABOT.Write(" ^ " + "c:" + strCLDivDivID);

                                foreach (ItemsChoiceType ict in cldiv2.ItemsElementName)
                                {
                                    intCCounter++;

                                    if (ict == ItemsChoiceType.ckind)
                                    {
                                        lintCKind.Add(intCCounter);
                                    }
                                    else if (ict == ItemsChoiceType.ctyp)
                                    {
                                        lintCTyp.Add(intCCounter);
                                    }
                                }

                                intCCounter = 0;
                                strCKind = "";
                                strCTyp = "";

                                foreach (object oCLDivDivItems in cldiv2.Items)
                                {
                                    intCCounter++;

                                    if (lintCKind.Contains(intCCounter))
                                    {
                                        strCKind = oCLDivDivItems.ToString();

                                        swMABOT.Write(" ^ " + "CKind:" + strCKind);

                                        if (!lCKind.Contains(strCKind))
                                        {
                                            lCKind.Add(strCKind);
                                        }
                                    }
                                    else if (lintCTyp.Contains(intCCounter))
                                    {
                                        strCTyp = oCLDivDivItems.ToString();

                                        swMABOT.Write(" ^ " + "CTyp:" + strCTyp);

                                        if (!lCTyp.Contains(strCTyp))
                                        {
                                            lCTyp.Add(strCTyp);
                                        }
                                    }
                                }

                                //write cdesc
                            }
                            //If strCLDivDivClass == bhp, then look at Items with Names of
                            //"div", which are of type versesVerseCLDivDivDiv.
                            else if (strCLDivDivClass == "bhp")
                            {
                                intCCounter = 0;
                                lintCKind.Clear();
                                lintCTyp.Clear();
                                lintBHPDiv.Clear();

                                foreach (ItemsChoiceType ict in cldiv2.ItemsElementName)
                                {
                                    intCCounter++;

                                    if (ict == ItemsChoiceType.div)
                                    {
                                        lintCKind.Add(intCCounter);
                                    }
                                }

                                intCCounter = 0;

                                foreach (object oCLDivDivItems in cldiv2.Items)
                                {
                                    intCCounter++;

                                    if (lintCKind.Contains(intCCounter))
                                    {
                                        if (oCLDivDivItems.GetType() == typeof(versesVerseCLDivDivDiv)) 
                                        {
                                            //If cldiv2.Items[x].@class == bhw, then it's hbint is the 
                                            //English word and heb has a lot of fields about the Hebrew.
                                            //Of particular note in heb (which is a collection of type
                                            //versesVerseCLDivDivDivHeb), is the 5th comma-separated
                                            //field of "onClick", which is the parse information for
                                            //that Hebrew word.
                                            if (((versesVerseCLDivDivDiv)oCLDivDivItems).@class == "bhw")
                                            {
                                                //After checking, it seems that only heb[0] exists, so:
                                                versesVerseCLDivDivDivHeb oHeb = ((versesVerseCLDivDivDiv)oCLDivDivItems).heb[0];
                                                int intStartIndex = oHeb.onclick.IndexOf('(');
                                                int intEndIndex = oHeb.onclick.LastIndexOf(')');
                                                string strOnclickPart = oHeb.onclick.Substring(intStartIndex, intEndIndex - intStartIndex);
                                                string strWordID = oHeb.id;

                                                strEnglishWord = ((versesVerseCLDivDivDiv)oCLDivDivItems).hbint;


                                                strWordParse = strOnclickPart.Split(',')[4].Trim('\''); //the 5th element

                                                swMABOT.Write(" ^ " + "w:" + strWordID);
                                                //first split strEnglishWord on ',' to give multiple whole possible translations.
                                                //then:
                                                //if, upon splitting strEnglishWord, we find a split part that contains
                                                //a '[' (which assumably contains ']' also), then if a '+' follows the ']'
                                                //and if there is a next split part, then the word in brackets,
                                                //though not part of the definition, adds to that next split part and refers
                                                //to a previously seen word (such as a pronoun referring to a previous proper noun).
                                                //'+[' works similarly, but adds to the previous word (split part), regardless of
                                                //whether it is the first, last or other split part in strEnglishWord.
                                                //NOTE: sometimes there is no + ?and/or? the whole strEnglishWord is bracketed
                                                swMABOT.Write(" ^ " + "we:" + strEnglishWord);
                                                swMABOT.Write(" ^ " + "wp:" + strWordParse);
                                                //write bhw
                                            }
                                            //If it's .@class is "pdesc", then look at pFunction
                                            //(ie. Time reference) and pTyp, which is of type
                                            //versesVerseCLDivDivDivPtyp and has a det, undet and text,
                                            //at least one of which should have a phrase type
                                            //(ie. Prepositional phrase).
                                            else if (((versesVerseCLDivDivDiv)oCLDivDivItems).@class == "pdesc")
                                            {
                                                //strid, strpfunction, optyp:.det, .undet, .Text
                                                strCLDivDivDivID = ((versesVerseCLDivDivDiv)oCLDivDivItems).id;
                                                strCLDivDivDivPFunction = ((versesVerseCLDivDivDiv)oCLDivDivItems).pfunction;

                                                swMABOT.Write(" ^ " + "p:" + strCLDivDivDivID);

                                                if (((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.Text != null)
                                                {
                                                    foreach (string strText in ((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.Text)
                                                    {
                                                        dCLDivDivDivPtypText.Add(dCLDivDivDivPtypText.Count + 1, strText);

                                                        swMABOT.Write(" ^ " + "pt:" + strText);

                                                        if (!lPt.Contains(strText))
                                                        {
                                                            lPt.Add(strText);
                                                        }
                                                    }
                                                }

                                                if (((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.det != null)
                                                {
                                                    strCLDivDivDivPtypDet = ((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.det;

                                                    swMABOT.Write(" ^ " + "pd:" + strCLDivDivDivPtypDet);

                                                    if (!lPd.Contains(strCLDivDivDivPtypDet))
                                                    {
                                                        lPd.Add(strCLDivDivDivPtypDet);
                                                    }
                                                }

                                                if (((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.undet != null)
                                                {
                                                    strCLDivDivDivPtypUndet = ((versesVerseCLDivDivDiv)oCLDivDivItems).ptyp.undet;

                                                    swMABOT.Write(" ^ " + "pu:" + strCLDivDivDivPtypUndet);

                                                    if (!lPu.Contains(strCLDivDivDivPtypUndet))
                                                    {
                                                        lPu.Add(strCLDivDivDivPtypUndet);
                                                    }
                                                }

                                                //write pdesc
                                            }
                                        }
                                    }
                                }

                                swMABOT.WriteLine();
                                //write bhp
                            }
                        }

                        
                    }
                }
            }

            swMABOT.Close();

            //NOTE: It seems I created these functions twice.
            //Find this functionality in MAB.Verses.WriteParseCombinations(..)
            //foreach (string strLine in lCKind.OrderBy(a => a))
            //{
            //    swCKind.WriteLine(strLine);
            //}
            //swCKind.Close();

            //foreach (string strLine in lCTyp.OrderBy(a => a))
            //{
            //    swCTyp.WriteLine(strLine);
            //}
            //swCTyp.Close();

            //foreach (string strLine in lPd.OrderBy(a => a))
            //{
            //    swPd.WriteLine(strLine);
            //}
            //swPd.Close();

            //foreach (string strLine in lPt.OrderBy(a => a))
            //{
            //    swPt.WriteLine(strLine);
            //}
            //swPt.Close();

            //foreach (string strLine in lPu.OrderBy(a => a))
            //{
            //    swPu.WriteLine(strLine);
            //}
            //swPu.Close();
        }
    }

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class verses
    {
        private versesVerse[] verseField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("verse")]
        public versesVerse[] verse
        {
            get
            {
                return this.verseField;
            }
            set
            {
                this.verseField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerse
    {

        private versesVerseVid vidField;

        private versesVerseCL[] clField;

        /// <remarks/>
        public versesVerseVid vid
        {
            get
            {
                return this.vidField;
            }
            set
            {
                this.vidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("cl")]
        public versesVerseCL[] cl
        {
            get
            {
                return this.clField;
            }
            set
            {
                this.clField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseVid
    {

        private string idField;

        private string onclickField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string onclick
        {
            get
            {
                return this.onclickField;
            }
            set
            {
                this.onclickField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public byte Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCL
    {

        private versesVerseCLDiv divField;

        /// <remarks/>
        public versesVerseCLDiv div
        {
            get
            {
                return this.divField;
            }
            set
            {
                this.divField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDiv
    {

        private versesVerseCLDivDiv[] divField;

        private string classField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("div")]
        public versesVerseCLDivDiv[] div
        {
            get
            {
                return this.divField;
            }
            set
            {
                this.divField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDivDiv
    {

        private object[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private string[] textField;

        private string idField;

        private string classField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ckind", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("connector", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("crela", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("ctyp", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("div", typeof(versesVerseCLDivDivDiv))]
        [System.Xml.Serialization.XmlElementAttribute("heb", typeof(object))]
        [System.Xml.Serialization.XmlElementAttribute("ref", typeof(versesVerseCLDivDivRef))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDivDivRef
    {

        private string mclidField;

        private string clidField;

        private string[] textField;

        private string onclickField;

        /// <remarks/>
        public string mclid
        {
            get
            {
                return this.mclidField;
            }
            set
            {
                this.mclidField = value;
            }
        }

        /// <remarks/>
        public string clid
        {
            get
            {
                return this.clidField;
            }
            set
            {
                this.clidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string onclick
        {
            get
            {
                return this.onclickField;
            }
            set
            {
                this.onclickField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDivDivDiv
    {

        private versesVerseCLDivDivDivHeb[] hebField;

        private string hbintField;

        private versesVerseCLDivDivDivPtyp ptypField;

        private string prelaField;

        private string pfunctionField;

        private string idField;

        private string classField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("heb")]
        public versesVerseCLDivDivDivHeb[] heb
        {
            get
            {
                return this.hebField;
            }
            set
            {
                this.hebField = value;
            }
        }

        /// <remarks/>
        public string hbint
        {
            get
            {
                return this.hbintField;
            }
            set
            {
                this.hbintField = value;
            }
        }

        /// <remarks/>
        public versesVerseCLDivDivDivPtyp ptyp
        {
            get
            {
                return this.ptypField;
            }
            set
            {
                this.ptypField = value;
            }
        }

        /// <remarks/>
        public string prela
        {
            get
            {
                return this.prelaField;
            }
            set
            {
                this.prelaField = value;
            }
        }

        /// <remarks/>
        public string pfunction
        {
            get
            {
                return this.pfunctionField;
            }
            set
            {
                this.pfunctionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDivDivDivHeb
    {

        private string idField;

        private string classField;

        private string onclickField;

        private string onmouseoverField;

        private string onmouseoutField;

        private string ondblclickField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string onclick
        {
            get
            {
                return this.onclickField;
            }
            set
            {
                this.onclickField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string onmouseover
        {
            get
            {
                return this.onmouseoverField;
            }
            set
            {
                this.onmouseoverField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string onmouseout
        {
            get
            {
                return this.onmouseoutField;
            }
            set
            {
                this.onmouseoutField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ondblclick
        {
            get
            {
                return this.ondblclickField;
            }
            set
            {
                this.ondblclickField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class versesVerseCLDivDivDivPtyp
    {

        private string detField;

        private string undetField;

        private string[] textField;

        /// <remarks/>
        public string det
        {
            get
            {
                return this.detField;
            }
            set
            {
                this.detField = value;
            }
        }

        /// <remarks/>
        public string undet
        {
            get
            {
                return this.undetField;
            }
            set
            {
                this.undetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {

        /// <remarks/>
        ckind,

        /// <remarks/>
        connector,

        /// <remarks/>
        crela,

        /// <remarks/>
        ctyp,

        /// <remarks/>
        div,

        /// <remarks/>
        heb,

        /// <remarks/>
        @ref,
    }


}
