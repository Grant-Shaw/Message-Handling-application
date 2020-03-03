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
using System.Text.RegularExpressions;
using System.IO;


namespace ELM
{
    /// <summary>
    /// A static class which is used to store filepaths and lists which will be written to files and printed later.
    /// </summary>
    public static class MessageFilter
    {
        public static string JSONpath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\jsontest.Json");
        public static string QuarantinePath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\QuarantineList.txt");
        public static string textSpeakPath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\textwords.csv");
        public static string incidentPath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\incidentList.txt");
        public static string SIRpath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\SIR List.txt");
        public static string hashtagPath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\Hashtaglist.txt");
        public static string mentionPath = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "\\MentionList.txt");

        public static List<string> incidentList = new List<string>();
        public static List<string> URLlist = new List<string>();
        public static List<string> mentionList = new List<string>();
        public static List<string> hashtagList = new List<string>();
        public static List<string> hashtagOccurence = new List<string>();
       
        //reads Textspeak abbreviations from file and adds to dictionary.
        public static Dictionary<string, string> dict = File.ReadLines(textSpeakPath).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line[1]);
              public static List<string> incidentDescriptions = File.ReadAllLines(incidentPath).ToList();
          




    }
}
