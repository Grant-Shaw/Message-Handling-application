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
    /// Class for email messages
    /// </summary>
    class Email : Message
    {
        //fields
        private string sender;
        private string messageText;
        private string subject;
        private string messagetype;
        
        
        

        //properties

        public string MessageType
        {
            get 
            { return messagetype; }
            set { messagetype = value; }
        }


        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }
     
        public override string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        public override string MessageText
        {
            get { return messageText; }
            set
            {
                if (value.Length > 1028)
                {
                    throw new Exception("Email cannot be longer than 1028 characters for a tweet");
                }
                else
                {
                    messageText = value;
                }
            }
        }



        //constructor for Email
        public Email(string m) : base(m)
        {
            try
            {
                MessageText = m;
                this.FindSender();
                this.FindSubject();
                if (MessageType == "SIR")
                {
                    NatureOfIncident(centreCodeFinder());   
                    
                }
                this.QuarantineURLs();
                
                foreach (var entry in MessageFilter.dict)
                {
                    MessageText = MessageText.Replace(" " + entry.Key + " ", " " + entry.Key + "<" + entry.Value + ">");
                }

            }
            catch(Exception n)
            {
                MessageBox.Show(n.Message);

            }
                        
        }


        //a method which adds the incident code and the nature of the incident to a list.
        private void NatureOfIncident(string code)
        {
            try
            {
                //using LINQ to find any matches within the messageText which match valid incident descriptions.
                var result = MessageFilter.incidentDescriptions.Where(t => MessageText.Contains(t)).ToList();
                //format string for SIR list to contain centre code and nature of incident.                
                string incident = string.Format("Sport centre Code: {0},  Nature of Incident: {1}", code, result[0]);
                        MessageFilter.incidentList.Add(incident);     
                
            }
            catch(Exception U)
            {
                throw new Exception("Nature of incident invalid, do not press finished until fixed.");
            }



        }

        //method for finding the centre code.
        private string centreCodeFinder()
        {
            try
            {                  
                   //searches for centre code matches in the text in format xx-xxx-xxx
                   Regex centreCodeFinder = new Regex(@"\d{2}-\d{3}-\d{3}");
                MatchCollection centreCodeMatches = centreCodeFinder.Matches(MessageText);
               
                    string centreCode = centreCodeMatches[0].Value;
                     return centreCode;
                                        
            }
            catch(Exception h)
            {               
                throw new Exception("centre code invalid");               
            }       
        }

        //finds the subject of the Email by using substrings
        private void FindSubject()
        {

            try
            {
                var subject1 = MessageText.Substring(0, 21);


                if (subject1.Contains("SIR"))
                {
                    MessageType = "SIR";
                    Subject = subject1.Substring(0, 14);
                    MessageText = MessageText.Replace(Subject, "");
                }
                else
                {
                    MessageType = "Email";
                    subject1 = subject1.Split('.')[0];
                    Subject = subject1;
                    MessageText = MessageText.Replace(Subject, "");
                    MessageText = MessageText.Substring(1);

                }
            }
            catch(Exception v)
            {
                throw new Exception("Please ensure that your subject is in either standard (less than 20 characters) or SIR format");
             }
            
        }


        //adds email addresses within messagetext to quarantine list.
        private void QuarantineURLs()
        {

            Regex URLRegex = new Regex(@"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?", RegexOptions.IgnoreCase);

            MatchCollection matches = URLRegex.Matches(MessageText);

            foreach (Match m in matches)
            {
                if (MessageFilter.URLlist.Contains(m.Value))
                {
                    continue;
                }
                else
                {
                    MessageFilter.URLlist.Add(m.Value);
                    
                }
            }


            foreach (string URL in MessageFilter.URLlist)
            {
                MessageText = MessageText.Replace(URL, "" + "<This URL has been quarantined> ");
            }

        }

        //method which finds the sender of the Email by using regex to search for first email address.

        private void FindSender()
        {

            //var EmailRegex = new Regex(@"(?:(?:\+?([1-9]|[0-9][0-9]|[0-9][0-9][0-9])\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([0-9][1-9]|[0-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?", RegexOptions.IgnoreCase);

            try
            {
                
                //regex which finds email addresses
                Regex EmailRegex = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}", RegexOptions.IgnoreCase);
                //
                //add all found email addresses to a MatchCollection
                MatchCollection matches = EmailRegex.Matches(MessageText);

                //make the Sender the first email address found

                Sender = matches[0].Value;

                //add all other email addresses to a list
                
                MessageText = MessageText.Replace(Sender, " ");
                //MessageText = MessageText.Remove(0, 11);
            }

            catch(Exception v)
            {
                throw new Exception("Please ensure that the senders email is a valid email address");
            }
            


        }
    }
}
