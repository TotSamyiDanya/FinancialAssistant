using FinancialAssistant.Models;
using Newtonsoft.Json.Linq;

namespace FinancialAssistant.Services
{
    public class MoexService
    {
        private static MoexService instance;
        private Timer MoexLoadToolsTimer;
        private Timer MoexLoadMarketsBoardsFiles;
        private MoexService() {}
        public static MoexService getInstance() => instance ?? new();
        private void LoadTools(object state)
        {
            //LoadTools();
            //Thread.Sleep(8000);
        }
        private void LoadMarketsBoardsFiles(object state)
        {
            //LoadMarkets();
            //Thread.Sleep(8000);
            //LoadAllBoards();
            //Thread.Sleep(8000);
            //LoadSamples();
            //Thread.Sleep(8000);
        }
        public void MoexTimerTools() => MoexLoadToolsTimer = new(LoadTools!, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        public void MoexTimerMarketsBoardsFiles() => MoexLoadMarketsBoardsFiles = new(LoadMarketsBoardsFiles!, null, TimeSpan.Zero, TimeSpan.FromDays(30));



        public void LoadTools()
        {
            Dictionary<string, string> markets = CreateUrlForSamples();
            foreach(string market in markets.Keys)
            {
                Deserialize(market);
                Thread.Sleep(8000);
            }
        }
        public void Deserialize(string market)
        {
            List<string> files = LoadFiles(market)!;
            Dictionary<string, int> columns = FindColumns(market);

            foreach (string file in files)
            {
                JObject jobj = JObject.Parse(file);
                if (jobj != null)
                    foreach (JToken token in jobj["history"]!["data"]!)
                        if (token != null)
                        {
                            using FinancialAssistantContext db = new();
                            Tool dbTool = new()
                            {
                                ToolName = token[columns["SHORTNAME"]]!.Value<string?>() ?? "none",
                                SecId = token[columns["SECID"]]!.Value<string?>() ?? "none",
                                Market = db.Markets.Where(m => m.MarketName == market).FirstOrDefault(),
                                Board = db.Boards.Where(b => b.BoardName.Substring(0, 4) == token[columns["BOARDID"]]!.Value<string?>()).FirstOrDefault()
                            };
                            if (!db.Tools.Any(t => t.ToolName == dbTool.ToolName & t.SecId == dbTool.SecId & t.Board == dbTool.Board & t.Market == dbTool.Market))
                            {
                                db.Tools.Add(dbTool);
                                db.SaveChanges();
                            }
                            TradeDate dbtradeDate = new()
                            {
                                Tool = db.Tools.Where(t => t.ToolName == dbTool.ToolName & t.SecId == dbTool.SecId & t.Board == dbTool.Board & t.Market == dbTool.Market).FirstOrDefault(),
                                Date = token[columns["TRADEDATE"]]!.Value<string?>() ?? "none",
                                Price = token[columns["CLOSE"]]!.Value<double?>() ?? 0
                            };
                            if (!db.TradeDates.Any(td => td.Date == dbtradeDate.Date && td.Tool.ToolName == dbtradeDate.Tool!.ToolName && td.Tool.Board.BoardName == dbtradeDate.Tool.Board!.BoardName))
                            {
                                db.TradeDates.Add(dbtradeDate);
                                db.SaveChanges();
                            }
                        }
            }

        }
        public void LoadMarkets()
        {
            using HttpResponseMessage responce = new HttpClient().GetAsync("https://iss.moex.com/iss/engines/stock/markets.json").Result;
            if (responce.IsSuccessStatusCode)
            {
                string result = responce.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(result))
                {
                    File.WriteAllText(@"Json\markets.json", result);
                    JObject jObj = JObject.Parse(result);
                    if (jObj != null)
                        foreach (JToken token in jObj["markets"]!["data"]!)
                        {
                            if (token != null)
                            {
                                using FinancialAssistantContext db = new();
                                Market market = new() { MarketName = token[1]!.Value<string>()};
                                if (!db.Markets.Any(m => m.MarketName == market.MarketName))
                                {
                                    db.Markets.Add(market);
                                    db.SaveChanges();
                                }
                            }
                        }
                }
            }
        }
        public void LoadAllBoards()
        {
            Dictionary<string, string> markets = CreateUrlForSamples();
            foreach(var market in markets) 
            {
                LoadBoards(market.Key);
                Thread.Sleep(8000);
            }
        }
        public void LoadBoards(string market)
        {
            using HttpResponseMessage responce = new HttpClient().GetAsync($"https://iss.moex.com/iss/engines/stock/markets/{market}/boards.json").Result;
            if (responce.IsSuccessStatusCode)
            {
                string result = responce.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(result))
                {
                    File.WriteAllText(@$"Json\{market}_boards.json", result);
                    JObject jObj = JObject.Parse(result);
                    if (jObj != null)
                        foreach (JToken token in jObj["boards"]!["data"]!)
                        {
                            if (token != null)
                            {
                                using FinancialAssistantContext db = new();
                                {
                                    Board board = new Board() { 
                                        BoardName = token[2]!.Value<string>() + "|" + token[3]!.Value<string>(),
                                        Market = db.Markets.Where(m => m.MarketName == market).FirstOrDefault()
                                    };
                                    if (!db.Boards.Any(b => b.BoardName == board.BoardName && b.Market.MarketName == board.Market!.MarketName))
                                    {
                                        db.Boards.Add(board);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                }
            }
        }
        public void LoadSamples()
        {
            Dictionary<string, string> samples = CreateUrlForSamples();
            foreach (var sample in samples)
            {
                using HttpResponseMessage response = new HttpClient().GetAsync(sample.Value.ToString()).Result;
                string file = response.Content.ReadAsStringAsync().Result;
                if (!String.IsNullOrEmpty(file))
                    File.WriteAllText(@$"Json\Samples\Sample_{sample.Key}.json", file);
                Thread.Sleep(2000);
            }
        }
        private Dictionary<string,string> CreateUrlForSamples()
        {
            Dictionary<string,string> result = new Dictionary<string, string>();
            DateTime today = DateTime.Now;
            using FinancialAssistantContext db = new();
            var markets = db.Markets.ToList();
            foreach(var market in markets)
                result.Add(market.MarketName!, CreateUrl(market.MarketName!, today.ToString("yyy-MM-dd")));
            return result;
        }
        private List<string>? LoadFiles(string market)
        {
            List<string> result = new();
            DateTime today = DateTime.Now;
            List<string> dates = CreateDates(today);
            foreach (string date in dates)
            {
                string? file = LoadFile(CreateUrl(market, date));
                result.Add(file!);
                Thread.Sleep(1000);
            }
            return result;
        }
        private string? LoadFile(string Url)
        {
            using HttpClient client = new();
                client.Timeout = TimeSpan.FromMinutes(1);
                using HttpResponseMessage responce = client.GetAsync(Url).Result;
                {
                    if (responce.IsSuccessStatusCode)
                    {
                        string file = responce.Content.ReadAsStringAsync().Result;
                        if (!String.IsNullOrEmpty(file))
                            return file;
                    }
                }
            return null;
        }
        private Dictionary<string, int> FindColumns(string market)
        {
            Dictionary<string, int> result = new();
            string file = File.ReadAllText(@$"Json\Samples\Sample_{market}.json");
            JObject jObj = JObject.Parse(file);
            int i = 0;
            if (jObj != null)
                foreach (JToken token in jObj["history"]!["columns"]!)
                    if (token != null)
                    {
                        switch (token.Value<string>())
                        {
                            case "SHORTNAME":
                                result.Add("SHORTNAME", i);
                                break;

                            case "SECID":
                                result.Add("SECID", i);
                                break;

                            case "BOARDID":
                                result.Add("BOARDID", i);
                                break;

                            case "TRADEDATE":
                                result.Add("TRADEDATE", i);
                                break;

                            case "CLOSE":
                                result.Add("CLOSE", i);
                                break;
                        }
                        i = i + 1;
            }
            return result;
        }
        private string CreateUrl(string market, string date) => $"https://iss.moex.com/iss/history/engines/stock/markets/{market}/securities.json?date={date}";
        private List<string> CreateDates(DateTime today)
        {
            List<string> result = new();
            int i = 1;
            while (result.Count < 100)
            {
                DateTime date = today.AddDays(-i);
                int j = 1;
                while (DayOff(date))
                {
                    j = j + 1;
                    date = date.AddDays(-1);
                }
                i = i + j;
                result.Add(date.ToString("yyy-MM-dd"));
            }
            return result;
        }
        private bool DayOff(DateTime date) => date.DayOfWeek == DayOfWeek.Saturday | date.DayOfWeek == DayOfWeek.Sunday;
    }
}
