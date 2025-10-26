using ShortURLCore.Models.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Models
{
    public class AppConfig : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string DbConnectionString { get; set; } = "";
        public bool IsInstalled { get; set; } = false;
    }
}
