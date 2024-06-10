using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using frontend.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Dynamic;

namespace frontend.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<GameController> _logger;
        private readonly HttpClient _client;
        private List<String> _allValues = new List<String> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        private readonly List<CardType> _allTypes = new List<CardType> { CardType.Heart, CardType.Diamond, CardType.Spade, CardType.Club };
        private readonly List<string> _allTypesString = new List<string> { "Heart", "Diamond", "Spade", "Club" };
        private readonly List<int> opponentNumb = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public List<List<Card>> cardOrderedTypes;
        public List<GameType> gameTypeList;
        private dynamic mymodel = new ExpandoObject();

        public GameController(ILogger<GameController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public IActionResult Index()
        {
            gameTypeList = new List<GameType> { new GameType("Texas Holdem", 2, 5), new GameType("Variant Poker", 2, 6) };
            ResetCards();
            mymodel.Cards = cardOrderedTypes;
            mymodel.Opponent = opponentNumb;
            mymodel.GameTypeList = gameTypeList;
            mymodel.InitialIndex = 0;
            return View(mymodel);
        }

        [HttpGet]
        public IActionResult OnGetPartial(string gameTypeIndex)
        {
            ResetCards();
            int index = Convert.ToInt32(gameTypeIndex);
            gameTypeList = new List<GameType> { new GameType("Texas Holdem", 2, 5), new GameType("Variant Poker", 2, 6) };
            mymodel.Cards = cardOrderedTypes;
            mymodel.Opponent = opponentNumb;
            mymodel.InitialIndex = index;
            mymodel.currentIndex = index;
            mymodel.GameTypeList = gameTypeList;
            return PartialView("_Index", mymodel);

        }

        public IActionResult Result(string playround, string testValue)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Result = testValue;
            mymodel.PlayroundDeserialized = JsonConvert.DeserializeObject<Playround>(playround);
            mymodel.PlayroundSerialized = JsonConvert.SerializeObject(mymodel.PlayroundDeserialized);
            return View(mymodel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void ResetCards()
        {
            //deck = new List<Card>();
            cardOrderedTypes = new List<List<Card>>();
            foreach (CardType type in _allTypes)
            {
                List<Card> cardTypeList = new List<Card>();
                foreach (String value in _allValues)
                {
                    cardTypeList.Add(new Card(type, value));
                    //deck.Add(card);
                }
                cardOrderedTypes.Add(cardTypeList);
            }
        }

        //check if JSON is manipulated
        public bool validatePlayround(Playround playround)
        {
            if (playround != null)
            {
                //check invalid length 
                // var boardcound = playround.boardcards.Count;
                // if (boardcound < 3 || boardcound > 5 || playround.handcards.Count != 2)
                // {
                //     return false;
                // }
                //check invalid values 
                var cards = new List<CardCardTypeString>();
                cards.AddRange(playround.boardcards);
                cards.AddRange(playround.handcards);
                var cardsAsString = new List<string>();
                foreach (CardCardTypeString card in cards)
                {
                    if (!(_allTypesString.Contains(card.CardType, StringComparer.OrdinalIgnoreCase) && _allValues.Contains(card.CardValue, StringComparer.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                    cardsAsString.Add(card.CardType + card.CardValue);
                }
                //check dupilicate values
                if (cardsAsString.Count != cardsAsString.Distinct().Count())
                {
                    return false;
                }
                //check invalid values 
                try
                {
                    int output = int.Parse(playround.opponent);
                    return output < 10 && output > 0;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        [HttpPost]
        public async Task<IActionResult> GetResult(string playround)
        {
            // var playround = Request.Form["playround"];
            // return RedirectToAction("Result", new { playround = playround, testValu = "10%" });
            try
            {
                JObject playroundJSON = JObject.Parse(playround);
            }
            catch
            {
                return RedirectToAction("Error");
            }
            //check if JSON is correct
            if (!validatePlayround(JsonConvert.DeserializeObject<Playround>(playround)))
            {
                return RedirectToAction("Error");
            }
            try
            {
                string url = "http://manager/Calculate";
                var content = new StringContent(playround, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, content);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    return RedirectToAction("Result", new { playround = playround, testValu = "10%" });
                }
            }
            catch
            {
                return StatusCode(500);
            }
            return RedirectToAction("Error");
        }

    }
}
