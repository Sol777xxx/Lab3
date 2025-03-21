using Lab3.Base;
using Lab3.Games;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab3
{

    public class User
    {
        public string Name { get; set; }
        [JsonProperty]
        public int RAM { get; set; }
        [JsonProperty]
        public int CPU { get; set; }
        [JsonProperty]
        public int GPU { get; set; }
        [JsonProperty]
        public int HDD { get; set; }
        [JsonProperty]
        public bool HasBrowser { get; private set; }
        [JsonProperty]
        public bool IsConnected { get; private set; }
        [JsonProperty]
        public bool HasWheel { get; private set; }
        [JsonProperty]
        public bool HasWindows { get; protected set; }

        public List<BaseGame> Games { get; set; } = new List<BaseGame>();

        public User(bool loadFromFile = true)
        {
            if (loadFromFile && File.Exists("user.json"))
            {
                User loadedUser = Database.LoadUser();

                RAM = loadedUser.RAM;
                CPU = loadedUser.CPU;
                GPU = loadedUser.GPU;
                HDD = 100; 

                HasWindows = loadedUser.HasWindows;
                HasBrowser = loadedUser.HasBrowser;
                IsConnected = loadedUser.IsConnected;
                HasWheel = loadedUser.HasWheel;
                Games = loadedUser.Games ?? new List<BaseGame>();

                foreach (var game in Games)
                {
                    if (game.IsInstalled)
                    {
                        HDD -= game.RequiredHDD;
                    }
                }

                Console.WriteLine($"Користувач успішно завантажений. Вільно на диску: {HDD} GB");
            }
            else
            {

                RAM = 16;
                CPU = 8;
                GPU = 6;
                HDD = 100;
                HasWindows = false;
                HasBrowser = false;
                IsConnected = false;
                HasWheel = false;
                Games = new List<BaseGame>();
            }
        }

        public void RestoreState()
        {
            Console.WriteLine("Відновлення стану користувача...");


            foreach (var game in Games)
            {
                if (game.IsInstalled)
                {
                    game.RestoreInstalled();
                }
            }

            UpdateHDD();
        }
        public void UpdateHDD()
        {
            int usedSpace = Games.Where(g => g.IsInstalled).Sum(g => g.RequiredHDD);
            HDD = 100 - usedSpace;
            Console.WriteLine($"Оновлено HDD: {HDD} GB");
        }

        public void InstallGame(BaseGame game)
        {
            if (HDD >= game.RequiredHDD)
            {
                game.Install(this);
                Games.Add(game);
                UpdateHDD();
            }
        }

        public void SaveUserData()
        {
            Database.SaveUser(this);
        }

        public static User LoadUserData()
        {
            return Database.LoadUser();
        }

        public void ToggleConnection()
        {
            IsConnected = !IsConnected;
            SaveUserData();
        }

        public void ToggleBrowser()
        {
            HasBrowser = !HasBrowser;
            SaveUserData();
        }

        public void ToggleWheel()
        {
            HasWheel = !HasWheel;
            SaveUserData();
        }
        public void ToggleWindows()
        {
            HasWindows = !HasWindows;
            Console.WriteLine($"Windows {(HasWindows ? "встановлено" : "видалено")}!");
            SaveUserData();
        }



        public bool CanAddGame(BaseGame newGame)
        {
            if (newGame is StrategyGame && Games.Count(g => g is StrategyGame) >=1 )
            {
                Console.WriteLine("У вас вже є стратегічна гра.");
                return false;
            }

            if (newGame is SimulatorGame && Games.Count(g => g is SimulatorGame) >= 3)
            {
                Console.WriteLine("Максимальна кількість симуляторів – 3.");
                return false;
            }
            return true;
        }
        public bool CanAddOnlineGame(BaseGame newGame)
        {
            var existingGames = Database.LoadGames();
            if (existingGames.Any(g => g is OnlineCasino))
            {
                Console.WriteLine("У вас вже є онлайн-казино. Додавати ще одне не можна.");
                Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
                Console.ReadKey();
                return false;
            }
            return true;
        }

    }
}
