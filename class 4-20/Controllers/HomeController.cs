using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using class_4_20.Models;
using class_4_20.data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace class_4_20.Controllers
{
    public class HomeController : Controller
    {
        private SimpleAdMgr _mgr = new SimpleAdMgr();
        public IActionResult Index()
        {
            if (TempData["Success"] != null)
            {
                ViewBag.Message = TempData["Success"];
            }
            bool isLoggedIn = User.Identity.IsAuthenticated;
            var user = new User();
            if (isLoggedIn)
            {
                user = _mgr.GetUserByEmail(User.Identity.Name);
            }
            var vm = new IndexViewModel
            {
                Ads = _mgr.GetSimpleAds().Select(ad => new SimpleAd
                {
                    Name = ad.Name,
                    PhoneNumber = ad.PhoneNumber,
                    Text = ad.Text,
                    DateCreated = ad.DateCreated,
                    Id = ad.Id,
                    IsUsersAd = isLoggedIn ? ad.UserId == user.Id : false
                }).ToList()
            };
            return View(vm);
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            _mgr.AddUser(user, password);
            TempData["Success"] = "Welcome! We hope you'll enjoy all the perks of having an account!";
            return Redirect("/");
        }
        public IActionResult Login()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Message = TempData["Error"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _mgr.Login(email, password);
            if (user == null)
            {
                TempData["Error"] = "Invalid Login";
                return Redirect("/home/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/login");
        }

        [Authorize]
        public IActionResult NewAd()
        {
            var user = _mgr.GetUserByEmail(User.Identity.Name);
            return View(user);
        }

        [HttpPost]
        public IActionResult NewAd(SimpleAd ad)
        {
            _mgr.AddPost(ad);
            TempData["Success"] = "New ad posted successfully!";
            return Redirect("/home/index");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var user = _mgr.GetUserByEmail(User.Identity.Name);
            user.Ads = _mgr.GetAdsForUserId(user.Id);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int adId)
        {
            var user = _mgr.GetUserByEmail(User.Identity.Name);
            var ads = _mgr.GetAdsForUserId(user.Id);
            var ids = ads.Select(ad => ad.Id);
            if (ids != null && ids.Contains(adId))
            {
                _mgr.DeleteAd(adId);
                TempData["Success"] = "Ad was deleted";
            }
            return Redirect("/");
        }
    }
}
