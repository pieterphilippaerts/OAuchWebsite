using System.Diagnostics.CodeAnalysis;

namespace OAuch.Helpers {
    public class Secrets {
        public string? CaptchaSiteKey { get; set; }
        public  string? CaptchaSiteSecret { get; set; }
        
        public  string? EMailPassword { get; set; }
        public string? EMailFrom { get; set; }
        public string? EMailTo { get; set; }
        public string? EMailHost { get; set; }

        //public string? FacebookClientId { get; set; }
        //public  string? FacebookClientSecret { get; set; }
        
        public  string? MicrosoftClientId { get; set; }
        public  string? MicrosoftClientSecret { get; set; }
        
        public  string? GoogleClientId { get; set; }
        public  string? GoogleClientSecret { get; set; }

        [MemberNotNullWhen(true, nameof (EMailHost), nameof(EMailTo), nameof(EMailFrom), nameof(EMailPassword))]
        public bool HasMailSettings() => EMailHost != null && EMailTo != null && EMailFrom != null && EMailPassword != null;

        //public  string TwitterClientId { get; set; }
        //public  string TwitterClientSecret { get; set; }
    }
}
