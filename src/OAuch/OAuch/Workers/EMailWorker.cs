using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OAuch.Database;
using OAuch.Helpers;
using OAuch.Shared;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OAuch.Workers {
    public class EMailWorker : IHostedService, IDisposable {
        private readonly CancellationTokenSource _cancellationSource;

        public EMailWorker() {
            _cancellationSource = new CancellationTokenSource();
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StartAsync(CancellationToken cancellationToken) {
#if !DEBUG
            ThreadPool.QueueUserWorkItem(Worker);
#endif
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public async void Worker(object? state) {
            try {
                while (!_cancellationSource.IsCancellationRequested) {
                    var (Message, SessionCount, TestCount) = GetParams();
                    await SendMail(Message, SessionCount, TestCount);

                    var tm = DateTime.Now.Date.AddDays(1).AddHours(8);
                    var ts = tm - DateTime.Now;
                    await Task.Delay(ts, _cancellationSource.Token);
                }
            } catch { }
        }
        private static (string Message, int SessionCount, int TestCount) GetParams() {
            using var db = new OAuchDbContext();
            var lastDay = DateTime.Now.AddDays(-1);
            var sessions = db.UserSessions.Where(us => us.LoggedInAt > lastDay).ToList();
            var tests = db.SerializedTestRuns.Where(str => str.StartedAt > lastDay).Count();
            if (sessions.Count > 0 || tests > 0) {
                var message = new StringBuilder();
                message.AppendLine($"A total of {sessions.Count} new sessions have been recorded in the last 24 hours. They are coming from these IP addresses:");
                foreach (var s in sessions) {
                    message.AppendLine($" - " + (s.RemoteIp ?? "<unknown>"));
                }
                message.AppendLine();
                if (tests > 0) {
                    message.AppendLine($"A total of {tests} test runs were performed.");
                } else {
                    message.AppendLine("No test runs were performed.");
                }
                return (message.ToString(), sessions.Count, tests);
            } else {
                return ("No new sessions or tests were recorded", 0, 0);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _cancellationSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task SendMail(string message, int sessionCount, int testCount) {
            try {
                var secrets = ServiceLocator.Resolve<Secrets>();
                if (secrets == null || !secrets.HasMailSettings())
                    return;
                var mailMessage = new MailMessage(secrets.EMailFrom, secrets.EMailTo, $"OAuch report: {sessionCount} new sessions and {testCount} new tests performed ({DateTime.Now:yyyy-MM-dd})", message) {
                    IsBodyHtml = false
                };
                var client = new SmtpClient(secrets.EMailHost, 587) {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(secrets.EMailFrom, secrets.EMailPassword)
                };
                await client.SendMailAsync(mailMessage, _cancellationSource.Token);
            } catch { }
        }
    }
}