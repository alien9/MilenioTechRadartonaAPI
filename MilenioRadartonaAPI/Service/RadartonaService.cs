using MilenioRadartonaAPI.DTO;
using MilenioRadartonaAPI.Models;
using MilenioRadartonaAPI.Models.Postgres;
using MilenioRadartonaAPI.Relatorios;
using MilenioRadartonaAPI.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Service
{
    public interface IRadartonaService
    {
        List<BaseRadaresDTO> GetLocalizacaoRadares();
        List<BaseRadaresDTO> GetRadaresTipoEnquadramento(string enquadramento);
        List<BaseRadaresDTO> GetRadaresLote(int lote);
        List<BaseRadaresJoinContagemDTO> GetFluxoVeiculosRadares(string radares, string dataConsulta);
        List<BaseRadaresJoinContagemPequenoDTO> GetTipoVeiculosRadares(string Radares, string DataConsulta);
        List<BaseRadaresJoinContagemPequenoDTO2> GetInfracoesPorRadar(string Radares, string DataConsulta);
        List<BaseRadaresJoinContagemPequenoDTO3> GetAcuraciaIdentificacaoRadares(string Radares, string DataConsulta);
        List<BaseRadaresDTO> GetPerfilVelocidadesRadar(int VelocidadeMin, int VelocidadeMax);
        List<TrajetosDTO> GetTrajetos(string DataConsulta, string Radares);
        List<VelocidadeMediaTrajetoDTO> GetVelocidadeMediaTrajeto(string DataConsulta, string Radares);
        List<ViagensDTO> GetViagens(string DataConsulta, string Radares);
        List<DistanciaViagemDTO> GetDistanciaViagem(int radarInicial, int radarFinal);
        Task LogRequest(string Usuario, string Endpoint, long TempoRequisicao, string connString);

        // ======= CSV =======
        byte[] GetLocalizacaoRadaresCSV();
        byte[] GetRadaresTipoEnquadramentoCSV(string Enquadramento);
        byte[] GetRadaresLoteCSV(int lote);
        byte[] GetFluxoVeiculosRadaresCSV(string Radares, string DataConsulta);
        byte[] GetTipoVeiculosRadaresCSV(string Radares, string DataConsulta);
        byte[] GetInfracoesPorRadarCSV(string Radares, string DataConsulta);
        byte[] GetAcuraciaIdentificacaoRadaresCSV(string Radares, string DataConsulta);
        byte[] GetPerfilVelocidadesRadarCSV(int VelocidadeMin, int VelocidadeMax);
        byte[] GetTrajetosCSV(string DataConsulta, string Radares);
        byte[] GetVelocidadeMediaTrajetoCSV(string DataConsulta, string Radares);
        byte[] GetViagensCSV(string DataConsulta, string Radares);
        byte[] GetDistanciaViagemCSV(int radarInicial, int radarFinal);

        Task<int> QtdRequestsDia(string Usuario, string connString);
    }


    public class RadartonaService : IRadartonaService
    {

        private readonly IRadartonaRepository _rep;

        public RadartonaService(IRadartonaRepository rep)
        {
            _rep = rep;
        }

        public List<BaseRadaresDTO> GetLocalizacaoRadares()
        {
            try
            {
                var achado = _rep.GetLocalizacaoRadares();
                List<BaseRadaresDTO> lista = JsonConvert.DeserializeObject<List<BaseRadaresDTO>>(achado.JsonRetorno);
                return lista;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<BaseRadaresDTO> GetRadaresTipoEnquadramento(string enquadramento)
        {
            string[] Enquadramentos = enquadramento.Split(",");
            var listaRetorno = _rep.GetRadaresTipoEnquadramento(Enquadramentos);
            List<RadaresTipoEnquadramento> listaNormal = new List<RadaresTipoEnquadramento>();
            List<BaseRadaresDTO> retornoMesmo = new List<BaseRadaresDTO>();

            try {
                foreach(List<RadaresTipoEnquadramento> re in listaRetorno)
                {
                    var array = re.ToArray();
                    foreach (RadaresTipoEnquadramento ra in array)
                    {
                        List<BaseRadaresDTO> radares = JsonConvert.DeserializeObject<List<BaseRadaresDTO>>(ra.JsonRetorno);
                        foreach (BaseRadaresDTO r in radares)
                        {
                            retornoMesmo.Add(r);
                        }

                    }
                }
                return retornoMesmo;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public List<BaseRadaresDTO> GetRadaresLote(int lote)
        {
            var lista = _rep.GetRadaresLote(lote);
            try
            {
                List<BaseRadaresDTO> retorno = new List<BaseRadaresDTO>();
                foreach (RadaresLote rad in lista)
                {
                    List<BaseRadaresDTO> radares = JsonConvert.DeserializeObject<List<BaseRadaresDTO>>(rad.JsonRetorno);
                    foreach (BaseRadaresDTO radar in radares)
                    {
                        retorno.Add(radar);
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<BaseRadaresJoinContagemDTO> GetFluxoVeiculosRadares(string Radares, string DataConsulta)
        {
            string[] lstRadares = Radares.Split(",");
            var retornoInicial = _rep.GetFluxoVeiculosRadares(lstRadares, DataConsulta);

            try
            {
                List<BaseRadaresJoinContagemDTO> retorno = new List<BaseRadaresJoinContagemDTO>();
                foreach (FluxoVeiculosRadares fvr in retornoInicial)
                {
                    List<BaseRadaresJoinContagemDTO> radares = JsonConvert.DeserializeObject<List<BaseRadaresJoinContagemDTO>>(fvr.JsonRetorno);
                    foreach (BaseRadaresJoinContagemDTO radar in radares)
                    {
                        retorno.Add(radar);
                    }
                }
                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public List<BaseRadaresJoinContagemPequenoDTO> GetTipoVeiculosRadares(string Radares, string DataConsulta)
        {
            string[] lstRadares = Radares.Split(",");
            try
            {
                var retornoInicial = _rep.GetTipoVeiculosRadares(lstRadares, DataConsulta);

                List<BaseRadaresJoinContagemPequenoDTO> retorno = new List<BaseRadaresJoinContagemPequenoDTO>();
                foreach (TipoVeiculosRadares fvr in retornoInicial)
                {
                    List<BaseRadaresJoinContagemPequenoDTO> radares = JsonConvert.DeserializeObject<List<BaseRadaresJoinContagemPequenoDTO>>(fvr.JsonRetorno);
                    foreach (BaseRadaresJoinContagemPequenoDTO radar in radares)
                    {
                        retorno.Add(radar);
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<BaseRadaresJoinContagemPequenoDTO2> GetInfracoesPorRadar(string Radares, string DataConsulta)
        {
            string[] lstRadares = Radares.Split(",");

            try
            {
                var retornoInicial = _rep.GetInfracoesPorRadar(lstRadares, DataConsulta);

                List<BaseRadaresJoinContagemPequenoDTO2> retorno = new List<BaseRadaresJoinContagemPequenoDTO2>();
                foreach (InfracoesRadares fvr in retornoInicial)
                {
                    List<BaseRadaresJoinContagemPequenoDTO2> radares = JsonConvert.DeserializeObject<List<BaseRadaresJoinContagemPequenoDTO2>>(fvr.JsonRetorno);
                    foreach (BaseRadaresJoinContagemPequenoDTO2 radar in radares)
                    {
                        retorno.Add(radar);
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public List<BaseRadaresJoinContagemPequenoDTO3> GetAcuraciaIdentificacaoRadares(string Radares, string DataConsulta)
        {
            string[] lstRadares = Radares.Split(",");

            try
            {
                var retornoInicial = _rep.GetAcuraciaIdentificacaoRadares(lstRadares, DataConsulta);

                List<BaseRadaresJoinContagemPequenoDTO3> retorno = new List<BaseRadaresJoinContagemPequenoDTO3>();
                foreach (AcuraciaIdentificacaoRadares fvr in retornoInicial)
                {
                    List<BaseRadaresJoinContagemPequenoDTO3> radares = JsonConvert.DeserializeObject<List<BaseRadaresJoinContagemPequenoDTO3>>(fvr.JsonRetorno);
                    foreach (BaseRadaresJoinContagemPequenoDTO3 radar in radares)
                    {
                        retorno.Add(radar);
                    }
                }

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<BaseRadaresDTO> GetPerfilVelocidadesRadar(int VelocidadeMin, int VelocidadeMax)
        {
            try
            {
                return _rep.GetPerfilVelocidadesRadar(VelocidadeMin, VelocidadeMax);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<TrajetosDTO> GetTrajetos(string DataConsulta, string Radares)
        {
            string[] lstRadares = Radares.Split(",");
            try
            {
                return _rep.GetTrajetos(lstRadares, DataConsulta);

                //List<MilenioRadartonaAPI.DTO.Trajeto> retorno = new List<MilenioRadartonaAPI.DTO.Trajeto>();
                //foreach (MilenioRadartonaAPI.Models.Trajetos fvr in retornoInicial)
                //{
                //    List<Trajeto> radares = JsonConvert.DeserializeObject<List<Trajeto>>(fvr.JsonRetorno);
                //    foreach (Trajeto radar in radares)
                //    {
                //        retorno.Add(radar);
                //    }
                //}

                //return retorno;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public List<VelocidadeMediaTrajetoDTO> GetVelocidadeMediaTrajeto(string DataConsulta, string Radares)
        {
            string[] lstRadares = Radares.Split(",");
            try
            {

                List<VelocidadeMediaTrajetoDTO> retorno = _rep.GetVelocidadeMediaTrajeto(DataConsulta, lstRadares);
                //var retornoInicial = _rep.GetVelocidadeMediaTrajeto(lstRadares, DataConsulta);

                //List<TrajetoVelocidadeMedia> retorno = new List<TrajetoVelocidadeMedia>();
                //foreach (MilenioRadartonaAPI.Models.Trajetos fvr in retornoInicial)  // AQUI É OUTRO RETORNO VER QUAL RETORNO
                //{
                //    List<TrajetoVelocidadeMedia> radares = JsonConvert.DeserializeObject<List<TrajetoVelocidadeMedia>>(fvr.JsonRetorno);
                //    foreach (TrajetoVelocidadeMedia radar in radares)
                //    {
                //        retorno.Add(radar);
                //    }
                //}

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public List<ViagensDTO> GetViagens(string DataConsulta, string Radares)
        {
            string[] lstRadares = Radares.Split(",");
            try
            {
                List<ViagensDTO> retorno = _rep.GetViagens(DataConsulta, lstRadares);

                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<DistanciaViagemDTO> GetDistanciaViagem(int radarInicial, int radarFinal)
        {
            try
            {
                List<DistanciaViagemDTO> retorno = _rep.GetDistanciaViagem(radarInicial, radarFinal);
                
                return retorno;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // === FUNCOES

        public async Task LogRequest(string Usuario, string Endpoint, long TempoRequisicao, string connString)
        {
            await _rep.LogRequest(Usuario, Endpoint, TempoRequisicao,connString);
        }

        public async Task<int> QtdRequestsDia(string Usuario, string connString)
        {
            return await _rep.QtdRequestsDia(Usuario, connString);
        }

        // ======== CSV ======== 
        public byte[] GetLocalizacaoRadaresCSV()
        {
            try
            {
                return _rep.GetLocalizacaoRadaresCSV();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetRadaresTipoEnquadramentoCSV(string Enquadramento)
        {
            try
            {
                string[] Enquadramentos = Enquadramento.Split(",");
                return _rep.GetRadaresTipoEnquadramentoCSV(Enquadramentos);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetRadaresLoteCSV(int lote)
        {
            try
            {
                return _rep.GetRadaresLoteCSV(lote);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetFluxoVeiculosRadaresCSV(string Radares, string DataConsulta)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetFluxoVeiculosRadaresCSV(lstRadares, DataConsulta);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetTipoVeiculosRadaresCSV(string Radares, string DataConsulta)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetTipoVeiculosRadaresCSV(lstRadares, DataConsulta);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetInfracoesPorRadarCSV(string Radares, string DataConsulta)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetInfracoesPorRadarCSV(lstRadares, DataConsulta);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetAcuraciaIdentificacaoRadaresCSV(string Radares, string DataConsulta)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetAcuraciaIdentificacaoRadaresCSV(lstRadares, DataConsulta);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetPerfilVelocidadesRadarCSV(int VelocidadeMin, int VelocidadeMax)
        {
            try
            {
                return _rep.GetPerfilVelocidadesRadarCSV(VelocidadeMin, VelocidadeMax);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetTrajetosCSV(string DataConsulta, string Radares)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetTrajetosCSV(DataConsulta, lstRadares);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetVelocidadeMediaTrajetoCSV(string DataConsulta, string Radares)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetVelocidadeMediaTrajetoCSV(DataConsulta, lstRadares);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public byte[] GetViagensCSV(string DataConsulta, string Radares)
        {
            try
            {
                string[] lstRadares = Radares.Split(",");
                return _rep.GetViagensCSV(DataConsulta, lstRadares);
            }
            catch (Exception e)
            {
                return null;
            }            
        }

        public byte[] GetDistanciaViagemCSV(int radarInicial, int radarFinal)
        {
            try
            {
                return _rep.GetDistanciaViagemCSV(radarInicial, radarFinal);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
