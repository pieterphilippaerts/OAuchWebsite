using Microsoft.AspNetCore.Mvc;
using System;

namespace OAuch.Controllers {
    public abstract class BaseController : Controller {
        public Guid? OAuchInternalId {
            get {
                if (_oauchInternalId == null && this.User != null) {
                    var claim = this.User.FindFirst(LoginController.OAuchInternalIdClaimType);
                    if (claim != null && Guid.TryParseExact(claim.Value, "N", out var internalId)) {
                        _oauchInternalId = internalId;
                    }
                }
                return _oauchInternalId;
            }
        }
        private Guid? _oauchInternalId;
    }
}