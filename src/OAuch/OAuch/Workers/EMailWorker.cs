using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OAuch.Database;
using OAuch.Helpers;
using OAuch.Shared;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OAuch.Workers {
    public class EMailWorker : IHostedService, IDisposable {
        private Timer timer;
        private int number;

        private CancellationTokenSource _cancellationSource;

        public EMailWorker() {
            _cancellationSource = new CancellationTokenSource();
        }

        public void Dispose() {
            //
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
#if !DEBUG
            ThreadPool.QueueUserWorkItem(Worker);
#endif
        }
        public async void Worker(object? state) {
            try {
                while (!_cancellationSource.IsCancellationRequested) {
                    var ret = GetParams();
                    await SendMail(ret.Message, ret.Count);

                    var tm = DateTime.Now.Date.AddDays(1).AddHours(8);
                    var ts = tm - DateTime.Now;
                    await Task.Delay(ts, _cancellationSource.Token);
                }
            } catch { }
        }
        private (string Message, int Count) GetParams() {
            using (var db = new OAuchDbContext()) {
                var lastDay = DateTime.Now.AddDays(-1);
                var sessions = db.UserSessions.Where(us => us.LoggedInAt > lastDay).ToList();
                if (sessions.Count > 0) {
                    var message = new StringBuilder();
                    message.AppendLine($"A total of {sessions.Count} new sessions have been recorded in the last 24 hours. They are coming from these IP addresses:");
                    foreach (var s in sessions) {
                        message.AppendLine($" - " + (s.RemoteIp ?? "<unknown>"));
                    }
                    return (message.ToString(), sessions.Count);
                } else {
                    return ("No new sessions were recorded", 0);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _cancellationSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task SendMail(string message, int count) {
            try {
                var secrets = ServiceLocator.Resolve<Secrets>();
                var mailMessage = new MailMessage(secrets.EMailFrom, secrets.EMailTo, $"OAuch report: {count} new sessions ({DateTime.Now.ToString("yyyy-MM-dd")})", message);
                mailMessage.IsBodyHtml = false;
                var client = new SmtpClient(secrets.EMailHost, 587);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(secrets.EMailFrom, secrets.EMailPassword);
                await client.SendMailAsync(mailMessage, _cancellationSource.Token);
            } catch (Exception e) { }
        }
    }
}