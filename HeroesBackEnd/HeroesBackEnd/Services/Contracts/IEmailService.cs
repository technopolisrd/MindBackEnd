namespace MindBackEnd.Services.Contracts;

#nullable disable

public interface IEmailService
{
    void Send(string to, string subject, string html, string from = null);
}