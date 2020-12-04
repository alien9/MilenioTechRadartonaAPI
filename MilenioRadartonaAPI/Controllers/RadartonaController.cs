using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MilenioRadartonaAPI.DTO;
using MilenioRadartonaAPI.Models;
using MilenioRadartonaAPI.Models.Postgres;
using MilenioRadartonaAPI.Relatorios;
using MilenioRadartonaAPI.Repository;
using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MilenioRadartonaAPI.Controllers
{
    public class RadartonaController : Controller
    {

        private readonly IRadartonaService _serv;
        private readonly IRadartonaRepository _rep;
        private readonly IOptions<MyConfig> _config;

        public RadartonaController(IRadartonaService serv, IRadartonaRepository rep, IOptions<MyConfig> config)
        {
            this._config = config;
            _serv = serv;
            _rep = rep;
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetLocalizacaoRadares")]
        public async Task<IActionResult> GetLocalizacaoRadares()
        {
            int QtdRequests = await QtdRequestsDia();
            
            if (QtdRequests < _config.Value.QtdMaxRequisicoes)
            {
                return Ok(_serv.GetLocalizacaoRadares());
            }
            else
            {
                return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
            }

        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetRadaresTipoEnquadramento")]
        public async Task<IActionResult> GetRadaresTipoEnquadramento(string enquadramento)
        {
            if (enquadramento != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresDTO> lstRetorno = _serv.GetRadaresTipoEnquadramento(enquadramento.ToUpper(), _config.Value.connString);

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;
                    await LogRequest("/v1/GetRadaresTipoEnquadramento", TempoRequisicao, _config.Value.connString);

                    if (lstRetorno.Count == 0)
                    {
                        return StatusCode(400, " Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/EnquadramentoRadar");
                    }

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/EnquadramentoRadar");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetRadaresLote")]
        public async Task<IActionResult> GetRadaresLote(int lote)
        {
            if ((lote == 1 || lote == 2 || lote == 3 || lote == 4))
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresDTO> lstRetorno = _serv.GetRadaresLote(lote, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetRadaresLote", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                //TODO: alterar o link da doc pra o final
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/EnquadramentoRadar");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetFluxoVeiculosRadares")]
        public async Task<IActionResult> GetFluxoVeiculosRadares(string radares, string dataConsulta)
        {
            if (radares != null && dataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresJoinContagemDTO> lstRetorno = _serv.GetFluxoVeiculosRadares(radares, dataConsulta, _config.Value.connString);

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;
                    await LogRequest("/v1/GetFluxoVeiculosRadares", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/FluxoVeiculosRadar");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetTipoVeiculosRadares")]
        public async Task<IActionResult> GetTipoVeiculosRadares(string Radares, string DataConsulta)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresJoinContagemPequenoDTO> lstRetorno = _serv.GetTipoVeiculosRadares(Radares, DataConsulta, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetTipoVeiculosRadares", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/TipoVeiculosRadares");
            }
        }


        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetInfracoesRadares")]
        public async Task<IActionResult> GetInfracoesRadares(string Radares, string DataConsulta)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresJoinContagemPequenoDTO2> lstRetorno = _serv.GetInfracoesPorRadar(Radares, DataConsulta, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetInfracoesRadares", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/InfracoesRadares");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetAcuraciaIdentificacaoRadares")]
        public async Task<IActionResult> GetAcuraciaIdentificacaoRadares(string Radares, string DataConsulta)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresJoinContagemPequenoDTO3> lstRetorno = _serv.GetAcuraciaIdentificacaoRadares(Radares, DataConsulta, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetAcuraciaIdentificacaoRadares", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/AcuraciaIdentificacaoRadares");
            }
        }


        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetPerfilVelocidadesRadar")]
        public async  Task<IActionResult> GetPerfilVelocidadesRadar(int VelocidadeMin, int VelocidadeMax)
        {
            if (VelocidadeMin > 0 && VelocidadeMax <= 100)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<BaseRadaresDTO> lstRetorno = _serv.GetPerfilVelocidadesRadar(VelocidadeMin, VelocidadeMax, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetPerfilVelocidadesRadar", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Radares/PerfilVelocidadesRadar");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetTrajetos")]
        public async Task<IActionResult> GetTrajetos(string DataConsulta, string Radares)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<TrajetosDTO> lstRetorno = _serv.GetTrajetos(DataConsulta, Radares, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetTrajetos", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Trajetos/Trajetos");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetVelocidadeMediaTrajeto")]
        public async Task<IActionResult> GetVelocidadeMediaTrajeto(string DataConsulta, string Radares)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<VelocidadeMediaTrajetoDTO> lstRetorno = _serv.GetVelocidadeMediaTrajeto(DataConsulta, Radares, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetVelocidadeMediaTrajeto", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Trajetos/VelocidadeMediaTrajeto");
            }
        }

        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetViagens")]
        public async Task<IActionResult> GetViagens(string DataConsulta, string Radares)
        {
            if (Radares != null && DataConsulta != null)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<ViagensDTO> lstRetorno = _serv.GetViagens(DataConsulta, Radares, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetViagens", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Viagens/Viagens");
            }
        }


        [HttpGet]
        [Authorize("Bearer")]
        [Route("/v1/GetDistanciaViagem")]
        public async Task<IActionResult> GetDistanciaViagem(int radarInicial, int radarFinal)
        {
            if (radarInicial.ToString().Length == 4 && radarFinal.ToString().Length == 4)
            {
                int QtdRequests = await QtdRequestsDia();

                if (QtdRequests < _config.Value.QtdMaxRequisicoes)
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    List<DistanciaViagemDTO> lstRetorno = _serv.GetDistanciaViagem(radarInicial, radarFinal, _config.Value.connString);
                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetDistanciaViagem", TempoRequisicao, _config.Value.connString);

                    return Ok(lstRetorno);
                }
                else
                {
                    return StatusCode(400, " Ultrapassou o Limite De 1000 Requisições Por Dia... Ou Algum Parâmetro Se Encontra Inválido...\n Por Favor Verifique Os Campos e Tente Novamente...\n Para mais informações:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3");
                }
            }
            else
            {
                return StatusCode(400, " Campos Inválidos! Revise Sua Query String e Tente Novamente...\n Se o Problema Persistir, Visite Nossa Documentação:\n>>> https://app.swaggerhub.com/apis-docs/willianchan/API_Milenio_Bus_Radartona/1.0.0-oas3#/Viagens/DistanciaViagem");
            }

        }

        // ====== FUNCOES ========
        private async Task LogRequest(string Endpoint, long TempoRequisicao, string connString)
        {
            string Usuario = String.Empty;
            try
            {
                Usuario = User.Identity.Name;
            }
            catch { }
            
            await _serv.LogRequest(Usuario, Endpoint, TempoRequisicao, connString);
        }

        private async Task<int> QtdRequestsDia()
        {
            string Usuario = String.Empty;
            string connString = _config.Value.connString;
            try
            {
                Usuario = User.Identity.Name;
            }
            catch { }


            return await _serv.QtdRequestsDia(Usuario, connString);
        }

        private bool VerificaData(string data)
        {
            try
            {
                var dtValida = DateTime.Parse(data);
                if (data.Length != 10)
                {
                    throw new Exception();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }


        // ======== CSV ========

        [HttpGet]
        [Route("/v1/GetLocalizacaoRadaresCSV")]
        public async Task<IActionResult> GetLocalizacaoRadaresCSV()
        {
            if (User.Identity.IsAuthenticated)
            {
                var watch = new Stopwatch();
                watch.Start();
                byte[] csv = _serv.GetLocalizacaoRadaresCSV(_config.Value.connString);
                var result = new FileContentResult(csv, "application/octet-stream");
                result.FileDownloadName = "LocalizacaoRadares.csv";

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;

                await LogRequest("/v1/GetLocalizacaoRadaresCSV", TempoRequisicao, _config.Value.connString);

                return result;
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }
        }

        [HttpGet]
        [Route("/v1/GetRadaresTipoEnquadramentoCSV")]
        public async Task<IActionResult> GetRadaresTipoEnquadramentoCSV(string Enquadramento)
        {
            if (User.Identity.IsAuthenticated)
            {
                var watch = new Stopwatch();
                watch.Start();
                byte[] csv = _serv.GetRadaresTipoEnquadramentoCSV(Enquadramento, _config.Value.connString);
                var result = new FileContentResult(csv, "application/octet-stream");
                result.FileDownloadName = "RadaresTipoEnquadramento.csv";

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;

                await LogRequest("/v1/GetRadaresTipoEnquadramentoCSV", TempoRequisicao, _config.Value.connString);

                return result;
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }
        }

        [HttpGet]
        [Route("/v1/GetRadaresLoteCSV")]
        public async Task<IActionResult> GetRadaresLoteCSV(int lote)
        {
            if (User.Identity.IsAuthenticated)
            {
                var watch = new Stopwatch();
                watch.Start();
                byte[] csv = _serv.GetRadaresLoteCSV(lote, _config.Value.connString);
                var result = new FileContentResult(csv, "application/octet-stream");
                result.FileDownloadName = String.Format("RadaresLote{0}.csv", lote);

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;

                await LogRequest("/v1/GetRadaresLoteCSV", TempoRequisicao, _config.Value.connString);

                return result;
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }
        }

        [HttpGet]
        [Route("/v1/GetFluxoVeiculosRadaresCSV")]
        public async Task<IActionResult> GetFluxoVeiculosRadaresCSV(string Radares, string DataConsulta)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetFluxoVeiculosRadaresCSV(Radares, DataConsulta, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "FluxoVeiculosRadares.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetFluxoVeiculosRadaresCSV", TempoRequisicao, _config.Value.connString);

                    return result;

                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetTipoVeiculosRadaresCSV")]
        public async Task<IActionResult> GetTipoVeiculosRadaresCSV(string Radares, string DataConsulta)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetTipoVeiculosRadaresCSV(Radares, DataConsulta, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "TipoVeiculosRadares.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetTipoVeiculosRadaresCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetInfracoesRadaresCSV")]
        public async Task<IActionResult> GetInfracoesRadaresCSV(string Radares, string DataConsulta)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {

                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetInfracoesPorRadarCSV(Radares, DataConsulta, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "InfracoesRadares.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetInfracoesRadaresCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetAcuraciaIdentificacaoRadaresCSV")]
        public async Task<IActionResult> GetAcuraciaIdentificacaoRadaresCSV(string Radares, string DataConsulta)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {

                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetAcuraciaIdentificacaoRadaresCSV(Radares, DataConsulta, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "AcuraciaIdentificacaoRadares.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/AcuraciaIdentificacaoRadaresCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }
        }

        [HttpGet]
        [Route("/v1/GetPerfilVelocidadesRadarCSV")]
        public async Task<IActionResult> GetPerfilVelocidadesRadarCSV(int VelocidadeMin, int VelocidadeMax)
        {
            if (User.Identity.IsAuthenticated)
            {
                var watch = new Stopwatch();
                watch.Start();
                byte[] csv = _serv.GetPerfilVelocidadesRadarCSV(VelocidadeMin, VelocidadeMax, _config.Value.connString);
                var result = new FileContentResult(csv, "application/octet-stream");
                result.FileDownloadName = "PerfilVelocidadesRadar.csv";

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;

                await LogRequest("/v1/GetPerfilVelocidadesRadarCSV", TempoRequisicao, _config.Value.connString);

                return result;
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetTrajetosCSV")]
        public async Task<IActionResult> GetTrajetosCSV(string DataConsulta, string Radares)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetTrajetosCSV(DataConsulta, Radares, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "Trajetos.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;

                    await LogRequest("/v1/GetTrajetosCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetVelocidadeMediaTrajetoCSV")]
        public async Task<IActionResult> GetVelocidadeMediaTrajetoCSV(string DataConsulta, string Radares)
        {
            if (User.Identity.IsAuthenticated)
            {
                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetVelocidadeMediaTrajetoCSV(DataConsulta, Radares, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "VelocidadeMediaTrajeto.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;
                    await LogRequest("/v1/GetVelocidadeMediaTrajetoCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }


        [HttpGet]
        [Route("/v1/GetViagensCSV")]
        public async Task<IActionResult> GetViagensCSV(string DataConsulta, string Radares)
        {
            if (User.Identity.IsAuthenticated)
            {

                var dataValida = VerificaData(DataConsulta);
                if (dataValida == false)
                {
                    return LocalRedirect("~/Intra/Index?erro=DataErrada");
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    byte[] csv = _serv.GetViagensCSV(DataConsulta, Radares, _config.Value.connString);
                    var result = new FileContentResult(csv, "application/octet-stream");
                    result.FileDownloadName = "GetViagens.csv";

                    watch.Stop();
                    var TempoRequisicao = watch.ElapsedMilliseconds;
                    await LogRequest("/v1/GetViagensCSV", TempoRequisicao, _config.Value.connString);

                    return result;
                }
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }

        [HttpGet]
        [Route("/v1/GetDistanciaViagemCSV")]
        public async Task<IActionResult> GetDistanciaViagemCSV(int radarInicial, int radarFinal)
        {
            if (User.Identity.IsAuthenticated)
            {
                var watch = new Stopwatch();
                watch.Start();
                byte[] csv = _serv.GetDistanciaViagemCSV(radarInicial, radarFinal, _config.Value.connString);
                var result = new FileContentResult(csv, "application/octet-stream");
                result.FileDownloadName = "GetDistanciaViagem.csv";

                watch.Stop();
                var TempoRequisicao = watch.ElapsedMilliseconds;
                await LogRequest("/v1/GetDistanciaViagemCSV", TempoRequisicao, _config.Value.connString);

                return result;
            }
            else
            {
                return StatusCode(401, " Usuário Não Autenticado, Para Baixar CSVs é Necessário Realizar o Login No Site e Baixar Através Da Nossa Plataforma:\n >>> https://mileniotech.com.br/Identity/Account/Login");
            }

        }









    }
}

