using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wedding.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Wedding.Controllers
{
    public class HomeController : Controller
    {
        private WeddingContext dbContext;
        // here we can "inject" our context service into the constructor
        public HomeController(WeddingContext context)
        {
            dbContext = context;
        }
     

        public IActionResult Index()
        {
            return View();
        }


[HttpPost("/register")]
    public IActionResult register(RegisterUser userFromForm)
    {
        System.Console.WriteLine("Reached Register route!!!!!!!!! *****************************");
        // Check initial ModelState
        if(ModelState.IsValid)
        {
            System.Console.WriteLine("Model state is valid");
            if(dbContext.Logged_In_User.Any(u => u.Email == userFromForm.Email))
            {
                System.Console.WriteLine("Email is not unique");
                ModelState.AddModelError("Email", "Email already in use!");
                return View("Index");
            }
            else
            {
                System.Console.WriteLine("Everything is valid!");
                PasswordHasher<RegisterUser> Hasher = new PasswordHasher<RegisterUser>();
                userFromForm.Password = Hasher.HashPassword(userFromForm, userFromForm.Password);
                System.Console.WriteLine("Password hashed!***************************");
                dbContext.Logged_In_User.Add(userFromForm);
                System.Console.WriteLine("New User Added!**************************");
                dbContext.SaveChanges();
                System.Console.WriteLine("New User Saved!**************************");
                
                HttpContext.Session.SetInt32("LoggedID", userFromForm.UserId);
                return RedirectToAction("dashboard");
            }
        }
        else
        {
            return View("Index");
        }
    } 


[HttpPost("Login")]
    public IActionResult Login(LoginUser userSubmission)
    {
        System.Console.WriteLine("Reached the Login route*************");
        if(ModelState.IsValid)
        {
            System.Console.WriteLine("Model state is valid*************");
            // If inital ModelState is valid, query for a user with provided email
            var userInDb = dbContext.Logged_In_User.FirstOrDefault(u => u.Email == userSubmission.Email);
            // If no user exists with provided email
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "User error, please replace user.");
                System.Console.WriteLine("user not in database... yet**************");
                return View("Index");
            }
            
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();
            
            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            System.Console.WriteLine("passwords match***************************");
            // result can be compared to 0 for failure
            if(result == 0)
            {
                ModelState.AddModelError("Password", "I even gave you a checkbox... retry the");
                return View("Index");
            }
            HttpContext.Session.SetInt32("LoggedID", userInDb.UserId);
            int? idUser = HttpContext.Session.GetInt32("LoggedID");
            System.Console.WriteLine("Should FINALLY be in session....*************");
            return RedirectToAction("dashboard");
        }
        return View("Index");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("LoggedID") != null)
                {
                List<WeddingModel>AllWeddings = dbContext.MyWeddings
                    .Include(w => w.Guests)
                    .ToList();
                List<WeddingModel> MostRecent = dbContext.MyWeddings
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToList();
                    int? idUser = HttpContext.Session.GetInt32("LoggedID");
                    ViewBag.idUser = idUser;
                    System.Console.WriteLine("should be in session***********");
			        return View("dashboard", AllWeddings);
                }
            else{
                ModelState.AddModelError("Email", "Please login to continue.");
                return Redirect("/");
                }
            
        }

    [HttpGet("logout")]
    public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

    [HttpGet("create")]
    public IActionResult Create()
        {
        if(HttpContext.Session.GetInt32("LoggedID") != null)
            {
                return View();
            }
        else{
            ModelState.AddModelError("lEmail", "Please login to continue.");
            return Redirect("/");
            }
        }

    [HttpPost("new")]
    public IActionResult New(WeddingModel newWedding)
        {
            if (ModelState.IsValid) {
                newWedding.CreatedAt = DateTime.Now;
                newWedding.UpdatedAt = DateTime.Now;
                newWedding.RegisterUserId = (int)HttpContext.Session.GetInt32("LoggedID");
                dbContext.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("dashboard");
            }
            else
            {
                return View ("create");
            }
        }

    [HttpGet("details/{weddingID}")]
    public IActionResult Details(int weddingID)
    {
        if(HttpContext.Session.GetInt32("LoggedID") != null)
            {
                WeddingModel aWedding = dbContext.MyWeddings
                    .Where(w => w.WeddingId == weddingID)
                    .Include(w => w.Guests)
                    .ThenInclude(r => r.Creator)
                    .SingleOrDefault();
                    
                return View(aWedding);
            }
            ModelState.AddModelError("Email", "Please login to continue.");
            return View("details");
    }

    [HttpGet("delete/{weddingID}")]
        public IActionResult Delete(int weddingID)
        {
            if(HttpContext.Session.GetInt32("LoggedID") != null)
            {
                int? idUser = HttpContext.Session.GetInt32("LoggedID");
                dbContext.Remove(dbContext.MyWeddings
                .Where(w => w.WeddingId == weddingID)
                .SingleOrDefault());
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("lEmail", "Please login to continue.");
            return View("Index");
        }


    [HttpGet("rsvp/{weddingID}")]
        public IActionResult AddGuest(int weddingID)
        {
            if(HttpContext.Session.GetInt32("LoggedID") != null)
            {
                int? idUser = HttpContext.Session.GetInt32("LoggedID");
                Association newAssociation = new Association();
                newAssociation.RegisterUserId = (int)idUser;
                newAssociation.WeddingModelId = weddingID;
                dbContext.Add(newAssociation);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("lEmail", "Please login to continue.");
            return View("Index");
        }

        [HttpGet("unrsvp/{weddingID}")]
        public IActionResult RemoveGuest(int weddingID)
        {
            if(HttpContext.Session.GetInt32("LoggedID") != null)
            {
                int? idUser = HttpContext.Session.GetInt32("LoggedID");
                dbContext.Remove(dbContext.Associations
                    .Where(r => r.WeddingModelId == weddingID)
                    .Where(r => r.RegisterUserId == (int)idUser)
                    .SingleOrDefault());
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("lEmail", "Please login to continue.");
            return View("Index");
        }






    }
}

