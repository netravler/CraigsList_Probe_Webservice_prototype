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
using System.Diagnostics;

namespace CraigsList_Probe_Webservice_prototype
{
    public partial class Form1 : Form
    {
        // communities
        string[] communities = { "sss", "eee", "ggg", "hhh", "jjj", "ppp", "res", "ccc", "bbb" };

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

            getCLWorld(); // calls to get the base world URLS this is what we build all other searches from.

            getRegion(); // this needs to be modified to loop thru all pages...

          }

        private void keywordToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void getCLWorld()
        {
            try
            {
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
                        if (lineRTB2.Trim() != "") // no blank lines allowed!!!
                        {
                            richTextBox1.AppendText(pullHTML(lineRTB2) + "\n");
                            richTextBox1.Refresh();
                        }
                    }
                }

                //cleanRTB(richTextBox1, richTextBox3); // ok clean it up and make ready for csv

                // ok we consume rtb1 it should only contain url's.
                // construct this:
                // 
                // http://kansascity.craigslist.org/search/sss?zoomToPosting=&catAbb=sss&query=ss+mm+kk&minAsk=&maxAsk=&sort=rel&excats=

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getRegion()
        {
            // we need to post to each entry in rtb1 gather the data from all the pages then getLocal()
            bool pageNext = true; // we assume true this may be in error
            string[] returnURLString = {"","","","","","","",""};
            string tagURL = "";
            
            while (pageNext == true)
            {
                //
                // next we go through each of the lines in rtb1 looking in each community and keyword combination:
                // (no keyword(s), no search)
                // ccc
                // eee
                // ggg
                // hhh
                // jjj
                // ppp
                // res
                // sss
                // bbb
                //
                // build search string
                foreach (string lineS in richTextBox1.Lines)
                {
                    // clear edit box
                    richTextBox5.Clear();

                    if (!lineS.Contains("about")) // these urls are informational and have no search value
                    {

                        foreach (string comm in communities)
                        {
                            /*
                                Will need to place the following into a loop searching for the work "next".  This
                                indicates that there are additional pages to include.
                             
                                The pages seem to advance in 100 increments and the url is emdedded in the page and
                                may have value in mining out for use in call.
                            */
                            WebRequest request = WebRequest.Create(lineS + @"/search/?sort=rel&areaID=348&subAreaID=&query=" + textBox1.Text + @"&catAbb=" + comm);
                            //request.Timeout = 8000;
                            WebResponse response = request.GetResponse();
                            Stream data = response.GetResponseStream();
                            string html = String.Empty;
                            using (StreamReader sr = new StreamReader(data))
                            {
                                richTextBox5.AppendText(sr.ReadToEnd());
                                // 
                                data.Close();
                            }
                        }

                        // clean the data here.  This won't work for the web service will need to be switched to a list object.

                        foreach (String lineRTB2 in richTextBox5.Lines)
                        {
                            if (lineRTB2.Contains(@"<p class="))
                            {
                                if (lineRTB2.Trim() != "") // no blank lines allowed!!!
                                {
                                    returnURLString[0] = "<a href=";
                                    returnURLString = splitEM(returnURLString, lineRTB2);
                                    //richTextBox2.AppendText(lineRTB2 + "\n");

                                    foreach (string lineEM in returnURLString)
                                    {
                                        if (lineEM.Contains(".html") && lineEM.Contains(@"data-id="))
                                        {
                                            tagURL = lineEM.Substring(1, lineEM.IndexOf("html") + 4);  // we start at 1 to get rid of "

                                            if (lineEM.Contains("http:")) // ok the return sometimes has the full URL so must detect:)
                                            {
                                                richTextBox2.AppendText(tagURL.Replace("\"", "") + Environment.NewLine);
                                            }
                                            else
                                            {
                                                richTextBox2.AppendText(lineS + tagURL.Replace("\"", "") + Environment.NewLine);
                                            }
                                          
                                            richTextBox2.Refresh();
                                        }
                                    }
                                }
                            }
                        }
                        // clear rtb5 before reload of next webgrab
                        richTextBox5.Clear();
                    }
                }
            }
        }

        private void richTextBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void richTextBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int cursorPosition = richTextBox1.SelectionStart;
            int lineIndex = richTextBox1.GetLineFromCharIndex(cursorPosition);
            string lineText = richTextBox1.Lines[lineIndex];

            Process.Start("IExplore.exe", lineText);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private string pullHTML(string inString)
        {
            // base site list
            inString = inString.Substring(inString.IndexOf("href=") + 6);
            inString = inString.Substring(0, inString.IndexOf(@""">"));
            // when it is done
            return inString;
        }

        private string[] splitEM(string [] splitString, string stringToSplit)
        {
            string[] sendBack = stringToSplit.Split(splitString, StringSplitOptions.None);
            return sendBack;
        }
 
    }
}

