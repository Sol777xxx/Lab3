﻿using Lab3.Games;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Lab3.Base
{
    public abstract class BaseGame
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



        public void StartGame(User user)
        {
            LoadProgress();
            IsRunning = true;
            Console.WriteLine($"{Name} запущена!");

            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    await Task.Delay(1000);
                    if (!IsRunning) break; 
                    GameTick?.Invoke(Name);
                }
            });

            Console.WriteLine("Натисніть будь-яку клавішу, щоб завершити гру...");
            Console.ReadKey();
            StopGame();
        }

        public void StopGame()
        {
            if (!IsRunning) return;

            IsRunning = false; 
            Console.WriteLine($"{Name} завершена.");
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
