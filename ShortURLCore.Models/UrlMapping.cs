using ShortURLCore.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortURLCore.Models
{
    public class UrlMapping : BaseEntity
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = "";
        public string ShortCode { get; set; } = "";
    }
}
