namespace Fance.TurnoFixer.Models
{
    public class ChatStatus
    {
        public long ChatId { get; set; }
        
        public ChatStatus(long chatId)
        {
            ChatId = chatId;
        }
    }
}