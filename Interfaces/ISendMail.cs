namespace ShoppingOnline.Interfaces
{
    public interface ISendMail
    {
        Task<bool> SendMail(string name, string subject, string content, string toMail);
    }
}
