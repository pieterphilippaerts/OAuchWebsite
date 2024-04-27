using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OAuch.Helpers {
    public static class SiteInfo {
        public static string BuildVersion {
            get {
                if (_buildVersion == null) {
                    var v = Assembly.GetExecutingAssembly().GetName().Version ?? new Version("1.0");
                    var bd = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);
                    _buildVersion = bd.ToString("yyyy.MM.dd");
#if DEBUG
                    _buildVersion += "d";
#endif
                }
                return _buildVersion;
            }
        }
        private static string? _buildVersion;

        private static string? _copyright;
        public static string Copyright {
            get {
                _copyright ??= $"Copyright © {DateTime.Now.Year}";
                return _copyright;
            }
        }
    }
}
