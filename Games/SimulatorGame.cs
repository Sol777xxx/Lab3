using Lab3.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Games
{
    public class SimulatorGame : BaseGame
    {
        public SimulatorGame(string name, int ram, int cpu, int gpu, int hdd)
            : base(name, ram, cpu, gpu, hdd)
        {
            GameTick += OnGameTick;
        }

        public override bool CanStartGame(User user)
        {
            if (!IsInstalled)
            {
                Console.WriteLine($"Гра-симулятор {Name} не встановлена.");
                return false;
            }

            if (!user.HasWindows)
            {
                Console.WriteLine($"Гра-симулятор {Name} потребує Windows.");
                return false;
            }

            if (user.HasWheel)
            {
                Console.Write("Кермо підключене. Використати його для гри? (так/ні): ");
                string answer = Console.ReadLine().Trim().ToLower();
                if (answer == "так")
                {
                    Console.WriteLine("Якість гри покращена завдяки керму!");
                }
            }

            return user.RAM >= RequiredRAM && user.CPU >= RequiredCPU && user.GPU >= RequiredGPU;
        }

        private int level = 0;
        public override void LoadProgress()
        {
            var saves = Database.LoadSaves();
            var save = saves.FirstOrDefault(s => s.GameName == Name);

            if (save != null)
            {
                Console.WriteLine($"Завантажено прогрес для гри-симулятора {Name}: {save.SaveData}");
                int.TryParse(save.SaveData.Replace("Рівень: ", ""), out level);
            }
        }

        private void OnGameTick(string name)
        {
            level++;
            Console.WriteLine($"[{name}] Рівень: {level}");
        }

        public override void SaveGame()
        {
            var saves = Database.LoadSaves();
            saves.RemoveAll(s => s.GameName == Name);

            saves.Add(new GameSave { GameName = Name, Genre = $"Симулятор", SaveData = $"Рівень: {level}" });
            Database.SaveSaves(saves);
            Console.WriteLine($"Прогрес гри-симулятора {Name} збережено.");
            Console.WriteLine($"Ваш збережений рівень: {level}");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
            Console.ReadKey();
        }
    }
}
