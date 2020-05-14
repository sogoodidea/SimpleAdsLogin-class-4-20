using class_4_20.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace class_4_20.Models
{
    public class IndexViewModel
    {
        public bool IsLoggedIn { get; set; }
        public List<SimpleAd> Ads { get; set; }
    }
}
