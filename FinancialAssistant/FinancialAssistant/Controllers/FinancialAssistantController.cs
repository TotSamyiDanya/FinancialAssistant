using FinancialAssistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FinancialAssistant.Services;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace FinancialAssistant.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FinancialAssistantController : ControllerBase
    {
        [HttpGet]
        [Route("Markets")]
        public IActionResult GetMarkets()
        {
            string path = @"Json\markets.json";
            string? json = System.IO.File.Exists(path) ?
                System.IO.File.ReadAllText(path) : null;
            if (!string.IsNullOrEmpty(json))
                return Ok(json);
            return NotFound();
        }
        [HttpGet]
        [Route("Boards")]
        public IActionResult GetBoards(string market)
        {
            string path = @$"Json\{market}_boards.json";
            string? json = System.IO.File.Exists(path) ?
                System.IO.File.ReadAllText(path) : null;
            if (!string.IsNullOrEmpty(json))
                return Ok(json);
            return NotFound();
        }
        [HttpGet]
        [Route("Load")]
        public void Load()
        {
            Response.WriteAsync("Helloy Broy :)");
            //Services.MoexService.getInstance().LoadMarkets();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().LoadAllBoards();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().LoadSamples();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().Deserialize("shares");
            //Services.MoexService.getInstance().Deserialize("");
        }
        [HttpGet]
        [Route("Get")]
        public IActionResult Get(string market, string board)
        {
            using FinancialAssistantContext db = new();
            var result = db.Tools.Where(t => t.Market.MarketName.Equals(market) &&
                t.Board.BoardName.Substring(0, 4).Equals(board))
                .Select(t => new { t.ToolName, t.Board.BoardName }).ToList();

            string? obj = Extensions.IsNull(result) ? null : JsonConvert.SerializeObject(result);
            if (!string.IsNullOrEmpty(obj))
                return Ok(obj);
            return NotFound();
        }
        [HttpGet]
        [Route("Tool")]
        public IActionResult Tool(string toolName, string board)
        {
            using FinancialAssistantContext db = new();
            var result = db.Tools.Where(t => t.Board.BoardName.Substring(0, 4).Equals(board) &&
                t.ToolName.Equals(toolName)).Select(t => new {t.ToolName, t.Market.MarketName, t.Board.BoardName, t.TradeDates}).FirstOrDefault();

            string? obj = Extensions.IsNull(result) ? null : JsonConvert.SerializeObject(result);
            if (!string.IsNullOrEmpty(obj))
                return Ok(obj);
            return NotFound();
        }
    }
}
