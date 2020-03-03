using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace ELM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //creates an object which stores a list of XML objects
        XMLMessageList XmlData;
        XMLDeserializer DataProcessor;      
        List<SMS> SMSMessageList = new List<SMS>();
        List<Email> emailMessageList = new List<Email>();
        List<Tweet> tweetMessageList = new List<Tweet>();

        int i = 0;
        int y = 0;

        public MainWindow()
        {
            //create a new Serialization object
            //use the Serialization object's deserialize method to read the data from XML file and add to list in XmlData object
            try
            {
                DataProcessor = new XMLDeserializer();
                XmlData = DataProcessor.deserializeXML();
                MessageFilter.dict.Remove("EMA");
                InitializeComponent();

                inputHeader.Text = XmlData.messageList[i].Header;
                inputBody.Text = XmlData.messageList[y].Body;
            }

            catch(Exception t)
            {
                MessageBox.Show(t.Message);
            }


        }

        private void NxtMsg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                inputHeader.Text = XmlData.messageList[i += 1].Header;
            }
            catch (Exception) { MessageBox.Show("No more messages to display"); }

            try
            {
                inputBody.Text = XmlData.messageList[y += 1].Body;
            }
            catch (Exception) { MessageBox.Show("No more messages to display"); }


        }

        //on button click , determines the Message type by the MessageID
        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
           

            try
            {
                //reads the first character from the header and determines which object type to create based on that.
                char[] headerArray = inputHeader.Text.ToCharArray();


                if (headerArray[0] == 'T')
                {
                    //creates a message object for validation purposes.
                    Tweet newTweet = new Tweet(Convert.ToString(inputBody.Text));

                    if (newTweet.Sender == null)
                    {
                        throw new Exception("Message cannot be processed , please ensure tweet has a valid sender");
                    }
                    else
                    {
                        tweetMessageList.Add(newTweet);
                        //tweetMessageList.Add(new Tweet(Convert.ToString(inputBody.Text)));
                        string jsonTweet = JsonConvert.SerializeObject(newTweet, Formatting.Indented);
                        outputBody.Text = jsonTweet;

                    }
                }

                if (headerArray[0] == 'E')
                {
                    Email newEmail = new Email(Convert.ToString(inputBody.Text));
                    if (newEmail.Sender == null)
                    {
                        throw new Exception("Message cannot be processed, Please ensure email has a subject and a sender");
                    }
                    else
                    {
                        emailMessageList.Add(newEmail);
                        string jsonEmail = JsonConvert.SerializeObject(newEmail, Formatting.Indented);
                        outputBody.Text = jsonEmail;
                    }

                }

                if (headerArray[0] == 'S')
                {
                    //create SMS object , serialize to Json and add to list of Json Strings

                    SMS newSMS = new SMS(inputBody.Text);
                    if (newSMS.Sender == null)
                    {
                        throw new Exception("Message cannot be stored, no sender found");
                    }
                    else
                    {                    
                            SMSMessageList.Add(newSMS);
                            string jsonSMS = JsonConvert.SerializeObject(newSMS, Formatting.Indented);
                            outputBody.Text = jsonSMS;
                                              
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Message);
            }

        }

        //this saved everything to JSON file , creates a list of quarantined emails and creates a serious incident report.
        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            outputBody.Text = "";
            string lists = "";

            //add show numbers of occurences of a hashtag.
            foreach (var grp in MessageFilter.hashtagList.GroupBy(i => i))
            {
                MessageFilter.hashtagOccurence.Add(grp.Key + ":  " + grp.Count());
            }


            lists += "Hashtag List: \n";
            foreach(string x in MessageFilter.hashtagOccurence)
            {
                lists += x + "\n";
            }
            lists += "Mention list: \n";
            foreach(string x in MessageFilter.mentionList)
            {
                lists += x + "\n";
            }
            lists += "SIR list: \n";
            foreach(string x in MessageFilter.incidentList)
            {
                lists += x + "\n";
            }
            outputBody.Text = lists;




            //writes all json messages to file.
            using (StreamWriter file = new StreamWriter(MessageFilter.JSONpath, true))
            {
                file.WriteLine("This is a compiled list of messages");
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                if (SMSMessageList != null)
                {
                    serializer.Serialize(file, SMSMessageList);
                }
                
                if (emailMessageList != null)
                {foreach (var s in emailMessageList)
                    {                    
                        serializer.Serialize(file, s);
                    }
                }
                if (tweetMessageList != null)
                { serializer.Serialize(file, tweetMessageList); }

                file.Close();
            }
           
            //writes all quarantines URL's to file
            using (StreamWriter file2 = new StreamWriter(MessageFilter.QuarantinePath, true))
            {
                file2.WriteLine("URL's quarantined during session: " + DateTime.Now);
                file2.WriteLine();
                foreach (String s in MessageFilter.URLlist)
                    file2.WriteLine(s);              
                file2.Close();
            }

            //writes list of SIR to file.
            using (StreamWriter file3 = new StreamWriter(MessageFilter.SIRpath, true))
            {

                file3.WriteLine("Incidents that occurred during session: " + DateTime.Now);                
                for(int x = 0; x < MessageFilter.incidentList.Count; x+=2)
                    file3.WriteLine(MessageFilter.incidentList[x]);

            }

            //writes list of hashtags to file.
            using (StreamWriter hashtags = new StreamWriter(MessageFilter.hashtagPath, true))
            {
                hashtags.WriteLine("Hashtag trends: " + DateTime.Now);
                for (int x = 0; x < MessageFilter.hashtagOccurence.Count; x+= 1)
                    hashtags.WriteLine(MessageFilter.hashtagOccurence[x]);
            }

            using (StreamWriter mentions = new StreamWriter(MessageFilter.mentionPath, true))
            {
                mentions.WriteLine("Mentions: " + DateTime.Now);
                for (int x = 0; x < MessageFilter.mentionList.Count; x += 1)
                    mentions.WriteLine(MessageFilter.mentionList[x]);
            }

                MessageBox.Show("Saved to JSON");

        }

        //stop the user from going back too far.
        private void PrevMsg_Click(object sender, RoutedEventArgs e)
        {
            if (i > 0)
            {
                inputHeader.Text = XmlData.messageList[i -= 1].Header;
                inputBody.Text = XmlData.messageList[y -= 1].Body;
            }
            else
            {
                MessageBox.Show("You cannot go back further.");
            }
        }
    }
}
