using FinancialAssistant.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            string json = System.IO.File.ReadAllText(@"Json\markets.json");

            if (!String.IsNullOrEmpty(json))
            {
                return Ok(json);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("Boards")]
        public IActionResult GetBoards(string market)
        {
            string json = System.IO.File.ReadAllText(@$"Json\{market}_boards.json");

            if (!String.IsNullOrEmpty(json))
            {
                return Ok(json);
            }
            return NotFound();
        }
        [HttpGet]
        [Route("Load")]
        public void Load()
        {
            //Response.WriteAsync("Helloy Broy :)");
            //Services.MoexService.getInstance().LoadMarkets();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().LoadAllBoards();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().LoadSamples();
            //Thread.Sleep(5000);
            //Services.MoexService.getInstance().Deserialize("shares");
            //Thread.Sleep(5000);
        }
        [HttpGet]
        [Route("Get")]
        public IActionResult Get(string market, string board)
        {
            using FinancialAssistantContext db = new();
            var result = db.Tools.Where(t => t.Board.BoardName.Substring(0, 4) == board && t.Market.MarketName == market).ToList();
            string obj = JsonConvert.SerializeObject(result);
            if (!String.IsNullOrEmpty(obj))
            {
                db.Dispose();
                return Ok(obj);
            }
            else
            {
                db.Dispose();
                return NotFound();
            }
        }
        [HttpGet]
        [Route("Tool")]
        public IActionResult Tool(string toolName, string board)
        {
            using FinancialAssistantContext db = new();
            var result = db.Tools.Where(t => t.Board.BoardName.Substring(0, 4) == board && t.ToolName == toolName).FirstOrDefault();
            string obj = JsonConvert.SerializeObject(result);
            if (!String.IsNullOrEmpty(obj))
            {
                db.Dispose();
                return Ok(obj);
            }
            else
            {
                db.Dispose();
                return NotFound();
            }
        }
    }
}
