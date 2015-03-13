using ICities;

namespace FreeMoveChirper
{
    public class ChirpMessage : IChirperMessage
    {
        //ChirperMessage Template by /u/SkylinesModEngineer

        private string message;
        private string sender;
        private uint id;

        public ChirpMessage(string sender, string message, uint id)
        {
            this.sender = sender;
            this.message = message;
            this.id = id;
        }

        public ChirpMessage(string message)
        {
            this.message = message;
            this.sender = "Chirper";
            this.id = 0;
        }

        public void SendMessage()
        {
            ChirpPanel.instance.AddMessage(this);
        }

        public static void SendMessage(string message)
        {
            new ChirpMessage(message).SendMessage();
        }

        public uint senderID
        {
            get { return id; }
        }

        public string senderName
        {
            get { return sender; }
        }

        public string text
        {
            get { return message; }
        }
    }
}
