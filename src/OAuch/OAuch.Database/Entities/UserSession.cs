﻿using System;

namespace OAuch.Database.Entities {
    public class UserSession {
        public string Scheme { get; set; } = string.Empty;
        public string LoginId { get; set; } = string.Empty;
        public Guid InternalId { get; set; }
        public string? RemoteIp { get; set; }
        public DateTime? LoggedInAt { get; set; }
        public DateTime? TOS { get; set; } // when did we show the user the 'Terms of Service' dialog on the dashboard?
    }
}
