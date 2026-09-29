using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using ChatApplication.DBContext;
using Chat.ViewModels;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Security.Claims;
using System;
using Chat.Services;
using System.ComponentModel.DataAnnotations;

namespace Chat.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ChatContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly EmailService _emailService;

        public AccountController(ChatContext context, IWebHostEnvironment webHost, EmailService emailService)
        {
            _context = context;
            _webHost = webHost;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Email or Password.");
                return View(model);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("UserID", user.UserId.ToString()),
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("Dashboard", "Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            //return RedirectToAction("Login");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            //if (!await IsCaptchaValid(Request.Form["g-recaptcha-response"]))
            //{
            //    ModelState.AddModelError("", "Captcha validation failed. Please try again.");
            //    return View(model);
            //}

            if (!ModelState.IsValid)
                return View(model);

            var email = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (email == null)
            {
                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                var originalFileName = Path.GetFileNameWithoutExtension(model.Image.FileName);
                var extension = Path.GetExtension(model.Image.FileName);
                var uniqueFileName = $"{originalFileName}_{timeStamp}{extension}";

                var fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    ImageName = uniqueFileName
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var adminEmail = model.Email;
                string subject = "Welcome";
                string body = $@"<!DOCTYPE html>
                    <html>
                    <head>
                    </head>
                    <body>
                        <div>Welcome <strong>{model.Username}</strong> to our website.</div>
                    </body>
                    </html>";

                //await _emailService.SendEmailAsync(adminEmail, subject, body);

                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "This Email has already account.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user != null)
            {
                var token = Guid.NewGuid().ToString();
                user.ResetToken = token;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
                _context.Update(user);
                await _context.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

                string subject = "Reset Your Password";
                string body = $@"<!DOCTYPE html>
                    <html>
                    <body>
                        <p>Click the link below to reset your password:</p>
                        <a href=""{resetLink}"">Reset Password</a>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(model.Email, subject, body);
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Account with this email does not exist.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest();

            return View(new ResetPasswordViewModel { Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword == model.ConfirmPassword)
            {
                var user = _context.Users.FirstOrDefault(u => u.ResetToken == model.Token && u.ResetTokenExpiry > DateTime.UtcNow);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid or expired token.");
                    return View();
                }

                user.Password = (model.NewPassword);
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "New Password and Confirm Password are not same.");
            return View(model);
        }

        private async Task<bool> IsCaptchaValid(string token)
        {
            var secretKey = "6LfNNHYrAAAAABehNy194q2wzcGTJD-UTwK04QD4"; // Replace with your secret key
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                null
            );
            var jsonString = await response.Content.ReadAsStringAsync();
            var captchaResponse = JsonSerializer.Deserialize<GoogleCaptchaResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return captchaResponse != null && captchaResponse.Success;
        }
        public class GoogleCaptchaResponse
        {
            public bool Success { get; set; }
            public List<string> ErrorCodes { get; set; }
        }



        public IActionResult AccessDenied() => View();
    }
}


