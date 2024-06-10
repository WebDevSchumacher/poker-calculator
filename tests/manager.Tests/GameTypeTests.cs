using System;
using contracts;
using Xunit;
using manager;

namespace manager.Tests
{
    public class GameTypeTests
    {
        [Fact]
        public async void InsertGameTypeTest()
        {
            GameType type = new GameType("holdem", 2, 5);
            string id = await manager.Controllers.DBController.AddGameType(type);
            Console.WriteLine(id + "qew");
            Assert.NotNull(id);
        }
    }
}
