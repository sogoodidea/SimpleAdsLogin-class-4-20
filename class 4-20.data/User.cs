using System;
using System.Collections.Generic;
using System.Text;

namespace class_4_20.data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public List<SimpleAd> Ads = new List<SimpleAd>();
    }
}
