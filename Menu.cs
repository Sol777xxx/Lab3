using Lab3.FactoryMethod;
using Lab3.Base;
using Lab3.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lab3.ObserverPattern;


namespace Lab3
{
    class Menu
    {
        private User user = new User { RAM = 16, CPU = 8, GPU = 6, HDD = 100 };

        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== ПК =====");
                Console.WriteLine("1. Статус комп'ютера");
                Console.WriteLine("2. Додати існуючі ігри");
                Console.WriteLine("3. Встановити існуючу гру ");
                Console.WriteLine("4. Запустити гру з переліку");
                Console.WriteLine("0. Вийти");
                Console.Write("Оберіть опцію: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": StatusPC(); break;
                    case "2": AddGames(); break;
                    case "3": DownloadGame(); break;
                    case "4": StartGameMenu(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Невідома команда, спробуйте ще раз.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        public void StatusPC()
        {
            user = Database.LoadUser();
            user.UpdateHDD();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Статус ПК =====");
                Console.WriteLine($"RAM: {user.RAM} GB");
                Console.WriteLine($"CPU: {user.CPU} cores");
                Console.WriteLine($"GPU: {user.GPU} GB");
                Console.WriteLine($"Всього пам'яті: 100 ГБ");
                Console.WriteLine($"Вільне місце: {user.HDD} ГБ");
                Console.WriteLine($"Windows встановлено: {(user.HasWindows ? "Так" : "Ні")}");
                Console.WriteLine($"Підключення до мережі: {(user.IsConnected ? "Так" : "Ні")}");
                Console.WriteLine($"Браузер встановлений: {(user.HasBrowser ? "Так" : "Ні")}");
                Console.WriteLine($"Кермо підключене: {(user.HasWheel ? "Так" : "Ні")}");
                Console.WriteLine("1. Інстолювати/видалити Windows");
                Console.WriteLine("2. Змінити підключення до мережі");
                Console.WriteLine("3. Встановити або видалити браузер");
                Console.WriteLine("4. Увімкнути або вимкнути кермо");
                Console.WriteLine("0. Повернутися до меню");
                Console.Write("Оберіть опцію: ");

                string choice = Console.ReadLine();
                bool changed = false;

                switch (choice)
                {
                    case "1":
                        user.ToggleWindows();
                        changed = true;
                        break;
                    case "2":
                        user.ToggleConnection();
                        changed = true;
                        break;
                    case "3":
                        user.ToggleBrowser();
                        changed = true;
                        break;
                    case "4":
                        user.ToggleWheel();
                        changed = true;
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невідома команда, спробуйте ще раз.");
                        continue;
                }

                if (changed)
                {
                    Database.SaveUser(user);
                    Console.WriteLine("Зміни збережено!");
                }
            }
        }




        public void AddGames()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Додати існуючі ігри =====");
                List<BaseGame> availableGames = Database.LoadGames();
                List<BaseGame> notInstalledGames = availableGames
                    .Where(g => !user.Games.Any(installed => installed.Name == g.Name)).ToList();
                Console.WriteLine("1. Додати нову гру");
                Console.WriteLine("0. Вийти");
                Console.Write("Оберіть опцію: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateNewGame(); break;
                    case "0": return;
                    default: Console.WriteLine("Невідома команда, спробуйте ще раз."); break;
                }
            }
        }

        public void CreateNewGame()
        {
            Console.Clear();
            Console.WriteLine("Оберіть жанр гри:");
            Console.WriteLine("1. Симулятор");
            Console.WriteLine("2. Стратегія");
            Console.WriteLine("3. Онлайн казино");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine();
            Console.Write("Введіть назву гри: ");
            string name = Console.ReadLine();

            GameCreator factory = null;

            switch (choice)
            {
                case "1":
                    factory = new SimulatorGameCreator();
                    break;
                case "2":
                    factory = new StrategyGameCreator();
                    break;
                case "3":
                    factory = new OnlineCasinoCreator();
                    break;
                default:
                    Console.WriteLine("Невірний вибір жанру.");
                    return;
            }

            int ram = 0, cpu = 0, gpu = 0, hdd = 0;

            if (choice != "3")
            {
                Console.Write("Скільки гра займає на HDD (в ГБ): ");
                int.TryParse(Console.ReadLine(), out hdd);

                Console.Write("Мінімальна RAM (в ГБ): ");
                int.TryParse(Console.ReadLine(), out ram);

                Console.Write("Мінімальний CPU (в ядрах): ");
                int.TryParse(Console.ReadLine(), out cpu);

                Console.Write("Мінімальна GPU (в ГБ): ");
                int.TryParse(Console.ReadLine(), out gpu);
            }

            BaseGame newGame = factory.FactoryMethod(name, ram, cpu, gpu, hdd);

            if (choice == "3" && !user.CanAddOnlineGame(newGame))
            {
                return;
            }

            var games = Database.LoadGames();
            games.Add(newGame);
            Database.SaveGames(games);

            Console.WriteLine($"Гра \"{newGame.Name}\" успішно додана в базу!");
        }


        public void DownloadGame()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Встановити гру =====");
                int currentSimulators = user.Games.Count(g => g is SimulatorGame);
                bool hasStrategy = user.Games.Any(g => g is StrategyGame);


                Console.WriteLine($"Наявні симулятори: {currentSimulators}/3");
                Console.WriteLine($"Стратегія: {(hasStrategy ? "Встановлена" : "Не встановлена")}");

                List<BaseGame> availableGames = Database.LoadGames();

                List<BaseGame> notInstalledGames = availableGames
                    .Where(g => !(g is OnlineCasino) && !user.Games.Any(installed => installed.Name == g.Name))
                    .ToList();
                Console.WriteLine("");

                Console.WriteLine("===== Доступні для встановлення ігри =====");

                var groupedGames = notInstalledGames.GroupBy(g => g.GetType().Name);
                foreach (var group in groupedGames)
                {
                    Console.WriteLine($"Жанр: {group.Key.Replace("Game", "")}"); 
                    foreach (var game in group)
                    {
                        Console.WriteLine($"- {game.Name} ({game.RequiredHDD} GB)");
                    }
                }

                if (notInstalledGames.Count == 0)
                {
                    Console.WriteLine("Усі ігри вже встановлені або немає доступних для встановлення.");
                    Console.WriteLine("0. Вийти");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("");
                for (int i = 0; i < notInstalledGames.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {notInstalledGames[i].Name} ({notInstalledGames[i].RequiredHDD} GB)");
                }

                Console.WriteLine("0. Вийти");
                Console.Write("Оберіть гру для встановлення: ");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > notInstalledGames.Count)
                {
                    return;
                }

                BaseGame selectedGame = notInstalledGames[choice - 1];
                if (user.HDD >= selectedGame.RequiredHDD)
                {
                   
                    if (!user.CanAddGame(selectedGame))
                    {
                        Console.WriteLine("Цю гру не можна встановити. Ви досягли обмеження за жанром.");
                        Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
                        Console.ReadKey();
                        return;
                    }
                    
                    selectedGame.Install(user); 
                    user.Games.Add(selectedGame);

                    Database.SaveUser(user); 
                    user.RestoreState();

                    Console.WriteLine($"Гра {selectedGame.Name} встановлена.");
                }
                else
                {
                    Console.WriteLine("Недостатньо місця на диску для встановлення гри.");
                    Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
                    Console.ReadKey();
                }
            }
        }

        public void StartGameMenu()
        {
            Console.Clear();
            Console.WriteLine("===== Запустити гру =====");
            List<BaseGame> availableGames = user.Games.ToList();

            List<BaseGame> allGames = Database.LoadGames();
            foreach (var game in allGames)
            {
                if (game is OnlineCasino && !availableGames.Any(g => g.Name == game.Name))
                {
                    availableGames.Add(game);
                }
            }

            if (availableGames.Count == 0)
            {
                Console.WriteLine("У вас немає встановлених ігор та доданих онлайн-ігор .");
                Console.WriteLine("0. Вийти");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < availableGames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableGames[i].Name}");
            }

            Console.WriteLine("0. Вийти");
            Console.Write("Оберіть гру для запуску: ");

            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > availableGames.Count)
            {
                return;
            }

            BaseGame selectedGame = availableGames[choice - 1];
            if (user.RAM < selectedGame.RequiredRAM || user.CPU < selectedGame.RequiredCPU || user.GPU < selectedGame.RequiredGPU)
            {
                Console.WriteLine($"Гра \"{selectedGame.Name}\" вимагає більше ресурсів, ніж має ваш ПК.");
                Console.WriteLine($"Ваш ПК: RAM {user.RAM}GB, CPU {user.CPU} ядер, GPU {user.GPU}GB.");
                Console.WriteLine($"Гра вимагає: RAM {selectedGame.RequiredRAM}GB, CPU {selectedGame.RequiredCPU} ядер, GPU {selectedGame.RequiredGPU}GB.");
                Console.WriteLine("Натисніть будь-яку клавішу, щоб повернутися в меню...");
                Console.ReadKey();
                return;
            }
            if (!selectedGame.CanStartGame(user))
            {
                Console.ReadLine();
                return;
            }


            Console.Write("Отримувати докладні сповіщення про процес гри? (так/ні): ");
            string answer = Console.ReadLine().Trim().ToLower();

            GameObserver observer = new GameObserver();

            if (answer == "так")
            {
                selectedGame.SubscribeObserver(observer);
                Console.WriteLine("🔔 Ви підписалися на докладні сповіщення.");
            }
            else
            {
                Console.WriteLine("🔕 Докладні сповіщення не були увімкнені.");
            }

            selectedGame.StartGame(user);
        }


    }
}
