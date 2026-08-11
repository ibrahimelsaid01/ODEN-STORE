using System.ComponentModel.DataAnnotations;

namespace StoreOde.Infrastructure.Email
{
    public sealed class SmtpEmailOptions : IValidatableObject
    {
        public const string SectionName = "Email";

        public const int HostMaxLength = 253;
        public const int EmailMaxLength = 254;
        public const int UserNameMaxLength = 320;
        public const int FromNameMaxLength = 100;

        [Required]
        [StringLength(HostMaxLength)]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int Port { get; set; } = 587;

        public SmtpSecurityMode SecurityMode { get; set; }
            = SmtpSecurityMode.StartTls;

        public bool RequireAuthentication { get; set; } = true;

        [StringLength(UserNameMaxLength)]
        public string? UserName { get; set; }

        public string? Password { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(EmailMaxLength)]
        public string FromEmail { get; set; } = string.Empty;

        [StringLength(FromNameMaxLength)]
        public string? FromName { get; set; }

        [Range(1, 120)]
        public int TimeoutSeconds { get; set; } = 30;

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                yield return new ValidationResult(
                    "SMTP host is required.",
                    new[] { nameof(Host) });
            }

            if (string.IsNullOrWhiteSpace(FromEmail))
            {
                yield return new ValidationResult(
                    "The sender email address is required.",
                    new[] { nameof(FromEmail) });
            }

            if (!RequireAuthentication)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult(
                    "SMTP username is required when authentication is enabled.",
                    new[] { nameof(UserName) });
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "SMTP password is required when authentication is enabled.",
                    new[] { nameof(Password) });
            }
        }
    }

    public enum SmtpSecurityMode
    {
        None = 0,
        StartTls = 1,
        SslOnConnect = 2
    }
}