using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkerService
{
    public class AppSettings
    {
        public string TelegramBotToken { get; init; } = string.Empty;
        public string BitrixApiUrl { get; init; } = string.Empty;
        public string BitrixApiLogin { get; init; } = string.Empty;
        public string BitrixApiPassword { get; init; } = string.Empty;

        public string BackSymbol { get; init; } = string.Empty;
        public string HomeSymbol { get; init; } = string.Empty;
        public string CallBackDataPrefix { get; init; } = string.Empty;
        public string CheckIdUrl { get; init; } = string.Empty;
        public string GreetingText { get; init; } = string.Empty;
        public string SQLConnection { get; init; } = string.Empty;
        public string HeadId { get; init; } = string.Empty;
    }
}
