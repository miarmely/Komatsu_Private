﻿using Microsoft.AspNetCore.Mvc;
using Presantation.Attributes;


namespace Temsa_Web.Controllers
{
	[Authorization("Editor,Admin,Editör,Yönetici")]
	public class HomeController : Controller
	{
	}
}
