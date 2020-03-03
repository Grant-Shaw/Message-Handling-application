using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM
{
    abstract class Message
    {


        public abstract string Sender { get; set; }
        public abstract string MessageText { get; set; }

        public Message(string m)
        {

            MessageText = m;


        }


    }
}
