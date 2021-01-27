using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BjSTools.File;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
namespace JsonViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region UI events

        private void btnImportTextPaste_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                    txtJson.Text = Clipboard.GetText();
            }
            catch { }
        }
      

    private void btnImportFileSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse json Files",

                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                DefaultExt = "json",
                Filter = "all files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtJson.Clear();
                txtImportFile.Text = openFileDialog1.FileName;
                string data = String.Empty;
                try
                {
                    using (StreamReader s = new StreamReader(txtImportFile.Text))
                    {
                        data = s.ReadToEnd();
                    }
                    txtJson.Text = data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void btnImportUrlLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string content = WGet(txtImportUrl.Text);
                if (String.IsNullOrEmpty(content))
                    throw new Exception("No data received!");

                LoadData(content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImportFileLoad_Click(object sender, EventArgs e)
        {
            string data = String.Empty;
            try
            {
                using (StreamReader s = new StreamReader(txtImportFile.Text))
                {
                    data = s.ReadToEnd();
                }
                 txtJson.Text = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void btnImportTextLoad_Click(object sender, EventArgs e)
        {
            LoadData(txtJson.Text);
        }

        #endregion

        private void LoadData(string data)
        {
          
            BjSJsonObject json = new BjSJsonObject(data);
           txtchild.Text= json.Count().ToString();

            lstJsonTree.Nodes.Clear();
            if (json != null)
            {
                foreach (BjSJsonObjectMember member in json)
                {
                }
            }
            
                TreeNode root = ConvertToTreeNode(json, String.Empty);
            foreach (TreeNode node in root.Nodes)
            {
                lstJsonTree.Nodes.Add(node);
                node.Expand();
            }

            txtJson.Text = json.ToJsonString(false);

            tabControl1.SelectedIndex = 1;
        }

        #region Helpers

        private TreeNode ConvertToTreeNode(BjSJsonObject obj, string name)
        {
            TreeNode root = new TreeNode(String.Format(String.IsNullOrEmpty(name) ? "Object{{{1}}}" : "\"{0}\" : Object{{{1}}}", name, obj.Count));
            root.ImageIndex = 0;
            root.SelectedImageIndex = 0;
            root.Tag = name;
            root.ToolTipText = name;

            foreach (BjSJsonObjectMember member in obj)
            {
                switch (member.ValueKind)
                {
                    case BjSJsonValueKind.Object:
                        root.Nodes.Add(ConvertToTreeNode(member.Value as BjSJsonObject, member.Name));
                        break;
                    case BjSJsonValueKind.Array:
                        root.Nodes.Add(ConvertToTreeNode(member.Value as BjSJsonArray, member.Name));
                        break;
                    case BjSJsonValueKind.String:
                        TreeNode tnString = new TreeNode(String.Format("\"{0}\" : \"{1}\"", member.Name, member.Value)) { ImageIndex = 2, SelectedImageIndex = 2 };
                        tnString.Tag = member.Name;
                        tnString.ToolTipText = member.Name;
                        root.Nodes.Add(tnString);
                        break;
                    case BjSJsonValueKind.Number:
                        TreeNode tnNumber = new TreeNode(String.Format("\"{0}\" : \"{1}\"", member.Name, member.Value)) { ImageIndex = 3, SelectedImageIndex = 3 };
                        tnNumber.Tag = member.Name;
                        tnNumber.ToolTipText = member.Name;
                        root.Nodes.Add(tnNumber);
                        break;
                    case BjSJsonValueKind.Boolean:
                        TreeNode tnBoolean = new TreeNode(String.Format("\"{0}\" : \"{1}\"", member.Name, member.Value)) { ImageIndex = 4, SelectedImageIndex = 4 };
                        tnBoolean.Tag = member.Name;
                        tnBoolean.ToolTipText = member.Name;
                        root.Nodes.Add(tnBoolean);
                        break;
                    case BjSJsonValueKind.Null:
                        TreeNode tnNull = new TreeNode(String.Format("\"{0}\" : \"{1}\"", member.Name, member.Value)) { ImageIndex = 5, SelectedImageIndex = 5 };
                        tnNull.Tag = member.Name;
                        tnNull.ToolTipText = member.Name;
                        root.Nodes.Add(tnNull);
                        break;
                    default:
                        break;
                }
            }

            return root;
        }
        private TreeNode ConvertToTreeNode(BjSJsonArray arr, string name)
        {
            TreeNode root = new TreeNode(String.Format(String.IsNullOrEmpty(name) ? "Array[{1}]" : "\"{0}\" : Array[{1}]", name, arr.Count));
            root.ImageIndex = 1;
            root.SelectedImageIndex = 1;
            root.Tag = name;
            root.ToolTipText = name;
            for (int i = 0; i < arr.Count; i++)
            {
                var obj = arr[i];
                switch (BjSJsonHelper.GetValueKind(obj))
                {
                    case BjSJsonValueKind.Object:
                        root.Nodes.Add(ConvertToTreeNode(obj as BjSJsonObject, i.ToString()));
                        break;
                    case BjSJsonValueKind.Array:
                        root.Nodes.Add(ConvertToTreeNode(obj as BjSJsonArray, i.ToString()));
                        break;
                    case BjSJsonValueKind.String:
                        TreeNode tnString = new TreeNode(String.Format("{0} : \"{1}\"", i, obj)) { ImageIndex = 2, SelectedImageIndex = 2 };
                        tnString.Tag = name;
                        tnString.ToolTipText = name;
                        root.Nodes.Add(tnString);
                        break;
                    case BjSJsonValueKind.Number:
                        TreeNode tnNumber = new TreeNode(String.Format("{0} : \"{1}\"", i, obj)) { ImageIndex = 3, SelectedImageIndex = 3 };
                        tnNumber.Tag = name;
                        tnNumber.ToolTipText = name;
                        root.Nodes.Add(tnNumber);
                        break;
                    case BjSJsonValueKind.Boolean:
                        TreeNode tnBoolean = new TreeNode(String.Format("{0} : \"{1}\"", i, obj)) { ImageIndex = 4, SelectedImageIndex = 4 };
                        tnBoolean.Tag = name;
                        tnBoolean.ToolTipText = name;
                        root.Nodes.Add(tnBoolean);
                        break;
                    case BjSJsonValueKind.Null:
                        TreeNode tnNull = new TreeNode(String.Format("{0} : \"{1}\"", i, obj)) { ImageIndex = 5, SelectedImageIndex = 5 };
                        tnNull.Tag = name;
                        tnNull.ToolTipText = name;
                        root.Nodes.Add(tnNull);
                        break;
                    default:
                        break;
                }
            }

            return root;
        }

        public static string WGet(string url)
        {
            string result = String.Empty;

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 (X11; Linux x86_64; rv:28.0) Gecko/20100101 Firefox/28.0";
            req.Timeout = 10000;
            req.Method = "GET";

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            if (res.StatusCode == HttpStatusCode.OK)
            {
                StreamReader s = new StreamReader(res.GetResponseStream());
                result = s.ReadToEnd();
            }
            else
                throw new Exception(String.Format("{0}: {1}", res.StatusCode, res.StatusDescription));

            res.Close();

            return result;
        }

        #endregion

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtJson.Clear();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtJson.Text)) {
                LoadData(txtJson.Text);
                txtallcolumn.Text = GetNodesCount(lstJsonTree.Nodes).ToString();
            }
            else if (!string.IsNullOrEmpty(txtImportUrl.Text))
            {
                try
                { 
                    Uri uriResult;
                    bool result = false;
                    string uriname = string.Empty;
                    if (!string.IsNullOrEmpty(txtImportUrl.Text))
                    {
                        uriname = txtImportUrl.Text;
                        result = Uri.TryCreate(uriname, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    }
                    if (result) {

                        WebRequest WR;
                        WebResponse WS;
                        StreamReader SR;
                        string ResultReadtoEnd;
                        string Identifier;
                        String UR = txtImportUrl.Text; // "https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profiles";
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        WR = WebRequest.Create(UR);
                        if (objOption != null)
                        {
                            foreach (ClassOptions item in objOption)
                            {
                                string sKey = string.Empty;
                                string sValue = string.Empty;
                                if (item != null)
                                {
                                    sKey = item.Key;
                                    sValue = item.Value;
                                    if (sKey.ToLower() == "contenttype")
                                    {
                                        WR.ContentType = sValue;
                                    } else
                                    {
                                        WR.Headers.Add(sKey, sValue);
                                    }
                                }
                            }
                            WR.Method = "GET";
                        }
                        else
                        {
                            WR.ContentType = "application/json";
                            WR.Headers.Add("APIKey", "b73355f36b26d872c166b3a4bf0903c6");
                            WR.Method = "GET";
                        }
                        //https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profiles?updatedSince=2020-10-01&size=30
                        if (WR != null)
                        {
                            WS = WR.GetResponse();
                            if (WS != null)
                            {
                                SR = new System.IO.StreamReader(WS.GetResponseStream());
                                if (SR != null)
                                {
                                    ResultReadtoEnd = SR.ReadToEnd();
                                    if (!string.IsNullOrEmpty(ResultReadtoEnd))
                                    {
                                        LoadData(ResultReadtoEnd);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                txtJson.Focus();
                MessageBox.Show("Please input json message!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string fnURLtoValue(String inURL)
        {
            string sResult = string.Empty;
            WebRequest WR;
            WebResponse WS;
            StreamReader SR;
            string ResultReadtoEnd;
            String UR = inURL; // "https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profiles";
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WR = WebRequest.Create(UR);
            if (objOption != null)
            {
                foreach (ClassOptions item in objOption)
                {
                    string sKey = string.Empty;
                    string sValue = string.Empty;
                    if (item != null)
                    {
                        sKey = item.Key;
                        sValue = item.Value;
                        if (sKey.ToLower() == "contenttype")
                        {
                            WR.ContentType = sValue;
                        }
                        else
                        {
                            WR.Headers.Add(sKey, sValue);
                        }
                    }
                }
                WR.Method = "GET";
            }
            else
            {
                WR.ContentType = "application/json";
                WR.Headers.Add("APIKey", "b73355f36b26d872c166b3a4bf0903c6");
                WR.Method = "GET";
            }

            if (WR != null)
            {
                WS = WR.GetResponse();
                if (WS != null)
                {
                    SR = new System.IO.StreamReader(WS.GetResponseStream());
                    if (SR != null)
                    {
                        ResultReadtoEnd = SR.ReadToEnd();
                        if (!string.IsNullOrEmpty(ResultReadtoEnd))
                        {
                            sResult = (ResultReadtoEnd);
                        }
                    }
                }
            }

            return sResult;
        }
        private void btnImportFileSelect_Click_1(object sender, EventArgs e)
        {
           if (!string.IsNullOrEmpty(txtImportFile.Text))
            {
                if (System.IO.File.Exists(txtImportFile.Text))
                {
                    string data = String.Empty;
                    try
                    {
                        using (StreamReader s = new StreamReader(txtImportFile.Text))
                        {
                            data = s.ReadToEnd();
                        }
                        txtJson.Text = data;
                    }catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("file not exits !");
                }
            }
            else
            {
                MessageBox.Show("Please input file name !");
            }
        }
        private void FindCheckedNodes(
         List<TreeNode> checked_nodes, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                // Add this node.
                if (node.Checked) checked_nodes.Add(node);

                // Check the node's descendants.
                FindCheckedNodes(checked_nodes, node.Nodes);
            }
        }
        private List<TreeNode> CheckedNodes(TreeView trv)
        {
            List<TreeNode> checked_nodes = new List<TreeNode>();
            FindCheckedNodes(checked_nodes, trv.Nodes);
            return checked_nodes;
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string pathvalue = string.Empty;
            string sFormatSelect = "{0} {1} PATH '{2}'";
            string sConvertJson = string.Empty;
            string sColumn = string.Empty;
            string[] spLine = null;
            string sSQLJSon = string.Empty;
            string result = GetCheckedNodesCount(lstJsonTree.Nodes);
            sSQLJSon= " SELECT * FROM CPC_NOTIFY_MESSAGE , JSON_TABLE(MESSAGE,'$' COLUMNS ( {0} ) )";


            if (!string.IsNullOrEmpty(result))
            {
            spLine = result.Split(new[] { Environment.NewLine },StringSplitOptions.None);
            if (spLine.Length > 0)
            {
                for (int i = 0; i <= spLine.Length - 1; i++)
                {
                        if (!string.IsNullOrEmpty(spLine[i]))
                        {
                            pathvalue=(SetReplaceKey(spLine[i]));
                            sConvertJson = string.Format(sFormatSelect, "A_" + i.ToString(), "VARCHAR2(50)", "$." + pathvalue.ToString());
                            if (!string.IsNullOrEmpty(sColumn)) sColumn += ",";
                            sColumn  += sConvertJson;
                        }
                    }
            }

                txtsql.Clear();
                txtsql.Text = string.Format(sSQLJSon, sColumn);
                if (!string.IsNullOrEmpty(txtsql.Text))
                {
                    tabControl1.SelectedIndex = 2;
                }
            }
        }
        private string SetReplaceKey(string sResult)
        {
            string sReturnKey = string.Empty;
            string sReturn = string.Empty;
            string[] spBlack = null;
            string sLine = string.Empty;
            string[] spKey = null;
            string[] spType = null;
            string sKey = string.Empty;
            Boolean isCheck = false;
            string sType = string.Empty;
            string sFormatSelect = "{0} {1} {2} '{3}'";
            string sKeyOrigin = string.Empty;
            int iType = 0;
            Guid g = Guid.NewGuid();
            var keyid = string.Empty;
            try
            {
                spBlack  = sResult.Split('\\');
                if (spBlack.Length > 0)
                {
                    for (int i=0; i <= spBlack.Length - 1; i++)
                    {
                        sLine = spBlack[i];
                        if (!string.IsNullOrEmpty(sLine))
                        {
                            sType = string.Empty;
                            spType = sLine.Split('#');
                            spKey = sLine.Split(':');
                            if (spType.Length>1) sType =spType[1];
                            if (!string.IsNullOrEmpty(sType))
                            {
                                if (char.IsNumber(sType,0))
                                {
                                    iType = Convert.ToInt16(sType);
                                }
                            }
                            if (spKey.Length > 0)
                            {
                                sKey = spKey[0].ToString().Replace("\"", string.Empty).Trim();
                                isCheck = char.IsNumber(sKey, 0);
                                if (!isCheck)
                                {
                                    if (!string.IsNullOrEmpty(sReturnKey)) sReturnKey += ".";
                                    sReturnKey += sKey;
                                    if (!string.IsNullOrEmpty(sKeyOrigin)) sKeyOrigin += "_";
                                    sKeyOrigin += sKey;
                                }
                                else
                                {
                                    sKeyOrigin += "1";
                                    sReturnKey += "[*]";
                                }
                            }
                        } 
                    }
                    if (char.IsNumber(sKeyOrigin.Substring(sKeyOrigin.Length-1), 0))
                    {
                        g = Guid.NewGuid();
                        keyid = g.ToString();
                    }else
                    {
                        keyid = sKeyOrigin;
                    }
                    if (iType > 1)
                    {
                        sReturn = string.Format(sFormatSelect, sKeyOrigin, "VARCHAR2(50)","PATH", "$." + sReturnKey.ToString());
                    }else
                    {
                        sReturn = string.Format(sFormatSelect, sKeyOrigin, "VARCHAR2(2000)", "FORMAT JSON PATH", "$." + sReturnKey.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return sReturn;
        }
        public int GetNodesCount(TreeNodeCollection nodes)
        {
            int checkedNodes = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                   checkedNodes++;
                if (node.Nodes.Count > 0)
                    checkedNodes += GetNodesCount(node.Nodes);
            }

            return checkedNodes;

        }

        public string GetCheckedNodesCount( TreeNodeCollection nodes)
        {
            int checkedNodes = 0;
            string result = "";
            for (int i = 0; i < nodes.Count; i++)
            {
                TreeNode node = nodes[i];
                if (node.Checked) {
                    if (!String.IsNullOrEmpty(result)) result += Environment.NewLine;
                    result += node.FullPath.ToString() + "#"+ node.ImageIndex.ToString();
                    checkedNodes++;
            }
                if (node.Nodes.Count > 0)
                    if (!String.IsNullOrEmpty(result)) result += Environment.NewLine;
                    result += GetCheckedNodesCount(node.Nodes);
            }

            return result;

        }

        private void keyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstJsonTree.SelectedNode.Checked = true;
            lstJsonTree.SelectedNode.ForeColor = Color.Red;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(lstJsonTree.SelectedNode.Text))
            {
                Clipboard.SetText(lstJsonTree.SelectedNode.Text);
            }
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            string pathvalue = string.Empty;
            string sConvertJson = string.Empty;
            string sColumn = string.Empty;
            string[] spLine = null;
            string sSQLJSon = string.Empty;
            string result = GetCheckedNodesCount(lstJsonTree.Nodes);
            sSQLJSon = " SELECT * FROM {0} , JSON_TABLE({1},'$' COLUMNS ( {2} ) )";


            if (!string.IsNullOrEmpty(result))
            {
                spLine = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                txtselectcolumn.Text = spLine.Length.ToString();
                if (spLine.Length > 0)
                {
                    for (int i = 0; i <= spLine.Length - 1; i++)
                    {
                        if (!string.IsNullOrEmpty(spLine[i]))
                        {
                            pathvalue = (SetReplaceKey(spLine[i]));
                            sConvertJson = pathvalue;
                            if (!string.IsNullOrEmpty(sColumn)) sColumn += ",";
                            sColumn += sConvertJson;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(txtTable.Text) && (!string.IsNullOrEmpty(txtColumn.Text)) && (!string.IsNullOrEmpty(sColumn)))
                {
                    txtsql.Clear();
                    txtsql.Text = string.Format(sSQLJSon,txtTable.Text ,txtColumn.Text, sColumn);
                    if (!string.IsNullOrEmpty(txtsql.Text)) tabControl1.SelectedIndex = 2;
                }
                else
                {
                    MessageBox.Show("Please verify conditions",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    if (string.IsNullOrEmpty(txtTable.Text)) txtTable.Focus();
                    if (string.IsNullOrEmpty(txtColumn.Text)) txtColumn.Focus();
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            lstJsonTree.ExpandAll();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            lstJsonTree.CollapseAll();
        }
        public void CheckAllNodes(TreeNodeCollection nodes,Boolean isChecked)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = isChecked;
                CheckChildren(node, isChecked);
            }
        }
 
        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CheckAllNodes(lstJsonTree.Nodes,true);
            txtselectcolumn.Text = GetNodesCount(lstJsonTree.Nodes).ToString();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CheckAllNodes(lstJsonTree.Nodes,false);
            txtselectcolumn.Text = "0";
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


        private void toolStripButton5_Click(object sender, EventArgs e)
        {

            string result = GetCheckedNodesCount(lstJsonTree.Nodes);
            if (!string.IsNullOrEmpty(result))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Delimited|*.csv|text file|*.txt|log file|*.log|all file|*.*";
                saveFileDialog1.Title = "save file name";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                string[] lines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                string docPath =   Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
               // using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, saveFileDialog1.FileName)))
                using (StreamWriter outputFile = new StreamWriter( saveFileDialog1.FileName))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
                }
            }
}

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                System.Diagnostics.Process.Start(Application.StartupPath);
            }
            if (e.KeyCode == Keys.F10)
            {
                btnLoad_Click(sender, e);
            }
            if  (e.KeyCode== Keys.F12)
            {
                btnOption.Visible = !(btnOption.Visible);
                buttonx2.Visible = !(buttonx2.Visible);
                buttonx3.Visible = !(buttonx3.Visible);
                btnLoad.Visible = !(btnLoad.Visible);
                btnClear.Visible = !(btnClear.Visible);
                btnValidateUrl.Visible = !(btnValidateUrl.Visible);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnUrl_Click(object sender, EventArgs e)
        {
            Uri uriResult;
            bool result = false;
            string uriname = string.Empty;
            if (!string.IsNullOrEmpty(txtImportUrl.Text))
            {
                uriname = txtImportUrl.Text;
                result  = Uri.TryCreate(uriname, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            if (result)
            {
                MessageBox.Show("is URL API", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }else
            {
                MessageBox.Show("is not URL API", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        List<ClassOptions> objOption;

        private void btnOption_Click(object sender, EventArgs e)
        {
            FRM_KEY_VALUE frm;
            try
            {
                frm = new FRM_KEY_VALUE();
                frm.ShowDialog();
                if (frm.Tag != null)
                {
                    objOption = (List<ClassOptions>)frm.Tag;
                }
            }
            catch ( Exception ex)
            {

            }
            finally
            {
                frm = null;
            }
        }
        public static DialogResult InputBox(List<string> listColumn, string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            ComboBox cmd = new ComboBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();


            form.Text = title;
            label.Text = promptText;
            if (listColumn != null)
            {
                foreach(string item in listColumn)
                {
                    if (item != null)
                    {
                        cmd.Items.Add(item);
                    }
                }
            }
            if (cmd.Items.Count > 0) cmd.SelectedIndex = 0;
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            cmd.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            cmd.Anchor = cmd.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, cmd, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            if (cmd.SelectedIndex > -1)
            {
                value = cmd.Items[cmd.SelectedIndex].ToString();
            }
            return dialogResult;
        }
        List<String> arrayColumn;

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassDB objDB;

            String sMappingName;
            TreeNode tn = lstJsonTree.SelectedNode;
            string cUser_Login = string.Empty;
            string cPassword_Login = string.Empty;
            string cHost = string.Empty;
            string cPort = string.Empty;
            string cInstance = string.Empty;
            string cTable = string.Empty;
            string informattns = "User Id={0}; password={1}; Data Source={2}:{3}/{4}";
            string intns = string.Empty;
            string sSQL = string.Empty;
            string sFormatSQL = "select column_name from cols where lower(table_name) = '{0}'";
            string sColumnName = string.Empty;
            DataTable oTable;
            Public_Service._Public objConnectDB;
            string itemfull=string.Empty;
            string itemnode= string.Empty;
            string sJsonKey= string.Empty;
            string[] spnode = null;
            if (tn != null )
            {
                if (tn.Nodes.Count == 0)
                {
                    if (arrayColumn != null)
                    {
                        sMappingName =  "Field Database";
                        if (InputBox(arrayColumn, "New Mapping", "New document name:", ref sMappingName) == DialogResult.OK)
                        {
                            itemfull = tn.FullPath;
                            itemnode = tn.Text;
                            if (!string.IsNullOrEmpty(itemnode))
                            {
                                spnode = itemnode.Split(new char[] {':'});
                                if (spnode != null)
                                {
                                    if (spnode.Length > 0)
                                    {
                                        sJsonKey = spnode[0];
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(sJsonKey) && (!string.IsNullOrEmpty(sMappingName)) && (!string.IsNullOrEmpty(itemfull)))
                            {
                                listView1.Items.Add(new ListViewItem(new string[] { sJsonKey, sMappingName ,itemfull ,tn.Level.ToString(),""}));
                            }
                        }
                    }else
                    {
                        FRM_CONNECT_DB frm = null;
                        frm = new FRM_CONNECT_DB();
                        frm.ShowDialog();
                        if (frm.Tag != null)
                        {
                            objDB = (ClassDB)frm.Tag;
                            if (objDB != null)
                            {
                                cUser_Login = objDB.User;
                                cPassword_Login = objDB.Password;
                                cHost = objDB.Host;
                                cPort = objDB.Port;
                                cInstance = objDB.Instance;
                                cTable = objDB.Table;

                                objConnectDB = new Public_Service._Public();
                                intns = string.Format(informattns, cUser_Login, cPassword_Login, cHost, cPort, cInstance);
                                sSQL = string.Format(sFormatSQL, cTable);
                                oTable = objConnectDB.SelectDataTNS(intns, sSQL);
                                if (oTable != null)
                                {
                                    if (oTable.Rows.Count > 0)
                                    {
                                        arrayColumn = new List<String>();
                                        for (int i = 0; i <= oTable.Rows.Count - 1; i++)
                                        {
                                            if (oTable.Rows[i]["column_name"] != null)
                                            {
                                                sColumnName = (string)oTable.Rows[i]["column_name"];
                                                arrayColumn.Add(sColumnName);
                                            }
                                        }
                                        sMappingName = "Field Database";
                                        if (InputBox(arrayColumn, "New Mapping", "New document name:", ref sMappingName) == DialogResult.OK)
                                        {
                                            itemfull = tn.FullPath;
                                            itemnode = tn.Text;
                                            if (!string.IsNullOrEmpty(itemnode))
                                            {
                                                spnode = itemnode.Split(new char[] { ':' });
                                                if (spnode != null)
                                                {
                                                    if (spnode.Length > 0)
                                                    {
                                                        sJsonKey = spnode[0];
                                                    }
                                                }
                                            }
                                         if (!string.IsNullOrEmpty(sJsonKey) && (!string.IsNullOrEmpty(sMappingName)) && (!string.IsNullOrEmpty(itemfull)))
                                            {
                                                listView1.Items.Add(new ListViewItem(new string[] { sJsonKey, sMappingName, itemfull, tn.Level.ToString(), "" }));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Node select not is child", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select node", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
      
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].Remove();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[2].Text);
            }
        }
        public TreeNode GetNodeFromPath(TreeNode node, string path)
        {
            TreeNode foundNode = null;
            foreach (TreeNode tn in node.Nodes)
            {
                if (tn.FullPath == path)
                {
                    return tn;
                }
                else if (tn.Nodes.Count > 0)
                {
                    foundNode = GetNodeFromPath(tn, path);
                }
                if (foundNode != null)
                    return foundNode;
            }
            return null;
        }
        TreeNode _NodeFound;
        public TreeNode SelectNode(string strNodeName)
        {
            _NodeFound = null;
          
            foreach (TreeNode MyNode in lstJsonTree.Nodes)
            {
                if (_NodeFound != null)
                {
                    lstJsonTree.SelectedNode = _NodeFound;
                    lstJsonTree.SelectedNode.BackColor = Color.Red;
                    break;
                }
                else
                    FindNodeByName(MyNode, strNodeName);
            }
            return _NodeFound;
        }
        List<ClassMapping>  listValue;
        List<String>  listSQL;
        private void RecurseNodes(TreeNodeCollection col, List<ClassFieldFind> objSQL)
        {
            string sNode = string.Empty;
            string sNodeFull = string.Empty;
            ClassMapping objMapping = null;
            string sSQLInsert = string.Empty;
            string[] spLevel = null;
            string[] spKeyvalue = null;
            string sID = string.Empty;
            string sSubID = string.Empty;
            string sKey = string.Empty;
            string sValue = string.Empty;
            string sNodeprevious = string.Empty;
            int  iArray;
            Boolean isArray=false;
            foreach (TreeNode tn in col)
            {
                foreach(ClassFieldFind item in objSQL)
                {
                    if (item != null)
                    {
                        sNode = tn.Text;
                        sNodeFull = tn.FullPath;
                        sKey = string.Empty;
                        sValue = string.Empty;
                        spLevel = sNodeFull.Split(new char[] { '\\' });
                        if ((sNode.ToLower().IndexOf(item.Key.ToLower()) > -1) && (tn.Level == item.Level))
                        {
                            for (int isp = 0; isp <= spLevel.Length - 1; isp++)
                            {
                                spKeyvalue = spLevel[isp].Split(new char[] { ':' });
                                if (spKeyvalue != null)
                                {
                                    if (spKeyvalue.Length > 0) sKey = spKeyvalue[0];
                                    if (spKeyvalue.Length > 1) sValue = spKeyvalue[1];

                                    isArray = int.TryParse(sKey.Replace("\"", ""), out iArray);
                                    if (isArray)
                                    {
                                        if (string.IsNullOrEmpty(sID))
                                        {
                                            sID = sKey;
                                        }
                                        else if (string.IsNullOrEmpty(sSubID))
                                        {
                                            sSubID = sKey;
                                        }
                                    }

                                }
                            }
                          
                            objMapping = new ClassMapping();
                            objMapping.Id = sID;
                            objMapping.SubId = sSubID;
                            objMapping.Value = sNode;
                            objMapping.Mapping = item.Mapping;
                            objMapping.IsMaster = item.Master;
                            listValue.Add(objMapping);


                        }
                    }
                }
                RecurseNodes(tn.Nodes, objSQL);
            }

        }
        private void FindNodeByName(TreeNode n, string strNodeName)
        {
            if (string.Compare(n.FullPath, strNodeName, true) == 0)
            {
                Console.WriteLine("Node Found: {0}", n.FullPath);
                _NodeFound = n;
            }

            foreach (TreeNode aNode in n.Nodes)
            {
                if (string.Compare(n.FullPath, strNodeName, true) == 0)
                    break;
                else
                    FindNodeByName(aNode, strNodeName);
            }
        }

        private string findSize(String sUrl)
        {
            string[] spURL = null;
            string[] spParameter = null;
            string[] spSize = null;
            string sSize = string.Empty;

            spURL = sUrl.Split(new char[] { '?' });
            if (spURL != null)
            {
                if (spURL.Length > 1)
                {
                    spParameter = spURL[1].ToString().Split(new char[] { '&' });
                    if (spParameter != null)
                    {
                        if (spParameter.Length > 0)
                        {
                            for (int ip = 0; ip < spParameter.Length; ip++)
                            {
                                if (spParameter[ip].ToLower().IndexOf("size=") > -1)
                                {
                                    spSize = spParameter[ip].ToLower().Split(new char[] { '=' });
                                    if (spSize != null)
                                    {
                                        if (spSize.Length > 1)
                                        {
                                            sSize = spSize[1];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(sSize)) sSize = "20";
            return sSize;
        }
        private  String findTreeView(TreeView tv, List<ClassFieldFind> objField,string sSQLQuery, int iObject)
        {
            string sNode = string.Empty;
            string sKey = string.Empty;
            string sMapping = string.Empty;
            string sFull = string.Empty;
            int iLevel = 0;

            string[] spKeyvalue = null;
            string sTextNodeKey = string.Empty;
            string sTextNodeValue = string.Empty;

            string sSQL = string.Empty;
            for(int i=0;i < iObject; i++)
            {
                if (!string.IsNullOrEmpty(sSQL)) sSQL += " Union ";
                //for (int io=0; io <= objField.Count - 1; io++)
                //{
                //    sKey= objField[io].Key;
                //    sMapping = objField[io].Mapping;
                //    sFull = objField[io].FullPath;
                //    iLevel = objField[io].Level;

                //    spKeyvalue = sNode.Split(new char[] { ':' });
                //    if (spKeyvalue.Length > 0) sTextNodeKey = spKeyvalue[0];
                //    if (spKeyvalue.Length > 1) sTextNodeValue = spKeyvalue[1];
                // }
                listSQL= new List<string>();
                listValue = new List<ClassMapping>();
                RecurseNodes(lstJsonTree.Nodes, objField);
                string sIdKeep = string.Empty;
                if (listValue != null)
                {
                    foreach (ClassMapping it in listValue)
                    {
                        if (it != null)
                        {
                            if (sIdKeep != it.Id)
                            {
                                sIdKeep = it.Id;
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        private void generateSQLInsertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sKey = string.Empty;
            string sFieldQuery = string.Empty;
            string sField = string.Empty;
            string sFieldValue = string.Empty;
            string sFieldValueInsert = string.Empty;
            string sFullPath = string.Empty;
            string sMaster = string.Empty;
            int inLevel = 0;
            String sSQLForInsert = string.Empty;
            List<ClassToSQL> objToSQL;
            ClassToSQL objToItem;

            List<ClassFieldFind> objFindField;
            ClassFieldFind objFindFieldItem;
            string sSize = string.Empty;
            int iSize = 20;
            Boolean isNumeric = false;
            string sSQL = string.Empty;
            string sSelectInsert = string.Empty;
            string sformatInsert = string.Empty;

            string sFormatSQL = " '${0}:' as {1} ";
            string sQuery = string.Empty;
            radioButton2.Checked = true;
            string sURL = txtImportUrl.Text;
           
            if (listView1.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(sURL))
                {
                  sSize=  findSize(txtImportUrl.Text);
                    if (!string.IsNullOrEmpty(sSize))
                    {
                        isNumeric = int.TryParse(sSize, out iSize);
                        if (!isNumeric)
                        {
                            iSize = 20;
                        }
                    }
                objToSQL = new List<ClassToSQL>();
                objFindField = new List<ClassFieldFind>();

                sformatInsert = string.Empty;

                for (int iformat = 0; iformat <= listView1.Items.Count - 1; iformat++)
                {
                    sFieldQuery = listView1.Items[iformat].SubItems[1].Text;
                    if (!String.IsNullOrEmpty(sformatInsert)) sformatInsert += ",";
                    sformatInsert += string.Format(sFormatSQL, sFieldQuery, sFieldQuery);

                    sKey = listView1.Items[iformat].SubItems[0].Text;
                    sField = listView1.Items[iformat].SubItems[1].Text;
                    sFullPath = listView1.Items[iformat].SubItems[2].Text;
                    inLevel = Convert.ToInt16(listView1.Items[iformat].SubItems[3].Text);
                    sMaster = listView1.Items[iformat].SubItems[4].Text;
                    objFindFieldItem = new ClassFieldFind();
                    objFindFieldItem.Key = sKey;
                    objFindFieldItem.Mapping = sField;
                    objFindFieldItem.FullPath = sFullPath;
                    objFindFieldItem.Level = inLevel;
                    objFindFieldItem.Master = (sMaster == "*");
                    objFindField.Add(objFindFieldItem);
                }
                sSelectInsert = "SELECT " + sformatInsert + " FROM DUAL";
                //SELECT '$IDENTIFIER:' as IDENTIFIER,
                //'$LAST_UPDATE_DATE:' as LAST_UPDATE_DATE,
                //'$LAST_COLLECTION_POINT_ID:' as LAST_COLLECTION_POINT_ID
                //FROM DUAL
                sSQLForInsert = findTreeView(lstJsonTree, objFindField, sSelectInsert, iSize);
                }
            }
        }

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Size = " + findSize(txtImportUrl.Text));
        }

        private void saveMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sFormat = "\"{0}\"";
            string sValue = string.Empty;
            string sResult = string.Empty;
            string sWriteLine = string.Empty;
            string sFilename = string.Empty;
            string sFullFilename = string.Empty;
            string sIcon = string.Empty;
            SaveFileDialog svFilter = null;
            try
            {
                if (listView1.Items.Count > 0)
                {
                    svFilter = new SaveFileDialog();
                    svFilter.Filter = "Delimited (*.csv*)|*.csv|Text Files (*.txt*)|*.txt";
                    if (svFilter.ShowDialog() == DialogResult.OK)
                    {
                        sFilename = svFilter.FileName;
                        if (!string.IsNullOrEmpty(sFilename))
                        {
                            using (StreamWriter csv = new StreamWriter(sFilename, true))
                            {
                                sWriteLine = "#";
                                for (int icol = 0; icol <= listView1.Columns.Count - 1; icol++)
                                {
                                    sValue = listView1.Columns[icol].Text;
                                    if (!string.IsNullOrEmpty(sValue))
                                    {
                                        sResult = string.Format(sFormat, sValue);
                                        if (!string.IsNullOrEmpty(sWriteLine))
                                            sWriteLine += "\t";
                                        sWriteLine += sResult;
                                    }
                                }
                                if (!string.IsNullOrEmpty(sWriteLine))
                                    csv.WriteLine(sWriteLine);

                                for (int irow = 0; irow <= listView1.Items.Count - 1; irow++)
                                {
                                    sWriteLine = string.Empty;
                                    // sIcon=ListView2.Items(irow).ImageKey 
                                    for (int icol = 0; icol <= listView1.Columns.Count - 1; icol++)
                                    {
                                        sValue = listView1.Items[irow].SubItems[icol].Text.Replace("\"", "'");
                                        if (!string.IsNullOrEmpty(sValue))
                                        {
                                            sResult = string.Format(sFormat, sValue);
                                            if (!string.IsNullOrEmpty(sWriteLine))
                                                sWriteLine += "\t";
                                            sWriteLine += sResult;
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(sWriteLine))
                                        csv.WriteLine(sWriteLine);
                                }
                            }
                        }
                        else
                            MessageBox.Show("This function can not be used.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    MessageBox.Show("This function can not be used.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                sFilename = string.Empty;
            }
            finally
            {
                svFilter = null;
            }
        }

        private void openMappingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                OpenFileDialog fd = null/* TODO Change to default(_) if this is not a reference type */;
                string strFileName = string.Empty;
                string[] Field = null;
                ListViewItem ListviewItem = null/* TODO Change to default(_) if this is not a reference type */;
                int iColumn = 0;
                int iLoop = 0;
                try
                {
                    fd = new OpenFileDialog();

                    fd.Title = "Open File Dialog";
                    fd.Filter = "Text Files (*.txt*)|*.txt|Delimited (*.csv*)|*.csv";
                    fd.FilterIndex = 2;
                    fd.RestoreDirectory = true;
                    if (fd.ShowDialog() == DialogResult.OK)
                        strFileName = fd.FileName;

                    if (!string.IsNullOrEmpty(strFileName))
                    {
                        if (listView1.Items.Count > 0)
                        {
                            if (MessageBox.Show("Are you clear record ?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                listView1.Items.Clear();
                            else
                            {
                                MessageBox.Show("Please save file export before import file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                        iColumn = listView1.Columns.Count;

                        int iIndex = 0;
                        //using (TextReader reader = new TextFieldParser(strFileName))
                        //{
                        //    reader.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                        //    reader.SetDelimiters("\t");
                        //    reader.HasFieldsEnclosedInQuotes = true;
                        //    reader.TrimWhiteSpace = true;
                        //    reader.CommentTokens = new string[] { "--", "#" };
                        //    while (!reader.EndOfData)
                        //    {
                        //        Field = reader.ReadFields();
                        //        ListviewItem = new ListViewItem();
                        //        if (!Field == null)
                        //        {
                        //            iIndex = 0;
                        //            foreach (var cField in Field)
                        //            {
                        //                if (iIndex == 0)
                        //                {
                        //                    ListviewItem.StateImageIndex = cField;
                        //                    iIndex += 1;
                        //                }
                        //                else
                        //                {
                        //                    ListviewItem.SubItems.Add(cField.Replace("'", "\""));
                        //                    iIndex += 1;
                        //                }
                        //            }
                        //            if (iIndex < iColumn)
                        //            {
                        //                iLoop = iColumn - iIndex;
                        //                for (int iAdd = 0; iAdd <= iLoop - 1; iAdd++)
                        //                    ListviewItem.SubItems.Add("");
                        //            }
                        //            listView1.Items.Add(ListviewItem);
                        //        }
                        //    }
                        //}
                        for (int i = 0; i <= listView1.Columns.Count - 1; i++)
                        {
                            if (listView1.Columns[i].Width > 0)
                                listView1.AutoResizeColumn(i, ColumnHeaderAutoResizeStyle.ColumnContent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    fd = null/* TODO Change to default(_) if this is not a reference type */;
                }
            }
        }

        private void masterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.SelectedItems[0].SubItems[4].Text = "*";
            }
        }

        private void generateFixedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iTotal = 0;
            int iPage = 0;
            int iSize = 0;
            string[] spKeyValue = null;
            string sKey= string.Empty;
            string sValue = string.Empty;
            string sResult = string.Empty;
            string sSQLPatternInsert = string.Empty;
            string sSQL = string.Empty;
            string sIDENTIFIER = string.Empty;
            string sLAST_UPDATE_DATE = string.Empty;
            string sPURPOSE_ID = string.Empty;
            string sPURPOSE_NAME = string.Empty;
            string sPURPOSE_VERSION = string.Empty;
            string sPURPOSE_STATUS = string.Empty;
            string sFIRST_TRANSACTION_DATE = string.Empty;
            string sLAST_TRANSACTION_DATE = string.Empty;
            string sCONSENT_DATE = string.Empty;
            string sTOTAL_TRANSACTION_COUNT = string.Empty;
            string sLAST_COLLECTION_POINT_ID = string.Empty;
            string sLAST_COLLECTION_POINT_VERSION = string.Empty;
            string sSQLPattern = string.Empty;
            string[] separatingStrings = { "\" : \""};
            string sFormatInsert = "{0} \n {1};";
            string sFormat = "select {0} as IDENTIFIER "
                            + ",{1} as LAST_UPDATE_DATE "
                            + ",{2} as PURPOSE_ID "
                            + ",{3} as PURPOSE_NAME "
                            + ",{4} as PURPOSE_VERSION "
                            + ",{5} as PURPOSE_STATUS "
                            + ",{6} as FIRST_TRANSACTION_DATE "
                            + ",{7} as LAST_TRANSACTION_DATE "
                            + ",{8} as CONSENT_DATE "
                            + ",{9} as TOTAL_TRANSACTION_COUNT "
                            + ",{10} as LAST_COLLECTION_POINT_ID "
                            + ",{11} as  LAST_COLLECTION_POINT_VERSION "
                            + " from dual";
            sSQLPatternInsert = "INSERT INTO STAGING.CNST_CONSENT_PROFILES_A "
                                + "("
                                + "IDENTIFIER"
                                + ",LAST_UPDATE_DATE"
                                + ",PURPOSE_ID"
                                + ",PURPOSE_NAME"
                                + ",PURPOSE_VERSION"
                                + ",PURPOSE_STATUS"
                                + ",FIRST_TRANSACTION_DATE"
                                + ",LAST_TRANSACTION_DATE"
                                + ",CONSENT_DATE"
                                + ",TOTAL_TRANSACTION_COUNT"
                                + ",LAST_COLLECTION_POINT_ID"
                                + ",LAST_COLLECTION_POINT_VERSION"
                                + ")";

            if (lstJsonTree.Nodes.Count > 0)
            {
                foreach (TreeNode tnLevel0 in lstJsonTree.Nodes)
                {
                    if (tnLevel0.Text.ToLower().IndexOf("totalelements") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iTotal)) iTotal = 0;
                    }
                    if (tnLevel0.Text.ToLower().IndexOf("totalpages") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iPage)) iPage = 0;
                    }
                    if (tnLevel0.Text.ToLower().IndexOf("size") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iSize)) iSize = 0;
                    }

                    foreach (TreeNode tnLevel1 in tnLevel0.Nodes)
                    {
                        sIDENTIFIER = string.Empty;
                        sLAST_UPDATE_DATE = string.Empty;
                        sPURPOSE_ID = string.Empty;
                        sPURPOSE_NAME = string.Empty;
                        sPURPOSE_VERSION = string.Empty;
                        sPURPOSE_STATUS = string.Empty;
                        sFIRST_TRANSACTION_DATE = string.Empty;
                        sLAST_TRANSACTION_DATE = string.Empty;
                        sCONSENT_DATE = string.Empty;
                        sTOTAL_TRANSACTION_COUNT = string.Empty;
                        sLAST_COLLECTION_POINT_ID = string.Empty;
                        sLAST_COLLECTION_POINT_VERSION = string.Empty;
                        sSQLPattern = string.Empty;

                        foreach (TreeNode tnLevel2 in tnLevel1.Nodes)
                        {
                            if (tnLevel2.Text.ToLower().IndexOf("identifier") > -1)
                            {

                                sKey = string.Empty;
                                sValue = string.Empty;
                                spKeyValue = tnLevel2.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                if (spKeyValue != null)
                                {
                                    if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                    if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                }
                                sResult = sValue.Replace("\"", "");
                                if (String.IsNullOrEmpty(sResult))
                                {
                                    sIDENTIFIER = "null";
                                }else
                                {
                                    sIDENTIFIER = "'" + sResult + "'";
                                }
                            }
                            if (tnLevel2.Text.ToLower().IndexOf("lastupdateddate") > -1)
                            {

                                sKey = string.Empty;
                                sValue = string.Empty;
                                spKeyValue = tnLevel2.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                if (spKeyValue != null)
                                {
                                    if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                    if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                }
                                sResult = sValue.Replace("\"", "").Replace("T", " ");
                                if (String.IsNullOrEmpty(sResult))
                                {
                                    sLAST_UPDATE_DATE = "null";
                                }
                                else
                                {
                                    sLAST_UPDATE_DATE = "timestamp '" + sResult+"'";
                                }
                            }
                            foreach (TreeNode tnLevel3 in tnLevel2.Nodes)
                                {
                               
                                foreach (TreeNode tnLevel4 in tnLevel3.Nodes)
                            {

                                    if (tnLevel4.Text.ToLower().IndexOf("id") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }

                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_ID = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_ID = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("name") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_NAME = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_NAME = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("version") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_VERSION = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_VERSION =  sResult ;
                                        }

                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("status") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_STATUS = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_STATUS = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("firsttransactiondate") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;
                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sFIRST_TRANSACTION_DATE = "null";
                                        }
                                        else
                                        {
                                            sFIRST_TRANSACTION_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactiondate") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_TRANSACTION_DATE = "null";
                                        }
                                        else
                                        {
                                            sLAST_TRANSACTION_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("consentdate") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sCONSENT_DATE = "null";
                                        }
                                        else
                                        {
                                            sCONSENT_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("totaltransactioncount") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sTOTAL_TRANSACTION_COUNT = "null";
                                        }
                                        else
                                        {
                                            sTOTAL_TRANSACTION_COUNT = sResult;
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactioncollectionpointid") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_COLLECTION_POINT_ID = "null";
                                        }
                                        else
                                        {
                                            sLAST_COLLECTION_POINT_ID = "'" + sResult+"'";
                                        }

                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactioncollectionpointversion") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_COLLECTION_POINT_VERSION = "null";
                                        }
                                        else
                                        {
                                            sLAST_COLLECTION_POINT_VERSION = sResult;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(sIDENTIFIER.Trim())) sIDENTIFIER = "null";
                                    if (string.IsNullOrEmpty(sLAST_UPDATE_DATE.Trim())) sLAST_UPDATE_DATE = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_ID.Trim())) sPURPOSE_ID = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_NAME.Trim())) sPURPOSE_NAME = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_VERSION.Trim())) sPURPOSE_VERSION = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_STATUS.Trim())) sPURPOSE_STATUS = "null";
                                    if (string.IsNullOrEmpty(sFIRST_TRANSACTION_DATE.Trim())) sFIRST_TRANSACTION_DATE = "null";
                                    if (string.IsNullOrEmpty(sLAST_TRANSACTION_DATE.Trim())) sLAST_TRANSACTION_DATE = "null";
                                    if (string.IsNullOrEmpty(sCONSENT_DATE.Trim())) sCONSENT_DATE = "null";
                                    if (string.IsNullOrEmpty(sTOTAL_TRANSACTION_COUNT.Trim())) sTOTAL_TRANSACTION_COUNT = "null";
                                    if (string.IsNullOrEmpty(sLAST_COLLECTION_POINT_ID.Trim())) sLAST_COLLECTION_POINT_ID = "null";
                                    if (string.IsNullOrEmpty(sLAST_COLLECTION_POINT_VERSION.Trim())) sLAST_COLLECTION_POINT_VERSION = "null";


                                    sSQLPattern = string.Format(sFormat, sIDENTIFIER, sLAST_UPDATE_DATE, sPURPOSE_ID, sPURPOSE_NAME, sPURPOSE_VERSION, sPURPOSE_STATUS, sFIRST_TRANSACTION_DATE, sLAST_TRANSACTION_DATE, sCONSENT_DATE, sTOTAL_TRANSACTION_COUNT, sLAST_COLLECTION_POINT_ID, sLAST_COLLECTION_POINT_VERSION);

                                    if (!string.IsNullOrEmpty(sSQL)) sSQL += "\n Union \n";
                                    sSQL += sSQLPattern;


                                }
                            }
                        }
                    }
                }
            }
            radioButton2.Checked = true;
            txtsql.Clear();
            txtsql.Text =string.Format(sFormatInsert,sSQLPatternInsert, sSQL);
            if (!string.IsNullOrEmpty(txtsql.Text))
            {
                tabControl1.SelectedIndex = 2;
            }

        }
        
        private Boolean isCompanyTrue(string inName)
        {
            Boolean isResult=false;
            string inFind = string.Empty;
            inFind = inName.ToLower().Replace("'","").Substring(0, 3);
            isResult = (inFind == "tuc" || inFind == "tvg" || inFind == "tru" || inFind == "tic");
            return isResult;
        }

        private Boolean AutoProcessConsents(String inUrl)
        {
            Public_Service._Public objExe = null;
            Boolean isResult = false;
            string sQuery = string.Empty;
            string sSQL = string.Empty;
            int iTotal = -1;
            int iSize = -1;
            int iTotaPage = -1;
            Boolean isPageNumber = false;
            Boolean isSize = false;
            int iPage = 0;
            int iPageNumber = 0;
            int iSizeNumber = 0;
            BjSJsonArray objArray = null;
            BjSJsonObject objObject = null;

            BjSJsonArray objArrayChild = null;
            BjSJsonObject objObjectChild = null;

            String sMemberName = string.Empty;
            string sValue = string.Empty;
            string sValueKind = string.Empty;


            String sMemberNamechild = string.Empty;
            string sValuechild = string.Empty;
            string sValueKindchild = string.Empty;

            string sIDENTIFIER = string.Empty;
            string sLAST_UPDATE_DATE = string.Empty;
            string sPURPOSE_ID = string.Empty;
            string sPURPOSE_NAME = string.Empty;
            string sPURPOSE_VERSION = string.Empty;
            string sPURPOSE_STATUS = string.Empty;
            string sFIRST_TRANSACTION_DATE = string.Empty;
            string sLAST_TRANSACTION_DATE = string.Empty;
            string sCONSENT_DATE = string.Empty;
            string sTOTAL_TRANSACTION_COUNT = string.Empty;
            string sLAST_COLLECTION_POINT_ID = string.Empty;
            string sLAST_COLLECTION_POINT_VERSION = string.Empty;


            string sFormatText = "'{0}'";
            string sFormatTime = "timestamp '{0}'";
            string sSQLInsert = string.Empty;
            string sInsertSQL = string.Empty;
            string inURLArray = string.Empty;
            string sFormatSQL = "select {0} as IDENTIFIER "
                        + ",{1} as LAST_UPDATE_DATE "
                        + ",{2} as PURPOSE_ID "
                        + ",{3} as PURPOSE_NAME "
                        + ",{4} as PURPOSE_VERSION "
                        + ",{5} as PURPOSE_STATUS "
                        + ",{6} as FIRST_TRANSACTION_DATE "
                        + ",{7} as LAST_TRANSACTION_DATE "
                        + ",{8} as CONSENT_DATE "
                        + ",{9} as TOTAL_TRANSACTION_COUNT "
                        + ",{10} as LAST_COLLECTION_POINT_ID "
                        + ",{11} as  LAST_COLLECTION_POINT_VERSION "
                        + " from dual";
            sInsertSQL = "INSERT INTO STAGING.CNST_CONSENT_PROFILES_A "
                                + "("
                                + "IDENTIFIER"
                                + ",LAST_UPDATE_DATE"
                                + ",PURPOSE_ID"
                                + ",PURPOSE_NAME"
                                + ",PURPOSE_VERSION"
                                + ",PURPOSE_STATUS"
                                + ",FIRST_TRANSACTION_DATE"
                                + ",LAST_TRANSACTION_DATE"
                                + ",CONSENT_DATE"
                                + ",TOTAL_TRANSACTION_COUNT"
                                + ",LAST_COLLECTION_POINT_ID"
                                + ",LAST_COLLECTION_POINT_VERSION"
                                + ")";


            Boolean isNull = false;
            string inTNS = string.Empty;
            string sResult = string.Empty;
            string informattns = "User Id={0}; password={1}; Data Source={2}:{3}/{4}";
            string sFilename = "consent_process.txt";
            string sPage = string.Empty;
            string sSize = string.Empty;
            string sReadAll = string.Empty;
            string[] spLine = null;

            Boolean isSuccess = false;
            List<string> arrayURL = null;
            try
            {
                if (System.IO.File.Exists(sFilename))
                {
                    using (StreamReader file = new StreamReader(sFilename))
                    {
                        if (file != null)
                        {
                            sReadAll = file.ReadToEnd();
                            file.Close();
                        }
                    }
                }
                if (string.IsNullOrEmpty(sReadAll))
                {
                    inURLArray = inUrl;
                    sPage = "0";
                    sSize = "50";
                }
                else
                {
                    int couter = 0;
                    spLine = sReadAll.Split(new char[] { '\n' });
                    if (spLine != null)
                    {
                        foreach (string ln in spLine)
                        {
                            if (!string.IsNullOrEmpty(ln))
                            {
                                if (couter == 0) inURLArray = ln;
                                if (couter == 1) sPage = ln;
                                if (couter == 2) sSize = ln;
                                couter++;
                            }
                        }
                    }
                }
                //&updateSince=2020-03-04
                string inFullURL = string.Empty;
                inFullURL = inUrl + "?size=" + sSize.ToString() + "&page=" + sPage.ToString();
                if (!String.IsNullOrEmpty(inFullURL))
                {
                    BjSJsonObject json = new BjSJsonObject(fnURLtoValue(inFullURL));
                    //?size=50&page=1
                    if (json != null)
                    {
                        foreach (BjSJsonObjectMember member in json)
                        {
                            if (member.Name.ToLower().IndexOf("totalelements") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iTotal);
                                }
                            }
                            if (member.Name.ToLower().IndexOf("size") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iSize);
                                }
                            }
                            if (member.Name.ToLower().IndexOf("totalpages") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iTotaPage);
                                }
                            }

                        }
                    }
                   



                    isPageNumber = int.TryParse(sPage, out iPageNumber);
                    //inURLArray
                    if (!isPageNumber)
                    {
                        iPageNumber=0;
                    }
                    isSize = int.TryParse(sSize, out iSize);
                    if (!isSize)
                    {
                        iSize = 50;
                    }
                    string inUrlList = string.Empty;
                    List<BjSJsonObject> arrayjsonURL = new List<BjSJsonObject>();
                    arrayURL = new List<string>();
                    if (iTotaPage > -1)
                    {
                        if (iPageNumber <= iTotaPage)
                        {
                            // iTotaPage = iPageNumber;
                            for (iPage = iPageNumber; iPage <= iTotaPage - 1; iPage++)
                            {
                                inUrlList  = inUrl+ "?size=" + iSize.ToString() + "&page=" + iPage.ToString();
                                arrayURL.Add(inUrlList);
                            }
                        }else
                        {
                            isSuccess = true;
                            isResult = true;
                        }
                    }
                    // Stopwatch stopWatch = new Stopwatch();
                    // Parallel.ForEach(arrayURL, surl =>
                    //     {
                    //         BjSJsonObject jsonArray = new BjSJsonObject(fnURLtoValue(surl));
                    //         arrayjsonURL.Add(jsonArray);
                    //     }
                    // );
                    //string sStop=  stopWatch.Elapsed.TotalSeconds.ToString();
                    if (!isSuccess)
                    {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    int iLoop = 0;
                    foreach (string iURL in arrayURL)
                        {
                                if (!string.IsNullOrEmpty(iURL))
                                {
                                string[] spParam = iURL.Split(new char[] { '?' });
                                string[] spQ =   null;
                                string sS = string.Empty;
                                string sP = string.Empty;
                                if (spParam.Length > 1)
                                {
                                   spQ = spParam[1].Split(new char[] { '&' });
                                   sS= spQ[0].Substring(spQ[0].IndexOf("="),spQ[0].Length- spQ[0].IndexOf("="));
                                   sP= spQ[1].Substring(spQ[1].IndexOf("="), spQ[1].Length - spQ[1].IndexOf("="));

                                }
                                    string sStop = stopWatch.Elapsed.TotalSeconds.ToString();
                                    if (!string.IsNullOrEmpty(sFilename))
                                    {
                                        stopWatch.Stop();
                                        // Get the elapsed time as a TimeSpan value.
                                        TimeSpan ts = stopWatch.Elapsed;

                                        // Format and display the TimeSpan value.
                                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10);
                                        System.IO.File.WriteAllText(sFilename, inUrl + "\n" + sP.Replace("=","") + "\n" + sS.Replace("=", "") + "\n" + "RunTime " + DateTime.Now.ToString());
                                    }

                                BjSJsonObject jsonArray = new BjSJsonObject(fnURLtoValue(iURL));
                            if (jsonArray != null)
                            {
                                if (jsonArray != null)
                                {
                                    foreach (BjSJsonObjectMember member in jsonArray)
                                    {
                                      
                                        if (member.Name.ToLower().IndexOf("content") > -1)
                                        {
                                            if (member.ValueKind.ToString() == "Array")
                                            {
                                                objArray = (BjSJsonArray)member.Value;
                                                if (objArray != null)
                                                {
                                                    for (int i = 0; i <= objArray.Count - 1; i++)
                                                    {
                                                        objObject = (BjSJsonObject)objArray[i];
                                                        for (int j = 0; j <= objObject.Count - 1; j++)
                                                        {
                                                            if (objObject[j].Value != null)
                                                            {
                                                                sMemberName = objObject[j].Name.ToString();
                                                                sValueKind = objObject[j].ValueKind.ToString();
                                                                sValue = objObject[j].Value.ToString();
                                                                if (sMemberName.ToLower().IndexOf("identifier") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sIDENTIFIER = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sIDENTIFIER = string.Format(sFormatText, sValue);
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("lastupdateddate") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sLAST_UPDATE_DATE = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sLAST_UPDATE_DATE = string.Format(sFormatTime, sValue.Replace("T"," "));
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("purpose") > -1)
                                                                {
                                                                    objArrayChild = (BjSJsonArray)objObject[j].Value;
                                                                    if (objArrayChild != null)
                                                                    {
                                                                        for (int ichild = 0; ichild <= objArrayChild.Count - 1; ichild++)
                                                                        {
                                                                            objObjectChild = (BjSJsonObject)objArrayChild[ichild];
                                                                            for (int jchild = 0; jchild <= objObjectChild.Count - 1; jchild++)
                                                                            {
                                                                                if (objObjectChild[jchild].Value != null)
                                                                                {
                                                                                    sMemberNamechild = objObjectChild[jchild].Name.ToString();
                                                                                    sValueKindchild = objObjectChild[jchild].ValueKind.ToString();
                                                                                    sValuechild = objObjectChild[jchild].Value.ToString();

                                                                                    if (sMemberNamechild.ToLower() == "id")
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sPURPOSE_ID = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sPURPOSE_ID = string.Format(sFormatText, sValuechild);
                                                                                        }
                                                                                    }
                                                                                    if (sMemberNamechild.ToLower() == "name")
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sPURPOSE_NAME = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sPURPOSE_NAME = string.Format(sFormatText, sValuechild);
                                                                                        }
                                                                                    }

                                                                                    if (sMemberNamechild.ToLower() == "status")
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sPURPOSE_STATUS = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sPURPOSE_STATUS = string.Format(sFormatText, sValuechild);
                                                                                        }
                                                                                    }

                                                                                    if (sMemberNamechild.ToLower() == "version")
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sPURPOSE_VERSION = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sPURPOSE_VERSION = sValuechild;
                                                                                        }
                                                                                    }
                                                                                    if (sMemberNamechild.ToLower() == "totaltransactioncount")  
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sTOTAL_TRANSACTION_COUNT = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sTOTAL_TRANSACTION_COUNT = sValuechild;
                                                                                        }
                                                                                    }
                                                                                    if (sMemberNamechild.ToLower() == "consentdate") 
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sCONSENT_DATE = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sCONSENT_DATE = string.Format(sFormatTime, sValuechild.Replace("T", " "));
                                                                                        }
                                                                                    }
                                                                                    if (sMemberNamechild.ToLower() == "firsttransactiondate")
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sFIRST_TRANSACTION_DATE= "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sFIRST_TRANSACTION_DATE = string.Format(sFormatTime, sValuechild.Replace("T", " "));
                                                                                        }
                                                                                    }

                                                                                    if (sMemberNamechild.ToLower() == "lasttransactiondate") 
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sLAST_TRANSACTION_DATE = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sLAST_TRANSACTION_DATE = string.Format(sFormatTime, sValuechild.Replace("T"," "));
                                                                                        }
                                                                                    }

                                                                                    if (sMemberNamechild.ToLower() == "lasttransactioncollectionpointid")  
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sLAST_COLLECTION_POINT_ID = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sLAST_COLLECTION_POINT_ID = string.Format(sFormatText, sValuechild);
                                                                                        }
                                                                                    }

                                                                                    if (sMemberNamechild.ToLower() ==  "lasttransactioncollectionpointversion") 
                                                                                    {
                                                                                        if (string.IsNullOrEmpty(sValuechild))
                                                                                        {
                                                                                            sLAST_COLLECTION_POINT_VERSION = "null";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            sLAST_COLLECTION_POINT_VERSION = sValuechild;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }

                                                                            if (isCompanyTrue(sPURPOSE_NAME)) { 
                                                                                isNull = ((sIDENTIFIER != "null") && (sLAST_UPDATE_DATE != "null") && (sPURPOSE_ID != "null") && (sPURPOSE_NAME != "null") && (sPURPOSE_VERSION != "null") && (sPURPOSE_STATUS != "null") && (sFIRST_TRANSACTION_DATE != "null") && (sLAST_TRANSACTION_DATE != "null") && (sCONSENT_DATE != "null") && (sTOTAL_TRANSACTION_COUNT != "null") && (sLAST_COLLECTION_POINT_ID != "null") && (sLAST_COLLECTION_POINT_VERSION != "null"));
                                                                                if (isNull)
                                                                                {
                                                                                    sSQL = string.Format(sFormatSQL, sIDENTIFIER, sLAST_UPDATE_DATE, sPURPOSE_ID, sPURPOSE_NAME, sPURPOSE_VERSION, sPURPOSE_STATUS, sFIRST_TRANSACTION_DATE, sLAST_TRANSACTION_DATE, sCONSENT_DATE, sTOTAL_TRANSACTION_COUNT, sLAST_COLLECTION_POINT_ID, sLAST_COLLECTION_POINT_VERSION);
                                                                                    if (!string.IsNullOrEmpty(sQuery)) sQuery += " Union All ";
                                                                                    sQuery += sSQL;
                                                                                        iLoop += 1;

                                                                                        if (iLoop % 1000 == 0)
                                                                                        {
                                                                                            sSQLInsert = sInsertSQL + "\n" + sQuery;
                                                                                            objExe = new Public_Service._Public();
                                                                                            inTNS = string.Format(informattns, "staging", "staging", "dbdwhdv1", "1550", "TEDWDEV");
                                                                                            sResult = objExe.ExecuteDataTNS(inTNS, sSQLInsert);
                                                                                            isResult = (sResult.ToLower() == "true");
                                                                                            if (isResult)
                                                                                            {
                                                                                                sQuery = string.Empty;
                                                                                               
                                                                                            }
                                                                                        }

                                                                                    }
                                                                                }
                                                                      }
                                                                }
                                                            }
                                                          }
                                                        }

                                                   
                                                    }
                                                }
                                            }
                                        }
                                        if (member.Name.ToLower() == "number" )
                                        {
                                            sPage =(Convert.ToInt16(member.Value)+1).ToString();
                                        }
                                    }
                                }
                            }
                        }
                    if (iTotaPage == 1)
                    {
                                sSQLInsert = sInsertSQL + "\n" + sQuery;
                                objExe = new Public_Service._Public();
                                inTNS = string.Format(informattns, "staging", "staging", "dbdwhdv1", "1550", "TEDWDEV");
                                sResult = objExe.ExecuteDataTNS(inTNS, sSQLInsert);
                                isResult = (sResult.ToLower() == "true");
                                if (isResult)
                                {
                                    sQuery = string.Empty;
                                    string sStop = stopWatch.Elapsed.TotalSeconds.ToString();
                                    if (!string.IsNullOrEmpty(sFilename))
                                    {
                                        stopWatch.Stop();
                                        // Get the elapsed time as a TimeSpan value.
                                        TimeSpan ts = stopWatch.Elapsed;

                                        // Format and display the TimeSpan value.
                                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                            ts.Hours, ts.Minutes, ts.Seconds,
                                            ts.Milliseconds / 10);
                                        System.IO.File.WriteAllText(sFilename, inUrl + "\n" + sPage + "\n" + sSize + "\n" + "RunTime " + DateTime.Now.ToString());
                                    }
                                }
                            } 
                    }
                }
              }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                isResult = false;
            }
            finally
            {
                objArray = null;
                objExe = null;
            }
            return isResult;
        }
        private Boolean AutoProcessCollection(String inUrl)
        {
            Public_Service._Public objExe =null;
            Boolean isResult = false;
            string sQuery = string.Empty;
            string sSQL = string.Empty;
            int iTotal = -1;
            int iSize = -1;
            int iTotaPage = -1;
            BjSJsonArray objArray = null;
            BjSJsonObject objObject = null;
            String sMemberName = string.Empty;
            string sValue = string.Empty;
            string sValueKind = string.Empty;
            string sCollectionID = string.Empty;
            string sCollectionName = string.Empty;
            string sCollectionStatus = string.Empty;
            string sCollectionCreateDate = string.Empty;
            string sCollectionType = string.Empty;
            string sCollectionSubject = string.Empty;
            string sFormatSQL = " select "
                                + " {0} as CollectionID "
                                + " ,{1} as CollectionName "
                                + " ,{2} as CollectionStatus "
                                + " ,{3} as CollectionType "
                                + " ,{4} as CollectionSubject "
                                + " ,{5} as CollectionCreateDate "
                                + " ,sysdate as PPN_TM"
                                + " ,'developer' as PPN_BY"
                                + " From Dual ";
            string sFormatText = "'{0}'";
            string sFormatTime = "timestamp '{0}'";
            string sSQLInsert = string.Empty;
            string sInsertSQL = "insert into staging.cnst_collection "
                                + " ( COLLECTION_ID"
                                + " ,COLLECTION_NAME "
                                + " ,COLLECTION_STATUS"
                                + " ,COLLECTION_TYPE"
                                + " ,COLLECTION_SUBJECT"
                                + " ,CREATE_DATE"
                                + " ,PPN_TM"
                                + " ,PPN_BY)";
            Boolean isNull = false;
            string inTNS = string.Empty;
            string sResult = string.Empty;
            string informattns = "User Id={0}; password={1}; Data Source={2}:{3}/{4}";

            List<string> arrayURL = null;
            try
            {
                if (!String.IsNullOrEmpty(inUrl))
                {
                    BjSJsonObject json = new BjSJsonObject(fnURLtoValue(inUrl));
                    //?size=50&page=1
                    if (json != null)
                    {
                        foreach (BjSJsonObjectMember member in json)
                        {
                            if (member.Name.ToLower().IndexOf("totalelements") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iTotal);
                                }
                            }
                            if (member.Name.ToLower().IndexOf("size") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iSize);
                                }
                            }
                            if (member.Name.ToLower().IndexOf("totalpages") > -1)
                            {
                                if (member.ValueKind.ToString().ToLower() == "number")
                                {
                                    int.TryParse(member.Value.ToString(), out iTotaPage);
                                }
                            }

                        }
                    }
                    string inUrlList = string.Empty;
                   arrayURL =new  List<string>();
                   if (iTotaPage > -1)
                    {
                        for(int iPage = 0; iPage <= iTotaPage - 1; iPage++)
                        {
                            inUrlList  = inUrl+ "?size=" + iSize.ToString() + "&page=" + iPage.ToString();
                            arrayURL.Add(inUrlList);
                        }
                    }
                   foreach(string iURL in arrayURL)
                    {
                        if (!string.IsNullOrEmpty(iURL))
                        {
                            BjSJsonObject jsonArray = new BjSJsonObject(fnURLtoValue(iURL));
                            if (jsonArray != null)
                            {
                                if (jsonArray != null)
                                {
                                    foreach (BjSJsonObjectMember member in jsonArray)
                                    {
                                        if (member.Name.ToLower().IndexOf("content") > -1)
                                        {
                                            if (member.ValueKind.ToString() == "Array")
                                            {
                                                objArray = (BjSJsonArray)member.Value;
                                                if (objArray != null)
                                                {
                                                    for (int i = 0; i <= objArray.Count - 1; i++)
                                                    {
                                                        objObject = (BjSJsonObject)objArray[i];
                                                        for (int j = 0; j <= objObject.Count - 1; j++)
                                                        {
                                                            if (objObject[j].Value != null)
                                                            {
                                                                sMemberName = objObject[j].Name.ToString();
                                                                sValueKind = objObject[j].ValueKind.ToString();
                                                                sValue = objObject[j].Value.ToString();
                                                                if (sMemberName.ToLower().IndexOf("id") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionID = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionID = string.Format(sFormatText, sValue);
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("name") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionName = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionName = string.Format(sFormatText, sValue);
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("createdate") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionCreateDate = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionCreateDate = string.Format(sFormatTime, sValue.Replace("T", " "));
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("status") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionStatus = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionStatus = string.Format(sFormatText, sValue);
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("collectionpointtype") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionType = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionType = string.Format(sFormatText, sValue);
                                                                    }
                                                                }
                                                                if (sMemberName.ToLower().IndexOf("subjectidentifier") > -1)
                                                                {
                                                                    if (string.IsNullOrEmpty(sValue))
                                                                    {
                                                                        sCollectionSubject = "null";
                                                                    }
                                                                    else
                                                                    {
                                                                        sCollectionSubject = string.Format(sFormatText, sValue);
                                                                    }
                                                                }


                                                            }
                                                        }

                                                        isNull =((sCollectionID != "null") && (sCollectionName != "null") && (sCollectionStatus != "null") && (sCollectionType != "null") && (sCollectionSubject != "null") && (sCollectionCreateDate != "null"));
                                                        if (isNull)
                                                        {
                                                            sSQL = string.Format(sFormatSQL, sCollectionID, sCollectionName, sCollectionStatus, sCollectionType, sCollectionSubject, sCollectionCreateDate);
                                                            if (!string.IsNullOrEmpty(sQuery)) sQuery += " Union All";
                                                            sQuery += sSQL;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                  

                    }
                if (!string.IsNullOrEmpty(sQuery))
                {
                    sSQLInsert = sInsertSQL + "\n" + sQuery;
                    objExe = new Public_Service._Public();
                    inTNS = string.Format(informattns, "staging", "staging", "dbdwhdv1", "1550", "TEDWDEV");
                    sResult = objExe.ExecuteDataTNS(inTNS, sSQLInsert);
                    isResult = (sResult.ToLower() == "true");
                  

                }
                }
            catch (Exception ex)
            {
                string message = ex.Message;
                isResult = false;
            }
            finally
            {
                objArray = null;
                objExe = null;
            }
            return isResult;
        }
        private string AutoProcessConsent(string inUrl)
        {
            int iTotal = 0;
            int iPage = 0;
            int iSize = 0;
            string[] spKeyValue = null;
            string sKey = string.Empty;
            string sValue = string.Empty;
            string sResult = string.Empty;
            string sSQLPatternInsert = string.Empty;
            string sSQL = string.Empty;
            string sIDENTIFIER = string.Empty;
            string sLAST_UPDATE_DATE = string.Empty;
            string sPURPOSE_ID = string.Empty;
            string sPURPOSE_NAME = string.Empty;
            string sPURPOSE_VERSION = string.Empty;
            string sPURPOSE_STATUS = string.Empty;
            string sFIRST_TRANSACTION_DATE = string.Empty;
            string sLAST_TRANSACTION_DATE = string.Empty;
            string sCONSENT_DATE = string.Empty;
            string sTOTAL_TRANSACTION_COUNT = string.Empty;
            string sLAST_COLLECTION_POINT_ID = string.Empty;
            string sLAST_COLLECTION_POINT_VERSION = string.Empty;
            string sSQLPattern = string.Empty;
            string[] separatingStrings = { "\" : \"" };
            string sFormatInsert = "{0} \n {1}";
            string sFormat = "select {0} as IDENTIFIER "
                            + ",{1} as LAST_UPDATE_DATE "
                            + ",{2} as PURPOSE_ID "
                            + ",{3} as PURPOSE_NAME "
                            + ",{4} as PURPOSE_VERSION "
                            + ",{5} as PURPOSE_STATUS "
                            + ",{6} as FIRST_TRANSACTION_DATE "
                            + ",{7} as LAST_TRANSACTION_DATE "
                            + ",{8} as CONSENT_DATE "
                            + ",{9} as TOTAL_TRANSACTION_COUNT "
                            + ",{10} as LAST_COLLECTION_POINT_ID "
                            + ",{11} as  LAST_COLLECTION_POINT_VERSION "
                            + " from dual";
            sSQLPatternInsert = "INSERT INTO STAGING.CNST_CONSENT_PROFILES_A "
                                + "("
                                + "IDENTIFIER"
                                + ",LAST_UPDATE_DATE"
                                + ",PURPOSE_ID"
                                + ",PURPOSE_NAME"
                                + ",PURPOSE_VERSION"
                                + ",PURPOSE_STATUS"
                                + ",FIRST_TRANSACTION_DATE"
                                + ",LAST_TRANSACTION_DATE"
                                + ",CONSENT_DATE"
                                + ",TOTAL_TRANSACTION_COUNT"
                                + ",LAST_COLLECTION_POINT_ID"
                                + ",LAST_COLLECTION_POINT_VERSION"
                                + ")";

            if (lstJsonTree.Nodes.Count > 0)
            {
                foreach (TreeNode tnLevel0 in lstJsonTree.Nodes)
                {
                    if (tnLevel0.Text.ToLower().IndexOf("totalelements") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iTotal)) iTotal = 0;
                    }
                    if (tnLevel0.Text.ToLower().IndexOf("totalpages") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iPage)) iPage = 0;
                    }
                    if (tnLevel0.Text.ToLower().IndexOf("size") > -1)
                    {
                        spKeyValue = tnLevel0.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                        if (spKeyValue != null)
                        {
                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                        }
                        if (!int.TryParse(sValue, out iSize)) iSize = 0;
                    }

                    foreach (TreeNode tnLevel1 in tnLevel0.Nodes)
                    {
                        sIDENTIFIER = string.Empty;
                        sLAST_UPDATE_DATE = string.Empty;
                        sPURPOSE_ID = string.Empty;
                        sPURPOSE_NAME = string.Empty;
                        sPURPOSE_VERSION = string.Empty;
                        sPURPOSE_STATUS = string.Empty;
                        sFIRST_TRANSACTION_DATE = string.Empty;
                        sLAST_TRANSACTION_DATE = string.Empty;
                        sCONSENT_DATE = string.Empty;
                        sTOTAL_TRANSACTION_COUNT = string.Empty;
                        sLAST_COLLECTION_POINT_ID = string.Empty;
                        sLAST_COLLECTION_POINT_VERSION = string.Empty;
                        sSQLPattern = string.Empty;

                        foreach (TreeNode tnLevel2 in tnLevel1.Nodes)
                        {
                            if (tnLevel2.Text.ToLower().IndexOf("identifier") > -1)
                            {

                                sKey = string.Empty;
                                sValue = string.Empty;
                                spKeyValue = tnLevel2.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                if (spKeyValue != null)
                                {
                                    if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                    if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                }
                                sResult = sValue.Replace("\"", "");
                                if (String.IsNullOrEmpty(sResult))
                                {
                                    sIDENTIFIER = "null";
                                }
                                else
                                {
                                    sIDENTIFIER = "'" + sResult + "'";
                                }
                            }
                            if (tnLevel2.Text.ToLower().IndexOf("lastupdateddate") > -1)
                            {

                                sKey = string.Empty;
                                sValue = string.Empty;
                                spKeyValue = tnLevel2.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                if (spKeyValue != null)
                                {
                                    if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                    if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                }
                                sResult = sValue.Replace("\"", "").Replace("T", " ");
                                if (String.IsNullOrEmpty(sResult))
                                {
                                    sLAST_UPDATE_DATE = "null";
                                }
                                else
                                {
                                    sLAST_UPDATE_DATE = "timestamp '" + sResult + "'";
                                }
                            }
                            foreach (TreeNode tnLevel3 in tnLevel2.Nodes)
                            {

                                foreach (TreeNode tnLevel4 in tnLevel3.Nodes)
                                {

                                    if (tnLevel4.Text.ToLower().IndexOf("id") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }

                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_ID = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_ID = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("name") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_NAME = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_NAME = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("version") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_VERSION = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_VERSION = sResult;
                                        }

                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("status") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sPURPOSE_STATUS = "null";
                                        }
                                        else
                                        {
                                            sPURPOSE_STATUS = "'" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("firsttransactiondate") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;
                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sFIRST_TRANSACTION_DATE = "null";
                                        }
                                        else
                                        {
                                            sFIRST_TRANSACTION_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactiondate") > -1)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_TRANSACTION_DATE = "null";
                                        }
                                        else
                                        {
                                            sLAST_TRANSACTION_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("consentdate") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "").Replace("T", " ");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sCONSENT_DATE = "null";
                                        }
                                        else
                                        {
                                            sCONSENT_DATE = "timestamp '" + sResult + "'";
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("totaltransactioncount") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sTOTAL_TRANSACTION_COUNT = "null";
                                        }
                                        else
                                        {
                                            sTOTAL_TRANSACTION_COUNT = sResult;
                                        }
                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactioncollectionpointid") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_COLLECTION_POINT_ID = "null";
                                        }
                                        else
                                        {
                                            sLAST_COLLECTION_POINT_ID = "'" + sResult + "'";
                                        }

                                    }
                                    if (tnLevel4.Text.ToLower().IndexOf("lasttransactioncollectionpointversion") > 0)
                                    {

                                        sKey = string.Empty;
                                        sValue = string.Empty;

                                        spKeyValue = tnLevel4.Text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                                        if (spKeyValue != null)
                                        {
                                            if (spKeyValue.Length > 0) sKey = spKeyValue[0]; else sValue = string.Empty;
                                            if (spKeyValue.Length > 1) sValue = spKeyValue[1]; else sValue = string.Empty;
                                        }
                                        sResult = sValue.Replace("\"", "");
                                        if (String.IsNullOrEmpty(sResult))
                                        {
                                            sLAST_COLLECTION_POINT_VERSION = "null";
                                        }
                                        else
                                        {
                                            sLAST_COLLECTION_POINT_VERSION = sResult;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(sIDENTIFIER.Trim())) sIDENTIFIER = "null";
                                    if (string.IsNullOrEmpty(sLAST_UPDATE_DATE.Trim())) sLAST_UPDATE_DATE = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_ID.Trim())) sPURPOSE_ID = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_NAME.Trim())) sPURPOSE_NAME = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_VERSION.Trim())) sPURPOSE_VERSION = "null";
                                    if (string.IsNullOrEmpty(sPURPOSE_STATUS.Trim())) sPURPOSE_STATUS = "null";
                                    if (string.IsNullOrEmpty(sFIRST_TRANSACTION_DATE.Trim())) sFIRST_TRANSACTION_DATE = "null";
                                    if (string.IsNullOrEmpty(sLAST_TRANSACTION_DATE.Trim())) sLAST_TRANSACTION_DATE = "null";
                                    if (string.IsNullOrEmpty(sCONSENT_DATE.Trim())) sCONSENT_DATE = "null";
                                    if (string.IsNullOrEmpty(sTOTAL_TRANSACTION_COUNT.Trim())) sTOTAL_TRANSACTION_COUNT = "null";
                                    if (string.IsNullOrEmpty(sLAST_COLLECTION_POINT_ID.Trim())) sLAST_COLLECTION_POINT_ID = "null";
                                    if (string.IsNullOrEmpty(sLAST_COLLECTION_POINT_VERSION.Trim())) sLAST_COLLECTION_POINT_VERSION = "null";


                                    sSQLPattern = string.Format(sFormat, sIDENTIFIER, sLAST_UPDATE_DATE, sPURPOSE_ID, sPURPOSE_NAME, sPURPOSE_VERSION, sPURPOSE_STATUS, sFIRST_TRANSACTION_DATE, sLAST_TRANSACTION_DATE, sCONSENT_DATE, sTOTAL_TRANSACTION_COUNT, sLAST_COLLECTION_POINT_ID, sLAST_COLLECTION_POINT_VERSION);

                                    if (!string.IsNullOrEmpty(sSQL)) sSQL += "\n Union \n";
                                    sSQL += sSQLPattern;


                                }
                            }
                        }
                    }
                }
            }
            return string.Format(sFormatInsert, sSQLPatternInsert, sSQL); 
        }
        private void runAutoProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string inURL = string.Empty;
            string inTNS = string.Empty;
            string sSQL = string.Empty;
            string sResult = string.Empty;
            string sFormat = "\"{0}\"";
            string informattns = "User Id={0}; password={1}; Data Source={2}:{3}/{4}";
            string sValue = string.Empty;
            string sFilename = "consent_process.txt";
            string sPage = string.Empty;
            string sSize = string.Empty;
            string sReadAll = string.Empty;
            string[] spLine = null;
            string sCollectionValue = string.Empty;
            string inUrlList = string.Empty;
            Public_Service._Public objExe;
            try
                {
                if (System.IO.File.Exists(sFilename))
                {
                    using (StreamReader file = new StreamReader(sFilename))
                    {
                        if (file != null)
                        {
                            sReadAll=  file.ReadToEnd();
                        file.Close();
                        }
                    }
                }
                if (string.IsNullOrEmpty(sReadAll))
                {
                    sPage = "1";
                    sSize = "20";
                    inURL = "https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profiles";

                }else
                {
                    int couter = 0;
                    spLine = sReadAll.Split(new char[] { '\n' });
                    if (spLine != null)
                    {
                        foreach(string ln in spLine)
                        {
                            if (!string.IsNullOrEmpty(ln))
                            {
                                if (couter == 0) inURL = ln;
                                if (couter == 1) sPage = ln;
                                if (couter == 2) sSize = ln;
                                couter++;
                            }
                    }
                }
                    if (sPage != "") sPage = (Convert.ToInt16(sPage) + 1).ToString();
                inTNS = string.Format(informattns, "staging", "staging", "dbdwhdv1", "1550", "TEDWDEV");
                txtImportUrl.Text =inURL +"?size=" + sSize + "&page=" + sPage;
                txtJson.Clear();
                btnLoad_Click(sender, e);
                sSQL = AutoProcessConsent(inURL + "?size=" + sSize + "&page=" + sPage);
                    if (!string.IsNullOrEmpty(sSQL))
                    {
                        objExe = new Public_Service._Public();
                        sResult = objExe.ExecuteDataTNS(inTNS, sSQL);
                        if (sResult.ToLower()=="true")
                        {
                            if (!string.IsNullOrEmpty(sFilename))
                            {
                                System.IO.File.WriteAllText(sFilename, inURL + "\n" + sPage + "\n" + sSize);
                             }
                            MessageBox.Show(sResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
        //https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profilesapi/consentmanager/v1/collectionpoints
        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void runCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isResult = false;
            string inCollectionURL = "https://uat-de.onetrust.com/api/consentmanager/v1/collectionpoints";
           isResult= AutoProcessCollection(inCollectionURL);
           if(isResult) {
                MessageBox.Show("Collection To Database Success", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
          }else
            {
                MessageBox.Show("Collection To Database not Success", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void runConsentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isResult = false;
            string inCollectionURL = "https://uat-de.onetrust.com/api/consentmanager/v1/datasubjects/profiles";
            isResult = AutoProcessConsents(inCollectionURL);
            if (isResult)
            {
                MessageBox.Show("Consent To Database Success", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Consent To Database not Success", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtJson_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
