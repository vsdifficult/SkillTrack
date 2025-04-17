using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace SkillTrack 
{ 
    public class Program 
    { 
        public static void Main(string[] args) 
        { 
            try 
            { 
                CreateHostBuilder(args).Build().Run(); 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine($"Excp: {ex.Message}"); 
                throw; 
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }); 
    }
}