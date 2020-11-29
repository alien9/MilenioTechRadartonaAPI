using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MilenioRadartonaAPI.Areas.Identity.Data;
using MilenioRadartonaAPI.Context;
using MilenioRadartonaAPI.DTO;
using MilenioRadartonaAPI.Models;
using MilenioRadartonaAPI.Token;

using Service;

namespace MilenioRadartonaAPI.Controllers
{

    //[Route("api")]
    public class LoginController : Controller
    {

        /*service*/
        private readonly IRadartonaService _serv;
        private IOptions<MyConfig> _config;
        private readonly ApplicationContext _ctx;
        private readonly UserManager<MilenioRadartonaAPIUser> _userManager;

        public LoginController(ApplicationContext ctx, UserManager<MilenioRadartonaAPIUser> userManager, IRadartonaService serv, IOptions<MyConfig> config)
        {
            _ctx = ctx;
            _userManager = userManager;
            _serv = serv;
            _config = config;
        }



        [AllowAnonymous]
        [Route("api/Login/{email}/{senha}")]
        [HttpGet]
        public IActionResult Post(
            //[FromBody]User usuario,
            [FromRoute] string email,
            [FromRoute] string senha,
            [FromServices]UserManager<MilenioRadartonaAPIUser> userManager,
            [FromServices]SignInManager<MilenioRadartonaAPIUser> signInManager,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            bool credenciaisValidas = false;
            if (email != null && !String.IsNullOrWhiteSpace(email))
            {
                // Verifica a existência do usuário nas tabelas do
                // ASP.NET Core Identity
                var userIdentity = userManager
                    .FindByNameAsync(email).Result;
                if (userIdentity != null)
                {
                    var cu = userManager.PasswordValidators.Count();
                    var passwordValidator = new PasswordValidator<IdentityUser>();
                    // Efetua o login com base no Id do usuário e sua senha
                    var resultadoLogin = userManager.CheckPasswordAsync(userIdentity, senha)
                        .Result;
                    if (resultadoLogin != null)
                    {
                        // Verifica se o usuário em questão possui
                        // a role Acesso-APIAlturas
                        credenciaisValidas = userManager.IsInRoleAsync(
                            userIdentity, Roles.ROLE_API_RADAR).Result;
                    }
                }
            }

            if (credenciaisValidas)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    new GenericIdentity(email, "Login"),
                    new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                        new Claim(JwtRegisteredClaimNames.UniqueName, email)
                    }
                );

                DateTime dataCriacao = DateTime.Now;
                DateTime dataExpiracao = dataCriacao +
                    TimeSpan.FromDays(tokenConfigurations.Days);

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                {
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity,
                    NotBefore = dataCriacao,
                    Expires = dataExpiracao
                });

                var token = handler.WriteToken(securityToken);

                Chave novaChave =  new Chave()
                {
                    Authenticated = true,
                    Created = dataCriacao,
                    Expiration = dataExpiracao,
                    Token = token,
                    Message = "OK",
                };

                var usuario = _ctx.Usuarios.Where(u => u.Email.Equals(email))
                    .Include(a => a.Chave)
                    .FirstOrDefault();

                if(usuario.Chave == null) { 
                    usuario.Chave = novaChave;
                    _ctx.Attach(usuario).State = EntityState.Modified;
                    _ctx.SaveChanges();
                }

                if (novaChave.Token != usuario.Chave.Token)
                {
                    usuario.Chave = novaChave;
                    _ctx.Attach(usuario).State = EntityState.Modified;
                    _ctx.SaveChanges();
                }

                string returnUrl = Url.Content("~/Intra/Index/");

                return LocalRedirect(returnUrl);

            }
            else
            {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [Route("v1/Login/GetToken")]
        [HttpGet]
        public async Task<IActionResult> GetToken(string email,string senha,
                                      [FromServices]UserManager<MilenioRadartonaAPIUser> userManager,
                                      [FromServices]SignInManager<MilenioRadartonaAPIUser> signInManager,
                                      [FromServices]SigningConfigurations signingConfigurations,
                                      [FromServices]TokenConfigurations tokenConfigurations)
        {

            var watch = new Stopwatch();
            watch.Start();

            bool credenciaisValidas = false;

            // Verifica a existência do usuário nas tabelas do
            // ASP.NET Core Identity
            var userIdentity = userManager
                .FindByNameAsync(email).Result;
            if (userIdentity != null)
            {
                // Efetua o login com base no Id do usuário e sua senha
                var resultadoLogin = signInManager
                    .CheckPasswordSignInAsync(userIdentity, senha, false)
                    .Result;
                if (resultadoLogin.Succeeded)
                {
                    // Verifica se o usuário em questão possui
                    // a role Acesso-APIAlturas
                    credenciaisValidas = userManager.IsInRoleAsync(
                        userIdentity, Roles.ROLE_API_RADAR).Result;
                }
            }

            if (credenciaisValidas)
            {
                Chave chave = _ctx.Usuarios.Where(u => u.Email.Equals(email)).FirstOrDefault().Chave;
                
                if (chave == null || chave.Token == "")
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(email, "Login"),
                        new[] {
                                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                                            new Claim(JwtRegisteredClaimNames.UniqueName, email)
                        }
                    );

                    DateTime dataCriacao = DateTime.Now;
                    DateTime dataExpiracao = dataCriacao +
                        TimeSpan.FromDays(tokenConfigurations.Days);

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Issuer = tokenConfigurations.Issuer,
                        Audience = tokenConfigurations.Audience,
                        SigningCredentials = signingConfigurations.SigningCredentials,
                        Subject = identity,
                        NotBefore = dataCriacao,
                        Expires = dataExpiracao
                    });

                    var token = handler.WriteToken(securityToken);

                    Chave novaChave = new Chave()
                    {
                        Authenticated = true,
                        Created = dataCriacao,
                        Expiration = dataExpiracao,
                        Token = token,
                        Message = "OK",
                    };

                    var usuario = _ctx.Usuarios.Where(u => u.Email.Equals(email))
                        .Include(a => a.Chave)
                        .FirstOrDefault();

                    if (usuario.Chave == null)
                    {
                        usuario.Chave = novaChave;
                        _ctx.Attach(usuario).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }

                    if (novaChave.Token != usuario.Chave.Token)
                    {
                        usuario.Chave = novaChave;
                        _ctx.Attach(usuario).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }

                    chave = novaChave;
                }

                LoginTokenDTO loginToken = new LoginTokenDTO
                {
                    DataCriacao = chave.Created,
                    DataExpiracao = chave.Expiration,
                    Token = chave.Token
                };

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;
                await LogRequest(email, "/Login/GetToken", TempoRequisicao);

                return Ok(loginToken);
            }
            else
            {
                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;

                if (email != null)
                {
                    await LogRequest(email, "/Login/GetToken", TempoRequisicao);
                }
                else
                {
                    await LogRequest("", "/Login/GetToken", TempoRequisicao);
                }

                return StatusCode(401, "Usuario ou senha invalidas");
            }

        }

        private async Task LogRequest(string Usuario, string Endpoint, long TempoRequisicao)
        {
            await _serv.LogRequest(Usuario, Endpoint, TempoRequisicao, _config.Value.connString);
        }
    }
}