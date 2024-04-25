﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.Database.Entities {
    public class Site {
        [Key]
        public Guid SiteId { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CurrentConfigurationJson { get; set; } = string.Empty;
        public Guid? LatestResultId { get; set; }

    }
}
