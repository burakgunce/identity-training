using IdentityApplicationSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;

namespace IdentityApplicationSample.Controllers
{
	public class AdminController : Controller
	{

		private UserManager<AppUser> userManager;
		private IPasswordHasher<AppUser> passwordHasher;


		public AdminController(UserManager<AppUser> _userManager, IPasswordHasher<AppUser> _passwordHasher)


		{
			this.userManager = _userManager;
			this.passwordHasher = _passwordHasher;
		}




		public IActionResult Index()
		{
			return View(userManager.Users);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]

		public async Task<IActionResult> Create(User user)
		{
			if (ModelState.IsValid)
			{

				AppUser appUser = new AppUser
				{

					UserName = user.Name,
					Email = user.Email,

				};
				IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

				if (result.Succeeded)


					return RedirectToAction("Index");
				else
				{

					foreach (IdentityError item in result.Errors)
					{
						ModelState.AddModelError("CreateUser", $"{item.Code}) -{item.Description}"); //hataları vercek

					}
				}



			}
			return View(user);
		}



		public async Task<IActionResult> Update(string id)
		{
			AppUser user = await userManager.FindByIdAsync(id);


			if (user != null)
			{

				return View(user);

			}
			else
			{
				return RedirectToAction("Index");


			}

		}
		[HttpPost]
		public async Task<IActionResult> Update(string id, string email, string password)
		{
			AppUser user = await userManager.FindByIdAsync(id);
			if (user != null)
			{

				if (!string.IsNullOrEmpty(email))
					user.Email = email;
				else ModelState.AddModelError("UpdateUser", "Email cannot be empty");

				if (!string.IsNullOrEmpty(password))
					user.PasswordHash = passwordHasher.HashPassword(user, password);


				else
					ModelState.AddModelError("UpdateUser", "Password cannot be empty");


				if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
				{
					IdentityResult result = await userManager.UpdateAsync(user);
					if (result.Succeeded)

						return RedirectToAction("Index");
					else
						Errors(result);


				}
			}
			else ModelState.AddModelError("UpdateUser", "User Not Found");
			return View(user);

		}

		private void Errors(IdentityResult result)
		{
			foreach (IdentityError item in result.Errors)
			{
				ModelState.AddModelError("UpdateUser", $"{item.Code}-{item.Description}");
			}

		}
		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			AppUser user = await userManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult result = await userManager.DeleteAsync(user);

				if (result.Succeeded) return RedirectToAction("Index");
				else Errors(result);
			}

			else ModelState.AddModelError("DeleteUser", "User Not Found");
			return View("Index",userManager.Users);

		}




	}




}
