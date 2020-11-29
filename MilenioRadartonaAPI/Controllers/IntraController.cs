using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilenioRadartonaAPI.Context;

namespace MilenioRadartonaAPI.Controllers
{
    public class IntraController : Controller
    {

        private readonly ApplicationContext _ctx;

        public IntraController(ApplicationContext ctx)
        {
            _ctx = ctx;
        }


        public IActionResult Index(string erro)
        {
            if (User.Identity.Name != null)
            {
                string email = User.Identity.Name;
                var usuario = _ctx.Usuarios.Where(u => u.Email.Equals(email))
                     .Include(u => u.Chave)
                     .FirstOrDefault();

                ViewData["Token"] = usuario.Chave.Token;
                ViewData["Nome"] = usuario.Nome;
                ViewData["Celular"] = usuario.Celular;
                if (erro != null)
                {
                    if (erro.Equals("DataErrada"))
                    { 
                        ViewData["Erro"] = " Por favor, Insira a Data Correta e No Formato Informado!\n Para Mais informações Visite Nossa Documentação:\n" ;
                    }
                }

                return View();
            }
            else
            {
                return LocalRedirect("~/Identity/Account/Login");
                   
            }
        }

        [Authorize("Bearer")]
        public IActionResult Teste()
        {
            return Ok();
        }



    }


}