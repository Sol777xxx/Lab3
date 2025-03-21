using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Lab3.Base;
using System.Security.Cryptography;
using System.Linq;

namespace Lab3
{
    public class Database
    {
        private const string GamesFile = "games.json";
        private const string SavesFile = "saves.json";

        public static void SaveGames(List<BaseGame> games)
        {
           
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };
            File.WriteAllText(GamesFile, JsonConvert.SerializeObject(games, settings));
        }

        public static List<BaseGame> LoadGames()
        {
            if (!File.Exists(GamesFile)) return new List<BaseGame>();

            string json = File.ReadAllText(GamesFile);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            List<BaseGame> games = JsonConvert.DeserializeObject<List<BaseGame>>(json, settings) ?? new List<BaseGame>();

            foreach (var game in games)
            {
                if (game.IsInstalled)
                {
                    game.RestoreInstalled();
                }
            }

            return games;
        }


        public static void SaveUser(User user)
        {
            user.UpdateHDD();
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };
            File.WriteAllText("user.json", JsonConvert.SerializeObject(user, settings));
        }

        public static User LoadUser()
        {
            if (!File.Exists("user.json"))
            {
                return new User(false);
            }

            try
            {
                string json = File.ReadAllText("user.json");
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };
                User loadedUser = JsonConvert.DeserializeObject<User>(json, settings) ?? new User(false);

                return loadedUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження користувача: {ex.Message}");
                return new User(false);
            }
        }

        public static void SaveSaves(List<GameSave> saves)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };
            File.WriteAllText(SavesFile, JsonConvert.SerializeObject(saves, settings));
        }


        public static List<GameSave> LoadSaves()
        {
            if (!File.Exists(SavesFile)) return new List<GameSave>();
            string json = File.ReadAllText(SavesFile);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return JsonConvert.DeserializeObject<List<GameSave>>(json, settings) ?? new List<GameSave>();
        }
    }

}
