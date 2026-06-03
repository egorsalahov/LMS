namespace EgorSalahovSemestrovka22.Models.Entities
{
    namespace EgorSalahovSemestrovka22.Models.Entities
    {
        public class Message
        {
            public int Id { get; set; }
            public string SenderId { get; set; }   
            public string ReceiverId { get; set; }  
            public string Content { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.Now;
            public bool IsRead { get; set; } = false;

            public Student Sender { get; set; }
            public Student Receiver { get; set; }
        }
    }
}
