using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Lab3.Base;
using System.Linq;
using Newtonsoft.Json.Serialization;

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
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = new CustomBinder()
            };

            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження ігор: {ex.Message}");
                return new List<BaseGame>();
            }
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
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    SerializationBinder = new CustomBinder()
                };

                return JsonConvert.DeserializeObject<User>(json, settings) ?? new User(false);
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
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                SerializationBinder = new CustomBinder()
            };

            try
            {
                return JsonConvert.DeserializeObject<List<GameSave>>(json, settings) ?? new List<GameSave>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження збережень: {ex.Message}");
                return new List<GameSave>();
            }
        }
    }

    public class CustomBinder : DefaultSerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("Lab2.Base"))
            {
                typeName = typeName.Replace("Lab2.Base", "Lab3.Base");
            }

            return base.BindToType(assemblyName, typeName);
        }
    }
}
