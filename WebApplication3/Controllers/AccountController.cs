using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;
using WebApplication3.Data;
using WebApplication3.Data.Repository;
using WebApplication3.Data.Static;
using WebApplication3.Data.ViewModels;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataContext _context;
        private readonly TwilioService _twilio;
        private readonly IAccountRepository _accountRepository;
      
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataContext context, TwilioService twilio, IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _twilio = twilio;
            _accountRepository = accountRepository;
           
        }

        public IActionResult Login() => View(new LoginVM());

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginvm)
        {
            if (!ModelState.IsValid) return View(loginvm);
            var user = await _userManager.FindByEmailAsync(loginvm.emailaddress);
            if (user != null)
            {
                var passwordcheck = await _userManager.CheckPasswordAsync(user, loginvm.Password);
                if (passwordcheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginvm.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Product");
                    }
                    if(result.IsNotAllowed)
                    {
                        ModelState.AddModelError("", "Not allowed to login");
                    }

                }
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginvm);
            }
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginvm);


        }
        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registervm)

        {
            if (!ModelState.IsValid) return View(registervm);
            var user = await _userManager.FindByEmailAsync(registervm.emailaddress);
            {
                if (user != null)
                {
                    TempData["Error"] = "This email address is already in use.";
                    return View(registervm);
                }

                if (registervm.companyname != null)
                {
                    var newUser = new ApplicationUser()
                    {
                        Email = registervm.emailaddress,
                        FullName = registervm.fullname,
                        Companyname = registervm.companyname,
                        PhoneNumber = registervm.phoneNumber,
                        UserName = registervm.emailaddress,
                        datacenterlocation = registervm.SelectedLocation.ToString(),
                        

                    };
                    var newuserresponse = await _userManager.CreateAsync(newUser, registervm.Password);
                    if (newuserresponse.Succeeded)
                    {
                        await _accountRepository.GenerateEmailConfirmationTokenAsync(newUser);
                    };
                }
                else
                {
                    var newUser = new ApplicationUser()
                    {
                        Email = registervm.emailaddress,
                        FullName = registervm.fullname,
                        UserName = registervm.emailaddress

                    };
                    var newuserresponse = await _userManager.CreateAsync(newUser, registervm.Password);
                    if (newuserresponse.Succeeded)
                    {
                        await _accountRepository.GenerateEmailConfirmationTokenAsync(newUser);
                        await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    };
                }

                return RedirectToAction("ConfirmEmail", new { email =  registervm.emailaddress });


            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail (string uid, string token, string email)
        {
            EmailConfirmModel model = new EmailConfirmModel
            {
                Email = email
            };
             if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
            var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
               
            }
            return View(model);

        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if(user != null)
            {
                if(user.EmailConfirmed)
                {
                    model.isConfirmed = true;
                    return View(model);

                }
                await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent = true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            return View(model);

        }

       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]

        public async Task<JsonResult> PhoneNumberVerificationHandler (string PhoneNumber)
        {
            // Send Verification Token 
          var  service = _twilio.SendVerificationToken(PhoneNumber);
            return Json(new { success = true, service });

        }

        [HttpGet]
        public async Task<JsonResult> PhoneNumberVerificationCheckHandler(string PhoneNumber, string code, string serviceSid)
        {
            // Send Verification Token 
            var status = _twilio.CheckVerificationToken(PhoneNumber, code, serviceSid);
            if (status == "approved")
            {
                return Json(new { success = true, status });
            }
            else
            {
                return Json(new { success = false, status });
            }
          

        }


        public IActionResult Profile()
        {
            return View();
        }

        [AllowAnonymous, HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel resetpasswordmodel = new ResetPasswordModel
            {
                Token = token,
                UserId = uid
            };
            return View(resetpasswordmodel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {

                model.Token = model.Token.Replace(' ', '+');
                var result = await _accountRepository.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(model.Email);
                if(user != null)
                {
                   await _accountRepository.GenerateForgotPasswordTokenAsync(user);
                }

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [Route("change-password")]

        public  IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError("", errors.Description);
                }
            }
            return View(model);
        }
    }
}
