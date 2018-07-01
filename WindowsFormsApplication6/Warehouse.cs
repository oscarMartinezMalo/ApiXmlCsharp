using System;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using WindowsFormsApplication6.test;

namespace WindowsFormsApplication6
{
    public class Warehouse
    {
        private int keyNumber;
        private CSSoapService myCS;
        private string user;

        public bool OpenSession(string user, string password)
        {

            this.myCS = new CSSoapService();
            int key = -1;

            try
            {
                if (myCS.StartSession(user, password, out key) == api_session_error.no_error)
                {
                    
                    this.keyNumber = key;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        public bool EndSession()
        {
            if (myCS.EndSession(keyNumber) == api_session_error.no_error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string[] getWareHouseReceipt(int flag, string searchNumberId)
        {

            string getXml;
            string[] elements = new string[4];
            api_session_error result = api_session_error.no_error;
            try
            {

                result = myCS.GetTransaction(keyNumber, "WH", flag, searchNumberId, out getXml);
                if (result == api_session_error.no_error)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(getXml);
                    XmlElement docXml = doc.DocumentElement;

                    elements[0] = docXml.GetElementsByTagName("CreatedOn")[0].InnerText;
                    elements[1] = docXml.GetElementsByTagName("CreatedByName")[0].InnerText;
                    elements[2] = docXml.GetElementsByTagName("WarehouseReceiptNumber")[0].InnerText;
                    return elements;
                }
                else
                {
                    if (result == api_session_error.transaction_not_found)
                        elements[3] = "Transaction not Found";
                    return elements;
                }

            }
            catch 
            {
                elements[3] = "Error";
                return elements;
            }
        }



        public string saveWareHouseReceipt(string[] elementsCSV)
        {

            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(@"C:\Users\Malo\documents\visual studio 2015\Projects\WindowsFormsApplication6\WindowsFormsApplication6\wh.xml");
                XmlElement root = doc.DocumentElement;

                DateTime myDateTime = DateTime.Now;
                string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd") + "T" + myDateTime.ToString("HH:mm:sszzz");
                root.GetElementsByTagName("CreatedOn")[0].InnerText = sqlFormattedDate;

                root.GetElementsByTagName("Number")[0].InnerText = elementsCSV[9];
                root.GetElementsByTagName("CreatedByName")[0].InnerText = this.user;
                root.GetElementsByTagName("ShipperName")[0].InnerText = elementsCSV[10];
                root.GetElementsByTagName("ConsigneeName")[0].InnerText = elementsCSV[11];
                if (elementsCSV[0] == "On Hand") elementsCSV[0] = "OnHand";
                root.GetElementsByTagName("Status")[0].InnerText = elementsCSV[0];
                root.GetElementsByTagName("PackageName")[0].InnerText = elementsCSV[1];
                root.GetElementsByTagName("Pieces")[0].InnerText = elementsCSV[3];
                root.GetElementsByTagName("Description")[0].InnerText = elementsCSV[2];
                root.GetElementsByTagName("Length")[0].InnerText = elementsCSV[4];
                root.GetElementsByTagName("Height")[0].InnerText = elementsCSV[5];
                root.GetElementsByTagName("Width")[0].InnerText = elementsCSV[6];
                root.GetElementsByTagName("Weight")[0].InnerText = elementsCSV[7];
                root.GetElementsByTagName("Volume")[0].InnerText = elementsCSV[8];

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                    doc.WriteTo(xmlTextWriter);
                    string stringXml = stringWriter.ToString();
                    int flag = 0x00000000;
                    string transXml = "";
                    api_session_error result;
                    result = myCS.SetTransaction(keyNumber, "WH", flag, stringXml, out transXml);
                    if (result == test.api_session_error.no_error)
                    {
                        return "Record saved";
                    }
                    else
                    {
                        return "Something went wrong";
                    }
                }              

            }
            catch (Exception)
            {
                return "Something went wrong";
            }
        }

        static void WareHouseSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                MessageBox.Show("warning");
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                MessageBox.Show("error");
            }
        }

    }
}
