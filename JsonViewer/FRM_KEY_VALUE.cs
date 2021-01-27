using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace JsonViewer
{
    public partial class FRM_KEY_VALUE : Form
    {
        List<ClassOptions> objOption;
        public FRM_KEY_VALUE()
        {
            InitializeComponent();
        }

        private void FRM_KEY_VALUE_Load(object sender, EventArgs e)
        {
            listView1.Items.Add(new ListViewItem(new string[] { "contenttype", "application/json" }));
            listView1.Items.Add(new ListViewItem(new string[] { "APIKey", "b73355f36b26d872c166b3a4bf0903c6" }));
        }

        private void radListView1_SelectedItemChanged(object sender, EventArgs e)
        {

        }
        private Boolean isDuplicate(string key,string value)
        {
            string itemKey=string.Empty;
            string itemValue = string.Empty;

            string checkkey = string.Empty;
            string checkvalue = string.Empty;
            Boolean isResult = true;
            if ((!String.IsNullOrEmpty(textBox1.Text)) && (!String.IsNullOrEmpty(textBox2.Text)))
            {
                checkkey = textBox1.Text;
                checkvalue = textBox2.Text;
                for (int i = 0; i <= listView1.Items.Count - 1; i++)
                {
                    itemKey=listView1.Items[i].SubItems[0].Text;
                    itemValue = listView1.Items[i].SubItems[1].Text;
                    if (isResult)
                    {
                        if (string.Equals(itemKey.ToUpper(), checkkey.ToUpper()) && string.Equals(itemValue.ToUpper(), checkvalue.ToUpper()))
                        {
                            isResult = false;
                        }
                    }
                }
            }
            return isResult;
        }
        private void BTN_ADD_Click(object sender, EventArgs e)
        {
            if (isDuplicate(textBox1.Text, textBox2.Text))
            {
                if((!String.IsNullOrEmpty(textBox1.Text)) && (!String.IsNullOrEmpty(textBox2.Text)) ){
                    listView1.Items.Add(new ListViewItem(new string[] { textBox1.Text, textBox2.Text }));
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox1.Focus();
                }
                else if (String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please input key", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                }
                else if (String.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Please input key", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                }
                else
                {
                    MessageBox.Show("Please validate value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BTN_DELETE_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count>0)
            {
                listView1.SelectedItems[0].Remove();
            }else
            {
                MessageBox.Show("Please Select Item", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
                textBox2.Text = listView1.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void FRM_KEY_VALUE_FormClosed(object sender, FormClosedEventArgs e)
        {
            string itemKey = string.Empty;
            string itemValue = string.Empty;
            ClassOptions itemOption=null;
            objOption = new List<ClassOptions>();
            for (int i = 0; i <= listView1.Items.Count - 1; i++)
                {
                    itemKey = listView1.Items[i].SubItems[0].Text;
                    itemValue = listView1.Items[i].SubItems[1].Text;
                    itemOption = new ClassOptions();
                    itemOption.Key = itemKey;
                    itemOption.Value = itemValue;
                    objOption.Add(itemOption);
                }
            this.Tag = objOption;
        }
    }
}
