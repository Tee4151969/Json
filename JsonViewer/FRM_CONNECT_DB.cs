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
    public partial class FRM_CONNECT_DB : Form
    {
        ClassDB objDB;
        public FRM_CONNECT_DB()
        {
            InitializeComponent();
        }

        private void FRM_CONNECT_DB_Load(object sender, EventArgs e)
        {

        }

        private void FRM_CONNECT_DB_FormClosed(object sender, FormClosedEventArgs e)
        {
           if (objDB !=null) this.Tag = objDB;
        }
        private void BTN_ADD_Click(object sender, EventArgs e)
        {
            Public_Service._Public objConnectDB;
            bool isResult = false;
            string cUser_Login = string.Empty;
            string cPassword_Login = string.Empty;
            string cHost = string.Empty;
            string cPort = string.Empty;
            string cInstance = string.Empty;
            string informattns = "User Id={0}; password={1}; Data Source={2}:{3}/{4}";
            string intns = string.Empty;
            string sSQL = string.Empty;
            string sFormatSQL = "select column_name from cols where lower(table_name) = '{0}'";
            DataTable oTable;
            isResult = (!string.IsNullOrEmpty(txtUserDB.Text) && !string.IsNullOrEmpty(txtPasswordDB.Text) && !string.IsNullOrEmpty(txtHost.Text) && !string.IsNullOrEmpty(txtPort.Text) && !string.IsNullOrEmpty(txtInstance.Text) && !string.IsNullOrEmpty(txtTable.Text));
            if (isResult)
            {
//'User Id=staging; password=staging; Data Source=dbdwhdv1:1550/tedwdev;'
                cUser_Login = txtUserDB.Text;
                cPassword_Login = txtPasswordDB.Text;
                cHost = txtHost.Text;
                cPort = txtPort.Text;
                cInstance = txtInstance.Text;

                objConnectDB = new Public_Service._Public();
                intns = string.Format(informattns, cUser_Login, cPassword_Login, cHost, cPort, cInstance);
                sSQL = string.Format(sFormatSQL, txtTable.Text);
                oTable = objConnectDB.SelectDataTNS(intns, sSQL);
                if (oTable != null)
                {
                    objDB = new ClassDB();
                    objDB.User = txtUserDB.Text;
                    objDB.Password = txtPasswordDB.Text;
                    objDB.Host = txtHost.Text;
                    objDB.Port = txtPort.Text;
                    objDB.Instance = txtInstance.Text;
                    objDB.Table = txtTable.Text;
                    MessageBox.Show("connect pass", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }else
                {
                    MessageBox.Show("is not connect database", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
    }
}
