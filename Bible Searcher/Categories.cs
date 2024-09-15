namespace Bible_Searcher
{
    public partial class Categories : Form
    {
        Data.Welcome data;

        public Categories(ref Data.Welcome data)
        {
            InitializeComponent();

            this.data = data;

            if (rbnWords.Checked)
            {
                lblNewWord.Text = data.dataobjects.categories.NextWord();
            }
            else if (rbnPhrases.Checked)
            {
                lblNewWord.Text = data.dataobjects.categories.NextPhrase();
            }
        }

        private void pnl_DragDrop(object sender, DragEventArgs e)
        {
            string strText = ((string)e.Data.GetData(DataFormats.Text)).ToLower();
            string strCategory = ((Panel)sender).Name.ToString().Remove(0, 3).ToLower();

            if (strCategory == "relativecomparatives")
            {
                strCategory = "relative comparatives";
            }

            if (strCategory == "actionverbs")
            {
                strCategory = "action verbs";
            }

            data.dataobjects.categories.strCategory = strCategory;
            lblUndo.Text = strText;

            if (rbnWords.Checked == true)
            {
                data.dataobjects.categories.AddWord(strText, strCategory);
                data.dataobjects.categories.RemoveWord(strText, "uncategorized");
                lblNewWord.Text = data.dataobjects.categories.NextWord();
            }
            else
            {
                data.dataobjects.categories.AddPhrase(strText, strCategory);
                data.dataobjects.categories.RemovePhrase(strText, "uncategorized");
                lblNewWord.Text = data.dataobjects.categories.NextPhrase();
            }
        }

        private void pnl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void lblNewWord_MouseDown(object sender, MouseEventArgs e)
        {
            DoDragDrop(lblNewWord.Text, DragDropEffects.Copy);
        }

        private void lblUndo_Click(object sender, EventArgs e)
        {
            string strWordUndo = lblUndo.Text;

            if (strWordUndo != "Undo")
            {
                if (rbnWords.Checked == true)
                {
                    data.dataobjects.categories.RemoveWord(strWordUndo, data.dataobjects.categories.strCategory);
                    data.dataobjects.categories.AddWord(strWordUndo, "uncategorized");
                }
                else
                {
                    data.dataobjects.categories.RemovePhrase(strWordUndo, data.dataobjects.categories.strCategory);
                    data.dataobjects.categories.AddPhrase(strWordUndo, "uncategorized");
                }

                lblUndo.Text = "Undo";
                lblNewWord.Text = strWordUndo;
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {   
            if (rbnWords.Checked)
            {
                lblNewWord.Text = data.dataobjects.categories.NextWord();

            }
            else if (rbnPhrases.Checked)
            {
                lblNewWord.Text = data.dataobjects.categories.NextPhrase();
            }

            lblUndo.Text = "Undo";
        }

        private void WordCategories_FormClosing(object sender, FormClosingEventArgs e)
        {
            data.dataobjects.categories.WriteWords();
            data.dataobjects.categories.WritePhrases();
        }
    }
}
