using System.Net.Mail;

namespace Fiap.FileCut.Core.Objects;

public class NotifyContext<T> (T context, Guid userId, MailAddress mail)
{
    public readonly T Context = context;

    public readonly Guid UserId = userId;

    public readonly MailAddress Mail = mail;
}