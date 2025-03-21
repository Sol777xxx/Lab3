using Lab3.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Games
{
    public class OnlineCasino : BaseGame//ConcreteClass1,ConcreteProduct1
    {
        public OnlineCasino(string name) : base(name, 0, 0, 0, 0)
        {
            GameTick += OnGameTick;
        }

        public override bool CanStartGame(User user)
        {
            if (!user.IsConnected)
            {
                Console.WriteLine("Потрібне підключення до мережі для запуску Online Casino.");
                return false;
            }
            if (!user.HasBrowser)
            {
                Console.WriteLine("Потрібне підключення браузера для запуску Online Casino.");
                return false;
            }
            return true;
        }
        private int balance = 100;
        public override void LoadProgress()
        {
            var saves = Database.LoadSaves();
            var save = saves.FirstOrDefault(s => s.GameName == Name);

            if (save != null)
            {
                Console.WriteLine($"Завантажено прогрес для онлайн-гри {Name}: {save.SaveData}");
                int.TryParse(save.SaveData.Replace("Баланс: ", ""), out balance);
            }
        }


        private Random random = new Random();

        private void OnGameTick(string gameName)
        {
            int result = random.Next(-10, 20);
            balance += result;
            Console.WriteLine($"[{gameName}] Ставка: {result}$, Баланс: {balance}$");
        }



        public override void SaveGame()
        {
            var saves = Database.LoadSaves();
            saves.RemoveAll(s => s.GameName == Name);

            saves.Add(new GameSave { GameName = Name, Genre = $"Казино", SaveData = $"Баланс: {balance}" });
            Database.SaveSaves(saves);

            Console.WriteLine($"Прогрес  онлайн-гри {Name} збережено.");
            Console.WriteLine($"Ваш збережений баланс: {balance}");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
            Console.ReadKey();
        }

    }
}

