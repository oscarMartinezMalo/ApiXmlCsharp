using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace WindowsFormsApplication6
{
    public partial class Access : Form
    {
        Thread th;
        Warehouse thisWH;
        public Access()
        {
            InitializeComponent();
        }

        public bool session()
        {
            return thisWH.OpenSession(textBox1.Text, textBox2.Text);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            thisWH = new Warehouse();
            Task<bool> task = new Task<bool>(session);
            task.Start();

            // Do this code without wait for session response
            label3.Text = "Please wait...";

            //Await for the method result then do block
            // Await task is gonna return if the connection succeed
            if (!await task)
            {
                MessageBox.Show("Error, try again");
                label3.Text = "Error, try again";
            }
            else
            {
                Close();
                th = new Thread(openForm);
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
            }

        }

        private void openForm(object obj)
        {
            Application.Run(new Form1(thisWH));
        }

    }
}
