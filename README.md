# Message-Handling-application
This application reads messages in XML format or user defined and then converts them to JSON. 


Allows a user to enter 4 types of Messages
- Text
- SMS
- SIR (Special incident report)
- Email

Upon starting the application will attempt to read a file in the program folder which could contain these messages in XML format.
When the page is displayed the user will be able to cycle between these pre-existing message or alternatively they will be able to type their own.

The message type will be detected if the user inputs it in the correct format, it will then be serialized to a Json file in the correct format.

When the finish button is clicked a number of files will be created.
- A Json file containing the messages
- A hashtag list, which tracks which hashtags were used in the messages and how many times
- A mention list which will track any @ twitter mentions used.
- A quarantined list which will contain any URL's quarantined from EMails
- An SIR list, which will contain any serious incidents. This includes the time, incident code and description of incident.
