using Lab3.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Games
{
    public class StrategyGame : BaseGame
    {
        public StrategyGame(string name, int ram, int cpu, int gpu, int hdd)
            : base(name, ram, cpu, gpu, hdd)
        {
            GameTick += OnGameTick;
        }
        public override bool CanStartGame(User user)
        {
            if (!IsInstalled)
            {
                Console.WriteLine($"Стратегічна гра {Name} не встановлена.");
                return false;
            }

            if (!user.HasWindows)
            {
                Console.WriteLine($"Стратегічна гр {Name} потребує Windows.");
                return false;
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
                Console.WriteLine($"Завантажено прогрес для гри-стратегії {Name}: {save.SaveData}");
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

            saves.Add(new GameSave { GameName = Name, Genre = $"Стратегія", SaveData = $"Рівень: {level}" });
            Database.SaveSaves(saves);

            Console.WriteLine($"Прогрес гри-стратегії {Name} збережено.");
            Console.WriteLine($"Ваш збережений рівень: {level}");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
            Console.ReadKey();
        }
    }
}
