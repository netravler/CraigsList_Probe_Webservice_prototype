using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//
using System.Net;
using System.IO;

namespace CraigsList_Probe_Webservice_prototype
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void bulkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getCLWorld();
        }

        private void keywordToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void getCLWorld()
        {
            //try
            //{
                WebRequest request = WebRequest.Create(@"http://www.craigslist.org/about/sites");
                    WebResponse response = request.GetResponse();
                    Stream data = response.GetResponseStream();
                    string html = String.Empty;
                    using (StreamReader sr = new StreamReader(data))
                    {
                        richTextBox5.AppendText(sr.ReadToEnd());
                        // 
                        data.Close();
                }

                // clean the data here.  This won't work for the web service

                foreach (String lineRTB2 in richTextBox5.Lines)
                {
                    if (lineRTB2.Contains("<li><a href="))
                    {
                        //lineRTB2 = cleanRTB();

                        // remember to remove the following:

                        //www.craigslist.org/about/help/
                        //www.craigslist.org/about/scams
                        //www.craigslist.org/about/terms.of.use
                        //www.craigslist.org/about/
                        if (lineRTB2.Trim() != "") // no blank lines allowed!!!
                        {
                            richTextBox1.AppendText(lineRTB2 + "\n");
                            richTextBox1.Refresh();
                        }
                    }
                }

                cleanRTB(richTextBox1, richTextBox3); // ok clean it up and make ready for csv

                // ok we consume rtb1 it should only contain url's.
                // construct this:
                // 
                // http://kansascity.craigslist.org/search/sss?zoomToPosting=&catAbb=sss&query=ss+mm+kk&minAsk=&maxAsk=&sort=rel&excats=

            //}
            //catch
            //{
                richTextBox1.AppendText("ERRERRERRERRERRERRERR");
            //}
        }

        private void cleanRTB(RichTextBox rtbClean, RichTextBox baseBox)
        {
            // we will consume richTextBox1 line by line into richTextBox2 then clear richTextBox2 and move back over.
            // yes there are other ways of doing this but I'm frigg'in lazy:)

            foreach (string lineInRTB1 in rtbClean.Lines)
            {
                baseBox.AppendText(pullHTML(lineInRTB1));
                baseBox.AppendText(Environment.NewLine);
                baseBox.Refresh();
            }

            // move the finished product to richTextBox1
            rtbClean.Clear();

            foreach (string lineInRTB2 in baseBox.Lines)
            {
                rtbClean.AppendText(lineInRTB2);
                rtbClean.AppendText(Environment.NewLine);
                baseBox.Refresh();
            }

        }

        private string pullHTML(string inString)
        {
            // base site list
            inString = inString.Substring(inString.IndexOf("href=") + 6);
 
            //inString = inString.Replace("<td>", "");
            //inString = inString.Replace("<td title=", "");
            //inString = inString.Replace("</td>", "");

            //inString = inString.Replace("&amp;", "");

            inString = inString.Substring(0, inString.IndexOf(@""">"));

            // when it is done
            return inString;

            // local site list

            // specific page entries. 
            // only pull links that match keyword bulk should bypass
        }
    }
}

