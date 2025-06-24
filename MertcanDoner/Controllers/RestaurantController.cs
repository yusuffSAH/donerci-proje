using Microsoft.AspNetCore.Mvc;
using MertcanDoner.Models;
using System.Collections.Generic;
using System.Linq;

public class RestaurantController : Controller
{
    public IActionResult Location()
    {
        return View();
    }
}
