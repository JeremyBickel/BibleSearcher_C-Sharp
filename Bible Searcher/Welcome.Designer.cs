namespace Bible_Searcher
{
    partial class Welcome
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbBooks = new System.Windows.Forms.ListBox();
            this.rtbText = new System.Windows.Forms.RichTextBox();
            this.clbPericopeSearchTerms = new System.Windows.Forms.CheckedListBox();
            this.btnSearchPericope = new System.Windows.Forms.Button();
            this.rbnPericopeSearchAny = new System.Windows.Forms.RadioButton();
            this.rbnPericopeSearchAll = new System.Windows.Forms.RadioButton();
            this.gbxPericopeSearchLogic = new System.Windows.Forms.GroupBox();
            this.lbxPericopeSearchResults = new System.Windows.Forms.ListBox();
            this.btnBibleLinks = new System.Windows.Forms.Button();
            this.btnSemanticGroups = new System.Windows.Forms.Button();
            this.clbOriginalFiles = new System.Windows.Forms.CheckedListBox();
            this.clbProcessedFiles = new System.Windows.Forms.CheckedListBox();
            this.btnCreateIntersectingPhrases = new System.Windows.Forms.Button();
            this.btnCreateChainedPhrasalConcordance = new System.Windows.Forms.Button();
            this.lblExistingChainedConcordanceBoundaries = new System.Windows.Forms.Label();
            this.lblExistingIntersectingPhrasesBoundaries = new System.Windows.Forms.Label();
            this.tbxIntersectingMaximum = new System.Windows.Forms.TextBox();
            this.tbxIntersectingMinimum = new System.Windows.Forms.TextBox();
            this.tbxChainedSmallest = new System.Windows.Forms.TextBox();
            this.tbxChainedLargest = new System.Windows.Forms.TextBox();
            this.lblIntersectingPhrasesInstructions = new System.Windows.Forms.Label();
            this.lblChainedPhrasalConcordanceInstruction = new System.Windows.Forms.Label();
            this.lblChainedSmallestLength = new System.Windows.Forms.Label();
            this.lblChainedLargestLength = new System.Windows.Forms.Label();
            this.lblIntersectingMinimumVerseID = new System.Windows.Forms.Label();
            this.lblIntersectingMaximumVerseID = new System.Windows.Forms.Label();
            this.btnWriteKJVMobySynonyms = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataObjectsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gbxPericopeSearchLogic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataObjectsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lbBooks
            // 
            this.lbBooks.FormattingEnabled = true;
            this.lbBooks.ItemHeight = 15;
            this.lbBooks.Location = new System.Drawing.Point(12, 12);
            this.lbBooks.Name = "lbBooks";
            this.lbBooks.Size = new System.Drawing.Size(50, 64);
            this.lbBooks.TabIndex = 0;
            // 
            // rtbText
            // 
            this.rtbText.Location = new System.Drawing.Point(165, 12);
            this.rtbText.Name = "rtbText";
            this.rtbText.Size = new System.Drawing.Size(38, 66);
            this.rtbText.TabIndex = 1;
            this.rtbText.Text = "";
            // 
            // clbPericopeSearchTerms
            // 
            this.clbPericopeSearchTerms.FormattingEnabled = true;
            this.clbPericopeSearchTerms.Location = new System.Drawing.Point(119, 12);
            this.clbPericopeSearchTerms.Name = "clbPericopeSearchTerms";
            this.clbPericopeSearchTerms.Size = new System.Drawing.Size(40, 58);
            this.clbPericopeSearchTerms.TabIndex = 5;
            // 
            // btnSearchPericope
            // 
            this.btnSearchPericope.Location = new System.Drawing.Point(-4, 189);
            this.btnSearchPericope.Name = "btnSearchPericope";
            this.btnSearchPericope.Size = new System.Drawing.Size(75, 23);
            this.btnSearchPericope.TabIndex = 6;
            this.btnSearchPericope.Text = "Search";
            this.btnSearchPericope.UseVisualStyleBackColor = true;
            this.btnSearchPericope.Click += new System.EventHandler(this.btnSearchPericope_Click);
            // 
            // rbnPericopeSearchAny
            // 
            this.rbnPericopeSearchAny.AutoSize = true;
            this.rbnPericopeSearchAny.Checked = true;
            this.rbnPericopeSearchAny.Location = new System.Drawing.Point(6, 22);
            this.rbnPericopeSearchAny.Name = "rbnPericopeSearchAny";
            this.rbnPericopeSearchAny.Size = new System.Drawing.Size(46, 19);
            this.rbnPericopeSearchAny.TabIndex = 7;
            this.rbnPericopeSearchAny.TabStop = true;
            this.rbnPericopeSearchAny.Text = "Any";
            this.rbnPericopeSearchAny.UseVisualStyleBackColor = true;
            // 
            // rbnPericopeSearchAll
            // 
            this.rbnPericopeSearchAll.AutoSize = true;
            this.rbnPericopeSearchAll.Location = new System.Drawing.Point(6, 47);
            this.rbnPericopeSearchAll.Name = "rbnPericopeSearchAll";
            this.rbnPericopeSearchAll.Size = new System.Drawing.Size(39, 19);
            this.rbnPericopeSearchAll.TabIndex = 8;
            this.rbnPericopeSearchAll.Text = "All";
            this.rbnPericopeSearchAll.UseVisualStyleBackColor = true;
            // 
            // gbxPericopeSearchLogic
            // 
            this.gbxPericopeSearchLogic.Controls.Add(this.rbnPericopeSearchAny);
            this.gbxPericopeSearchLogic.Controls.Add(this.rbnPericopeSearchAll);
            this.gbxPericopeSearchLogic.Location = new System.Drawing.Point(12, 82);
            this.gbxPericopeSearchLogic.Name = "gbxPericopeSearchLogic";
            this.gbxPericopeSearchLogic.Size = new System.Drawing.Size(78, 76);
            this.gbxPericopeSearchLogic.TabIndex = 9;
            this.gbxPericopeSearchLogic.TabStop = false;
            this.gbxPericopeSearchLogic.Text = "Pericope Search Logic";
            // 
            // lbxPericopeSearchResults
            // 
            this.lbxPericopeSearchResults.FormattingEnabled = true;
            this.lbxPericopeSearchResults.HorizontalScrollbar = true;
            this.lbxPericopeSearchResults.ItemHeight = 15;
            this.lbxPericopeSearchResults.Location = new System.Drawing.Point(68, 12);
            this.lbxPericopeSearchResults.Name = "lbxPericopeSearchResults";
            this.lbxPericopeSearchResults.Size = new System.Drawing.Size(45, 64);
            this.lbxPericopeSearchResults.TabIndex = 10;
            // 
            // btnBibleLinks
            // 
            this.btnBibleLinks.Location = new System.Drawing.Point(-4, 164);
            this.btnBibleLinks.Name = "btnBibleLinks";
            this.btnBibleLinks.Size = new System.Drawing.Size(75, 23);
            this.btnBibleLinks.TabIndex = 11;
            this.btnBibleLinks.Text = "Bible Links";
            this.btnBibleLinks.UseVisualStyleBackColor = true;
            this.btnBibleLinks.Click += new System.EventHandler(this.btnBibleLinks_Click);
            // 
            // btnSemanticGroups
            // 
            this.btnSemanticGroups.Location = new System.Drawing.Point(96, 82);
            this.btnSemanticGroups.Name = "btnSemanticGroups";
            this.btnSemanticGroups.Size = new System.Drawing.Size(107, 23);
            this.btnSemanticGroups.TabIndex = 12;
            this.btnSemanticGroups.Text = "Semantic Groups";
            this.btnSemanticGroups.UseVisualStyleBackColor = true;
            this.btnSemanticGroups.Click += new System.EventHandler(this.btnSemanticGroups_Click);
            // 
            // clbOriginalFiles
            // 
            this.clbOriginalFiles.FormattingEnabled = true;
            this.clbOriginalFiles.Items.AddRange(new object[] {
            "Bible - KJV Strongs",
            "Bible - MAB (With Semantic Data)",
            "Bible - STEP Bible - Old Testament",
            "Bible - STEP Bible - New Testament",
            "Lexicon - Blue Letter Bible - Hebrew Lexicon",
            "Lexicon - Blue Letter Bible - Greek Lexicon",
            "Concordance - Blue Letter Bible - Hebrew Concordance",
            "Concordance - Blue Letter Bible - Greek Concordance",
            "Pericopes - Crawfordville - Comparative Gospels",
            "Pericopes - Semantic Bible - Whole Bible",
            "Pericopes - TSK - Whole Bible",
            "Pericopes - Wikipedia - Acts",
            "Pericopes - Widipedia - Galatians",
            "Strongs - Hebrew",
            "Strongs - Greek",
            "Synonyms - Moby",
            "Synonyms - Roget",
            "Cross References"});
            this.clbOriginalFiles.Location = new System.Drawing.Point(72, 164);
            this.clbOriginalFiles.Name = "clbOriginalFiles";
            this.clbOriginalFiles.Size = new System.Drawing.Size(83, 58);
            this.clbOriginalFiles.TabIndex = 16;
            // 
            // clbProcessedFiles
            // 
            this.clbProcessedFiles.FormattingEnabled = true;
            this.clbProcessedFiles.Items.AddRange(new object[] {
            "KJV Strongs",
            "STEP Bible - Location, Parse, Strongs Number, Translations, Semantic Translation",
            "MAB",
            "",
            "Word IDs",
            "Word Positions",
            "Word Counts",
            "Phrases",
            "Verse - Phrase - POS",
            "",
            "Lexicon - Blue Letter Bible - Hebrew",
            "Lexicon - Blue Letter Bible - Greek",
            "Lexicon Derivations - Blue Letter Bible - Derivations/Connections",
            "Lexicon Derivations - Blue Letter Bible - Aramaic",
            "Lexicon Derivations - Blue Letter Bible - Roots",
            "Lexicon Derivations - Blue Letter Bible - RootedAramaic",
            "Lexicon Derivations - Blue Letter Bible - NonAramaic",
            "Lexicon Derivations - Blue Letter Bible - NonRoots",
            "Concordance - Blue Letter Bible - Hebrew",
            "Concordance - Blue Letter Bible - Greek",
            "Syllable Comparison - Blue Letter Bible - Hebrew",
            "Syllable Comparison - Blue Letter Bible - Greek",
            "Permutations - Book, Word, Word Count in this Book",
            "Strongs - Phrase Counts - By Count",
            "Strongs - Phrase Counts - By Phrase",
            "Strongs - Multiple Strongs Numbers - Counts",
            "Strongs - Multiple Strongs Numbers - Counts - First English Translation",
            "Strongs - Verse - Major Words - All English Translations",
            "Strongs - Verse - Phrase - Muliple Strongs Counter - Strongs Number",
            "Strongs - Verse - Strongs Numbers",
            "Cross References",
            "Phrasal Concordance",
            "Strongs Phrasal Concordance - English Translations",
            "Strongs Phrasal Concordance - Strongs Numbers",
            "Chained Phrasal Concordance - Ending Phrase Reference - Variable Length Set of Co" +
                "nsecutive Phrases",
            "Chained Phrasal Concordance - Phrase - Count",
            "Intersecting Phrases - 2 Verse IDs - Phrase - Percent Phrase Share in Verses - 2 " +
                "Verse Texts",
            "Pericopes - All Headings",
            "Pericopes - Heading, Locations",
            "Pericopes - Location, Headings",
            "Pericopes - Treasury of Scripture Knowledge",
            "Pericopes - Reference Range - Multiple Headings",
            "Pericopes - Words - Headings - Count of Word in Heading",
            "Synonyms - Moby - Group ID - Synonyms",
            "Synonyms - Moby Index - Synonym - Group IDs",
            "Synonyms - KJV Word Moby Synonyms",
            "",
            "Ongoing - Semantic Groups",
            "",
            "Experimental - Verse Relatedness by Phrase Similarity",
            "Experimental - Subject Verb Objects (Simple)",
            "",
            "Last Verse ID in Each Book",
            "Book Name - Chapter - Verse - VerseID"});
            this.clbProcessedFiles.Location = new System.Drawing.Point(165, 164);
            this.clbProcessedFiles.Name = "clbProcessedFiles";
            this.clbProcessedFiles.Size = new System.Drawing.Size(83, 58);
            this.clbProcessedFiles.TabIndex = 17;
            // 
            // btnCreateIntersectingPhrases
            // 
            this.btnCreateIntersectingPhrases.Location = new System.Drawing.Point(12, 389);
            this.btnCreateIntersectingPhrases.Name = "btnCreateIntersectingPhrases";
            this.btnCreateIntersectingPhrases.Size = new System.Drawing.Size(101, 60);
            this.btnCreateIntersectingPhrases.TabIndex = 18;
            this.btnCreateIntersectingPhrases.Text = "Create Intersecting Phrases";
            this.btnCreateIntersectingPhrases.UseVisualStyleBackColor = true;
            this.btnCreateIntersectingPhrases.Click += new System.EventHandler(this.btnCreateIntersectingPhrases_Click);
            // 
            // btnCreateChainedPhrasalConcordance
            // 
            this.btnCreateChainedPhrasalConcordance.Location = new System.Drawing.Point(12, 248);
            this.btnCreateChainedPhrasalConcordance.Name = "btnCreateChainedPhrasalConcordance";
            this.btnCreateChainedPhrasalConcordance.Size = new System.Drawing.Size(127, 58);
            this.btnCreateChainedPhrasalConcordance.TabIndex = 19;
            this.btnCreateChainedPhrasalConcordance.Text = "Create Chained Phrasal Concordance";
            this.btnCreateChainedPhrasalConcordance.UseVisualStyleBackColor = true;
            this.btnCreateChainedPhrasalConcordance.Click += new System.EventHandler(this.btnCreateChainedPhrasalConcordance_Click);
            // 
            // lblExistingChainedConcordanceBoundaries
            // 
            this.lblExistingChainedConcordanceBoundaries.AutoSize = true;
            this.lblExistingChainedConcordanceBoundaries.Location = new System.Drawing.Point(12, 309);
            this.lblExistingChainedConcordanceBoundaries.Name = "lblExistingChainedConcordanceBoundaries";
            this.lblExistingChainedConcordanceBoundaries.Size = new System.Drawing.Size(102, 15);
            this.lblExistingChainedConcordanceBoundaries.TabIndex = 20;
            this.lblExistingChainedConcordanceBoundaries.Text = "Lengths   to   Exist";
            // 
            // lblExistingIntersectingPhrasesBoundaries
            // 
            this.lblExistingIntersectingPhrasesBoundaries.AutoSize = true;
            this.lblExistingIntersectingPhrasesBoundaries.Location = new System.Drawing.Point(12, 452);
            this.lblExistingIntersectingPhrasesBoundaries.Name = "lblExistingIntersectingPhrasesBoundaries";
            this.lblExistingIntersectingPhrasesBoundaries.Size = new System.Drawing.Size(103, 15);
            this.lblExistingIntersectingPhrasesBoundaries.TabIndex = 21;
            this.lblExistingIntersectingPhrasesBoundaries.Text = "VerseIDs   to   Exist";
            // 
            // tbxIntersectingMaximum
            // 
            this.tbxIntersectingMaximum.Location = new System.Drawing.Point(119, 418);
            this.tbxIntersectingMaximum.Name = "tbxIntersectingMaximum";
            this.tbxIntersectingMaximum.Size = new System.Drawing.Size(36, 23);
            this.tbxIntersectingMaximum.TabIndex = 22;
            // 
            // tbxIntersectingMinimum
            // 
            this.tbxIntersectingMinimum.Location = new System.Drawing.Point(119, 389);
            this.tbxIntersectingMinimum.Name = "tbxIntersectingMinimum";
            this.tbxIntersectingMinimum.Size = new System.Drawing.Size(36, 23);
            this.tbxIntersectingMinimum.TabIndex = 23;
            // 
            // tbxChainedSmallest
            // 
            this.tbxChainedSmallest.Location = new System.Drawing.Point(145, 248);
            this.tbxChainedSmallest.Name = "tbxChainedSmallest";
            this.tbxChainedSmallest.Size = new System.Drawing.Size(36, 23);
            this.tbxChainedSmallest.TabIndex = 24;
            // 
            // tbxChainedLargest
            // 
            this.tbxChainedLargest.Location = new System.Drawing.Point(145, 277);
            this.tbxChainedLargest.Name = "tbxChainedLargest";
            this.tbxChainedLargest.Size = new System.Drawing.Size(36, 23);
            this.tbxChainedLargest.TabIndex = 25;
            // 
            // lblIntersectingPhrasesInstructions
            // 
            this.lblIntersectingPhrasesInstructions.AutoSize = true;
            this.lblIntersectingPhrasesInstructions.Location = new System.Drawing.Point(12, 341);
            this.lblIntersectingPhrasesInstructions.Name = "lblIntersectingPhrasesInstructions";
            this.lblIntersectingPhrasesInstructions.Size = new System.Drawing.Size(283, 45);
            this.lblIntersectingPhrasesInstructions.TabIndex = 27;
            this.lblIntersectingPhrasesInstructions.Text = "This will create multiple files in 100 verse batches, \r\neach of which is compared" +
    " with the rest of the Bible.\r\nThey can be large.";
            // 
            // lblChainedPhrasalConcordanceInstruction
            // 
            this.lblChainedPhrasalConcordanceInstruction.AutoSize = true;
            this.lblChainedPhrasalConcordanceInstruction.Location = new System.Drawing.Point(12, 215);
            this.lblChainedPhrasalConcordanceInstruction.Name = "lblChainedPhrasalConcordanceInstruction";
            this.lblChainedPhrasalConcordanceInstruction.Size = new System.Drawing.Size(267, 30);
            this.lblChainedPhrasalConcordanceInstruction.TabIndex = 28;
            this.lblChainedPhrasalConcordanceInstruction.Text = "This will create two files per length, maximum 54.\r\nThese can be large (on the or" +
    "der of 100 MB).";
            // 
            // lblChainedSmallestLength
            // 
            this.lblChainedSmallestLength.AutoSize = true;
            this.lblChainedSmallestLength.Location = new System.Drawing.Point(187, 251);
            this.lblChainedSmallestLength.Name = "lblChainedSmallestLength";
            this.lblChainedSmallestLength.Size = new System.Drawing.Size(91, 15);
            this.lblChainedSmallestLength.TabIndex = 29;
            this.lblChainedSmallestLength.Text = "Smallest Length";
            // 
            // lblChainedLargestLength
            // 
            this.lblChainedLargestLength.AutoSize = true;
            this.lblChainedLargestLength.Location = new System.Drawing.Point(187, 280);
            this.lblChainedLargestLength.Name = "lblChainedLargestLength";
            this.lblChainedLargestLength.Size = new System.Drawing.Size(85, 15);
            this.lblChainedLargestLength.TabIndex = 30;
            this.lblChainedLargestLength.Text = "Largest Length";
            // 
            // lblIntersectingMinimumVerseID
            // 
            this.lblIntersectingMinimumVerseID.AutoSize = true;
            this.lblIntersectingMinimumVerseID.Location = new System.Drawing.Point(161, 392);
            this.lblIntersectingMinimumVerseID.Name = "lblIntersectingMinimumVerseID";
            this.lblIntersectingMinimumVerseID.Size = new System.Drawing.Size(104, 15);
            this.lblIntersectingMinimumVerseID.TabIndex = 31;
            this.lblIntersectingMinimumVerseID.Text = "Minimum Verse ID";
            // 
            // lblIntersectingMaximumVerseID
            // 
            this.lblIntersectingMaximumVerseID.AutoSize = true;
            this.lblIntersectingMaximumVerseID.Location = new System.Drawing.Point(161, 421);
            this.lblIntersectingMaximumVerseID.Name = "lblIntersectingMaximumVerseID";
            this.lblIntersectingMaximumVerseID.Size = new System.Drawing.Size(106, 15);
            this.lblIntersectingMaximumVerseID.TabIndex = 32;
            this.lblIntersectingMaximumVerseID.Text = "Maximum Verse ID";
            // 
            // btnWriteKJVMobySynonyms
            // 
            this.btnWriteKJVMobySynonyms.Location = new System.Drawing.Point(96, 113);
            this.btnWriteKJVMobySynonyms.Name = "btnWriteKJVMobySynonyms";
            this.btnWriteKJVMobySynonyms.Size = new System.Drawing.Size(101, 45);
            this.btnWriteKJVMobySynonyms.TabIndex = 33;
            this.btnWriteKJVMobySynonyms.Text = "Write KJV Moby Synonyms";
            this.btnWriteKJVMobySynonyms.UseVisualStyleBackColor = true;
            this.btnWriteKJVMobySynonyms.Click += new System.EventHandler(this.btnWriteKJVMobySynonyms_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.DataSource = this.dataObjectsBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(790, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(222, 543);
            this.dataGridView1.TabIndex = 34;
            // 
            // dataObjectsBindingSource
            // 
            this.dataObjectsBindingSource.DataSource = typeof(Data.DataObjects);
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 830);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnWriteKJVMobySynonyms);
            this.Controls.Add(this.lblIntersectingMaximumVerseID);
            this.Controls.Add(this.lblIntersectingMinimumVerseID);
            this.Controls.Add(this.lblChainedLargestLength);
            this.Controls.Add(this.lblChainedSmallestLength);
            this.Controls.Add(this.lblChainedPhrasalConcordanceInstruction);
            this.Controls.Add(this.lblIntersectingPhrasesInstructions);
            this.Controls.Add(this.tbxChainedLargest);
            this.Controls.Add(this.tbxChainedSmallest);
            this.Controls.Add(this.tbxIntersectingMinimum);
            this.Controls.Add(this.tbxIntersectingMaximum);
            this.Controls.Add(this.lblExistingIntersectingPhrasesBoundaries);
            this.Controls.Add(this.lblExistingChainedConcordanceBoundaries);
            this.Controls.Add(this.btnCreateChainedPhrasalConcordance);
            this.Controls.Add(this.btnCreateIntersectingPhrases);
            this.Controls.Add(this.clbProcessedFiles);
            this.Controls.Add(this.clbOriginalFiles);
            this.Controls.Add(this.btnSemanticGroups);
            this.Controls.Add(this.btnBibleLinks);
            this.Controls.Add(this.lbxPericopeSearchResults);
            this.Controls.Add(this.gbxPericopeSearchLogic);
            this.Controls.Add(this.btnSearchPericope);
            this.Controls.Add(this.clbPericopeSearchTerms);
            this.Controls.Add(this.rtbText);
            this.Controls.Add(this.lbBooks);
            this.Name = "Welcome";
            this.Text = "Welcome to Bible Searcher";
            this.Load += new System.EventHandler(this.Welcome_Load);
            this.gbxPericopeSearchLogic.ResumeLayout(false);
            this.gbxPericopeSearchLogic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataObjectsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox lbBooks;
        private RichTextBox rtbText;
        private CheckedListBox clbPericopeSearchTerms;
        private Button btnSearchPericope;
        private RadioButton rbnPericopeSearchAny;
        private RadioButton rbnPericopeSearchAll;
        private GroupBox gbxPericopeSearchLogic;
        private ListBox lbxPericopeSearchResults;
        private Button btnBibleLinks;
        private Button btnSemanticGroups;
        private CheckedListBox clbOriginalFiles;
        private CheckedListBox clbProcessedFiles;
        private Button btnCreateIntersectingPhrases;
        private Button btnCreateChainedPhrasalConcordance;
        private Label lblExistingChainedConcordanceBoundaries;
        private Label lblExistingIntersectingPhrasesBoundaries;
        private TextBox tbxIntersectingMaximum;
        private TextBox tbxIntersectingMinimum;
        private TextBox tbxChainedSmallest;
        private TextBox tbxChainedLargest;
        private Label lblIntersectingPhrasesInstructions;
        private Label lblChainedPhrasalConcordanceInstruction;
        private Label lblChainedSmallestLength;
        private Label lblChainedLargestLength;
        private Label lblIntersectingMinimumVerseID;
        private Label lblIntersectingMaximumVerseID;
        private Button btnWriteKJVMobySynonyms;
        private DataGridView dataGridView1;
        private BindingSource dataObjectsBindingSource;
    }
}