using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace StoreOde.Infrastructure.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpEmailOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(
            IOptions<SmtpEmailOptions> options,
            ILogger<SmtpEmailSender> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentNullException.ThrowIfNull(logger);

            _options = options.Value;
            _logger = logger;

            ValidateRuntimeConfiguration(_options);
        }

        public async Task SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(subject);
            ArgumentException.ThrowIfNullOrWhiteSpace(htmlMessage);

            if (subject.Contains('\r') ||
                subject.Contains('\n'))
            {
                throw new ArgumentException(
                    "Email subject cannot contain line breaks.",
                    nameof(subject));
            }

            var message = CreateMessage(
                email,
                subject,
                htmlMessage);

            var socketOptions =
                GetSecureSocketOptions(
                    _options.SecurityMode);

            using var smtpClient = new SmtpClient();

            smtpClient.Timeout =
                checked(_options.TimeoutSeconds * 1000);

            using var operationTimeout =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(
                        _options.TimeoutSeconds));

            try
            {
                await smtpClient.ConnectAsync(
                    _options.Host.Trim(),
                    _options.Port,
                    socketOptions,
                    operationTimeout.Token);

                if (_options.RequireAuthentication)
                {
                    await smtpClient.AuthenticateAsync(
                        _options.UserName!,
                        _options.Password!,
                        operationTimeout.Token);
                }

                await smtpClient.SendAsync(
                    message,
                    operationTimeout.Token);

                _logger.LogDebug(
                    "An email message was delivered to the configured SMTP server.");
            }
            catch (OperationCanceledException exception)
                when (operationTimeout.IsCancellationRequested)
            {
                throw new TimeoutException(
                    "The SMTP operation exceeded the configured timeout.",
                    exception);
            }
            finally
            {
                await DisconnectSafelyAsync(
                    smtpClient);
            }
        }

        private MimeMessage CreateMessage(
            string recipientEmail,
            string subject,
            string htmlMessage)
        {
            var recipient =
                ParseRecipientAddress(
                    recipientEmail);

            var sender =
                CreateSenderAddress();

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            var message = new MimeMessage();

            message.From.Add(sender);
            message.To.Add(recipient);

            message.Subject = subject;
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        private MailboxAddress CreateSenderAddress()
        {
            if (!MailboxAddress.TryParse(
                    _options.FromEmail.Trim(),
                    out var parsedAddress) ||
                parsedAddress is null)
            {
                throw new InvalidOperationException(
                    "The configured sender email address is invalid.");
            }

            if (string.IsNullOrWhiteSpace(
                _options.FromName))
            {
                return parsedAddress;
            }

            return new MailboxAddress(
                _options.FromName.Trim(),
                parsedAddress.Address);
        }

        private static MailboxAddress ParseRecipientAddress(
            string email)
        {
            if (!MailboxAddress.TryParse(
                    email.Trim(),
                    out var recipient) ||
                recipient is null)
            {
                throw new ArgumentException(
                    "The recipient email address is invalid.",
                    nameof(email));
            }

            return recipient;
        }

        private async Task DisconnectSafelyAsync(
            SmtpClient smtpClient)
        {
            if (!smtpClient.IsConnected)
            {
                return;
            }

            try
            {
                var disconnectTimeoutSeconds =
                    Math.Min(
                        _options.TimeoutSeconds,
                        10);

                using var disconnectTimeout =
                    new CancellationTokenSource(
                        TimeSpan.FromSeconds(
                            disconnectTimeoutSeconds));

                await smtpClient.DisconnectAsync(
                    quit: true,
                    disconnectTimeout.Token);
            }
            catch (Exception exception)
            {
                /*
                 * A disconnect failure must not hide the result of the
                 * original send operation.
                 *
                 * Do not log SMTP credentials or recipient information.
                 */
                _logger.LogWarning(
                    exception,
                    "The SMTP connection could not be closed cleanly.");
            }
        }

        private static SecureSocketOptions GetSecureSocketOptions(
            SmtpSecurityMode securityMode)
        {
            return securityMode switch
            {
                SmtpSecurityMode.None
                    => SecureSocketOptions.None,

                SmtpSecurityMode.StartTls
                    => SecureSocketOptions.StartTls,

                SmtpSecurityMode.SslOnConnect
                    => SecureSocketOptions.SslOnConnect,

                _ => throw new InvalidOperationException(
                    $"Unsupported SMTP security mode: {securityMode}.")
            };
        }

        private static void ValidateRuntimeConfiguration(
            SmtpEmailOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Host))
            {
                throw new InvalidOperationException(
                    "SMTP host configuration is missing.");
            }

            if (options.Port is < 1 or > 65535)
            {
                throw new InvalidOperationException(
                    "SMTP port configuration is invalid.");
            }

            if (options.TimeoutSeconds is < 1 or > 120)
            {
                throw new InvalidOperationException(
                    "SMTP timeout configuration is invalid.");
            }

            if (string.IsNullOrWhiteSpace(
                options.FromEmail))
            {
                throw new InvalidOperationException(
                    "SMTP sender email configuration is missing.");
            }

            if (!MailboxAddress.TryParse(
                    options.FromEmail.Trim(),
                    out _))
            {
                throw new InvalidOperationException(
                    "SMTP sender email configuration is invalid.");
            }

            if (!Enum.IsDefined(options.SecurityMode))
            {
                throw new InvalidOperationException(
                    "SMTP security mode configuration is invalid.");
            }

            if (!options.RequireAuthentication)
            {
                return;
            }

            if (options.SecurityMode ==
                SmtpSecurityMode.None)
            {
                throw new InvalidOperationException(
                    "SMTP authentication requires an encrypted connection.");
            }

            if (string.IsNullOrWhiteSpace(
                options.UserName))
            {
                throw new InvalidOperationException(
                    "SMTP username configuration is missing.");
            }

            if (string.IsNullOrWhiteSpace(
                options.Password))
            {
                throw new InvalidOperationException(
                    "SMTP password configuration is missing.");
            }
        }
    }
}