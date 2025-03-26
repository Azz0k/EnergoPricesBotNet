using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace PricesBotWorkerService
{
    public class DBRepository : DbContext
    {
        private AppSettings _settings;
        private ILogger<WindowsBackgroundService> _logger;
        public DbSet<Users>? Users { get; set; }
        public DBRepository(IOptions<AppSettings> settings, ILogger<WindowsBackgroundService> logger) 
        {
            _settings = settings.Value;
            _logger = logger;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(_settings.SQLConnection);
           // optionsBuilder.LogTo(Console.WriteLine);
        }
 
    }
}
[Keyless]
public class Users
{
    [Column("id")]
    public long? ID { get; set; }
    [Required, StringLength(50), Column("phone_number")]
    public required string PhoneNumber { get; set; }
}