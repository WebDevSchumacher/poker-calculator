
using System.Collections.Generic;

namespace frontend.Models
{
    public class Playround
    {
        public List<CardCardTypeString> handcards { get; set; }
        public List<CardCardTypeString> boardcards { get; set; }
        public string opponent { get; set; }
    }

}
