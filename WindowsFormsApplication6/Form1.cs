using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

namespace WindowsFormsApplication6
{
    public partial class Form1 : Form
    {

        Warehouse thisWH;
        public Form1(Warehouse WH)
        {
            this.thisWH = WH;
            InitializeComponent();

            //if (!thisWH.OpenSession("oscar", "123456"))
            //{
            //    MessageBox.Show("Connection Error");
            //}

        }

        DataTable dt = new DataTable("Data");
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt;
            int unsavedRecordsCount = 0;
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", ValidateNames = true, Multiselect = false })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    dt = readCsv(ofd.FileName);
                    foreach (DataRow row in dt.Rows)
                    {
                        // Checks if the row is Empty
                        string rowValues = "";
                        string[] csvElements = new string[dt.Columns.Count];
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            rowValues += row[i].ToString();
                            csvElements[i] = row[i].ToString();
                        }
                        if (rowValues == "")
                        {
                            continue;
                        }

                        if (thisWH.saveWareHouseReceipt(csvElements) == "Something went wrong")
                        {
                            unsavedRecordsCount++;
                        }
                    }
                    if (unsavedRecordsCount == 0)
                        MessageBox.Show("Records are saved");
                    else
                        MessageBox.Show(unsavedRecordsCount.ToString() + " records were not saved");

                }
            }

        }

        public DataTable readCsv(string fileName)
        {

            using (OleDbConnection cn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" +
                Path.GetDirectoryName(fileName) + "\";Extended Properties='text;HDR=yes;FMT=Delimited(,)';"))
            {
                cn.Open(); using (OleDbCommand cmd = new OleDbCommand(string.Format("select *from [{0}]", new FileInfo(fileName).Name), cn))
                {

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }


        // Get Warehouse by Number
        private void button2_Click(object sender, EventArgs e)
        {
            int flag = 0x00000000;
            string[] showResult = thisWH.getWareHouseReceipt(flag, textBox4.Text);

            //ShowResult[3] Error position
            if (showResult[3] != null)
            {
                MessageBox.Show(showResult[3]);
            }
            else
            {
                textBox2.Text = ""; textBox3.Text = "";
                textBox2.Text = showResult[1];
                textBox3.Text = showResult[2];
                try
                {
                    dateTimePicker1.Value = Convert.ToDateTime(showResult[0]);
                }
                catch
                {
                    dateTimePicker1.Value = DateTime.Now;
                }

            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!thisWH.EndSession())
            {
                MessageBox.Show("Couldn't end the session");
            }
        }
    }
}
