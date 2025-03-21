using Lab3.Games;
using Lab3.ObserverPattern;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Lab3.Base
{
    public abstract class BaseGame//AbstractClass Template Method (шаблонний),IProduct хоч і не інтерфейс 
    {
        public string Name { get; protected set; }
        [JsonProperty]
        public int RequiredRAM { get; protected set; }
        [JsonProperty]
        public int RequiredCPU { get; protected set; }
        [JsonProperty]
        public int RequiredGPU { get; protected set; }
        [JsonProperty]
        public int RequiredHDD { get; set; } 

        [JsonProperty]
        public bool IsInstalled { get; private set; }
        [JsonProperty]
        public bool IsRunning { get; private set; }


        public event Action<string> GameTick;

        public abstract void SaveGame();
        public abstract bool CanStartGame(User user);
        public abstract void LoadProgress();


        protected BaseGame(string name, int ram, int cpu, int gpu, int hdd)
        {
            Name = name;
            RequiredRAM = ram;
            RequiredCPU = cpu;
            RequiredGPU = gpu;
            RequiredHDD = hdd;
            IsInstalled = false;
            IsRunning = false;
        }

        public void Install(User user)
        {
            if (!IsInstalled && user.HDD >= RequiredHDD)
            {
                IsInstalled = true;
                user.HDD -= RequiredHDD;

              
                List<BaseGame> allGames = Database.LoadGames();
                foreach (var game in allGames)
                {
                    if (game.Name == this.Name)
                    {
                        game.IsInstalled = true;
                    }
                }
                Database.SaveGames(allGames);

             
                Database.SaveUser(user);

                Console.WriteLine($"{Name} успішно встановлена.");
            }
            else
            {
                Console.WriteLine($"Гра {Name} вже встановлена або недостатньо місця.");
            }
        }


        private GameMonitor gameMonitor = new GameMonitor();

        public void SubscribeObserver(GameObserver observer)
        {
            observer.Subscribe(gameMonitor);
        }

        protected void Notify(string message)
        {
            gameMonitor.Notify(new GameEvent(Name, message));
        }


        public void StartGame(User user)// задає загальний алгоритм запуску гри
        {
            LoadProgress();
            IsRunning = true;
            Console.WriteLine($"{Name} запущена!");
            Notify("Гра запущена!");
            Console.WriteLine("Натисніть ESC, щоб завершити гру...");
            while (IsRunning)
            {
                System.Threading.Thread.Sleep(1000);
                GameTick?.Invoke(Name);
                Notify("Ігровий процес триває...");
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Гру завершено за запитом гравця.");
                        StopGame();
                        break;
                    }
                }
            }
        }

        public void StopGame()
        {
            if (!IsRunning) return;
            IsRunning = false;
            Console.WriteLine($"{Name} завершена.");
            Notify("Гра завершена.");
            gameMonitor.Complete();// завершуємо та видаляємо підписників
            SaveGame();

            Menu menu = new Menu();
            menu.StartGameMenu();
        }


        public void RestoreInstalled()
        {
            IsInstalled = true;
        }

    }
}
