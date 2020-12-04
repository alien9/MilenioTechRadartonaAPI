using MilenioRadartonaAPI.Models;
using MilenioRadartonaAPI.Relatorios;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MilenioRadartonaAPI.Context;
using System.Data;
using MilenioRadartonaAPI.Models.Postgres;
using MilenioRadartonaAPI.DTO;
using System.Threading.Tasks;
using System.Text;
using System.Globalization;

namespace MilenioRadartonaAPI.Repository
{
    public interface IRadartonaRepository
    {
        LocalizacaoRadares GetLocalizacaoRadares();
        List<List<RadaresTipoEnquadramento>> GetRadaresTipoEnquadramento(string[] enquadramentos, string connString);
        List<RadaresLote> GetRadaresLote(int lote, string connString);
        List<FluxoVeiculosRadares> GetFluxoVeiculosRadares(string[] radares, string dataConsulta, string connString);
        List<TipoVeiculosRadares> GetTipoVeiculosRadares(string[] radares, string dataConsulta, string connString);
        List<InfracoesRadares> GetInfracoesPorRadar(string[] radares, string dataConsulta, string connString);
        List<AcuraciaIdentificacaoRadares> GetAcuraciaIdentificacaoRadares(string[] radares, string dataConsulta, string connString);
        List<BaseRadaresDTO> GetPerfilVelocidadesRadar(int velocidadeMin, int velocidadeMax, string connString);
        List<TrajetosDTO> GetTrajetos(string[] radares, string dataConsulta, string connString);
        List<VelocidadeMediaTrajetoDTO> GetVelocidadeMediaTrajeto(string dataConsulta, string[] radares, string connString);
        List<ViagensDTO> GetViagens(string dataConsulta, string[] Radares, string connString);
        List<DistanciaViagemDTO> GetDistanciaViagem(int radarInicio, int radarFinal, string connString);

        Task LogRequest(string Usuario, string Endpoint, long TempoRequisicao, string connString);

        // ======= CSV =======
        byte[] GetLocalizacaoRadaresCSV(string connString);
        byte[] GetRadaresTipoEnquadramentoCSV(string[] Enquadramentos, string connString);
        byte[] GetRadaresLoteCSV(int lote, string connString);
        byte[] GetFluxoVeiculosRadaresCSV(string[] Radares, string DataConsulta, string connString);
        byte[] GetTipoVeiculosRadaresCSV(string[] Radares, string DataConsulta, string connString);
        byte[] GetInfracoesPorRadarCSV(string[] Radares, string DataConsulta, string connString);
        byte[] GetAcuraciaIdentificacaoRadaresCSV(string[] Radares, string DataConsulta, string connString);
        byte[] GetPerfilVelocidadesRadarCSV(int VelocidadeMin, int VelocidadeMax, string connString);
        byte[] GetTrajetosCSV(string DataConsulta, string[] Radares, string connString);
        byte[] GetVelocidadeMediaTrajetoCSV(string DataConsulta, string[] Radares, string connString);
        byte[] GetViagensCSV(string DataConsulta, string[] Radares, string connString);
        byte[] GetDistanciaViagemCSV(int radarInicial, int radarFinal, string connString);

        Task<int> QtdRequestsDia(string Usuario, string connString);
    }


    public class RadartonaRepository : IRadartonaRepository
    {
        private readonly ApplicationContextCamadaVizualizacao _ctxView;
        private readonly ApplicationContext _ctx;
        private readonly MyConfig _config;

        public RadartonaRepository(ApplicationContextCamadaVizualizacao cxtView, ApplicationContext ctx)
        {
            _ctxView = cxtView;
            _ctx = ctx;
        }

        public LocalizacaoRadares GetLocalizacaoRadares()
        {
            var retorno = _ctxView.LocalizacaoRadares.Where(k => k.Id == 1).FirstOrDefault();
            return retorno;
        }


        public List<List<RadaresTipoEnquadramento>> GetRadaresTipoEnquadramento(string[] enquadramentos, string connString)
        {
            List<List<RadaresTipoEnquadramento>> lista = new List<List<RadaresTipoEnquadramento>>();

            for (int i = 0; i < enquadramentos.Length; i ++)
            {
                lista.Add(_ctxView.RadaresTipoEnquadramento.Where(k => k.Enquadramento.Equals(enquadramentos[i])).ToList());
            }

            return lista;
        }


        public List<RadaresLote> GetRadaresLote(int lote, string connString)
        {
            var retorno = _ctxView.RadaresZonaConcessao.Where(k => k.ZonaConcessao.Contains(Convert.ToString(lote))).ToList();
            return retorno;
        }


        public List<FluxoVeiculosRadares> GetFluxoVeiculosRadares(string[] radares, string dataConsulta, string connString)
        {

            List<FluxoVeiculosRadares> lista = new List<FluxoVeiculosRadares>();

            for (int i = 0; i < radares.Length; i++)
            {
                var achado = _ctxView.FluxoVeiculosRadares.Where(k => k.Radares.Contains(radares[i]) && Convert.ToString(k.DataConsulta).Contains(dataConsulta)).FirstOrDefault();
                lista.Add(achado);
            }

            return lista;
        }

        public List<TipoVeiculosRadares> GetTipoVeiculosRadares(string[] radares, string dataConsulta, string connString)
        {
            List<TipoVeiculosRadares> lista = new List<TipoVeiculosRadares>();

            for (int i = 0; i < radares.Length; i++)
            {
                var achado = _ctxView.TipoVeiculosRadares.Where(k => k.Radares.Contains(radares[i]) && Convert.ToString(k.DataConsulta).Contains(dataConsulta)).FirstOrDefault();
                lista.Add(achado);
            }

            return lista;
        }

        public List<InfracoesRadares> GetInfracoesPorRadar(string[] radares, string dataConsulta, string connString)
        {

            List<InfracoesRadares> lista = new List<InfracoesRadares>();

            for (int i = 0; i < radares.Length; i++)
            {
                var achado = _ctxView.InfracoesRadares.Where(k => k.Radares.Contains(radares[i]) && Convert.ToString(k.DataConsulta).Contains(dataConsulta)).FirstOrDefault();
                lista.Add(achado);
            }

            return lista;
        }

        public List<AcuraciaIdentificacaoRadares> GetAcuraciaIdentificacaoRadares(string[] radares, string dataConsulta, string connString)
        {
            List<AcuraciaIdentificacaoRadares> lista = new List<AcuraciaIdentificacaoRadares>();

            for (int i = 0; i < radares.Length; i++)
            {
                var achado = _ctxView.AcuraciaIdentificacaoRadares.Where(k => k.Radares.Contains(radares[i]) && Convert.ToString(k.DataConsulta).Contains(dataConsulta)).FirstOrDefault();
                lista.Add(achado);
            }

            return lista;
        }


        // Não possui camada de visualização, pois query é simples demais
        public List<BaseRadaresDTO> GetPerfilVelocidadesRadar(int velocidadeMin, int velocidadeMax, string connString)
        {
            List<BaseRadaresDTO> lstRetorno = new List<BaseRadaresDTO>();

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select lote, codigo, endereco, sentido, referencia, tipo_equip, enquadrame, qtde_fxs_f,  \n" +
                                   "data_publi, velocidade, lat, lon, bairro  from \"BaseRadares\" b where velocidade_carro_moto between " + velocidadeMin.ToString() + " and " + velocidadeMax + ";";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();


                while (dr.Read())
                {
                    BaseRadaresDTO ett = new BaseRadaresDTO();
                    ett.Lote = Convert.ToInt32(dr["lote"]);
                    ett.Codigo = dr["codigo"].ToString();
                    ett.Endereco = dr["endereco"].ToString();
                    ett.Sentido = dr["sentido"].ToString();
                    ett.Referencia = dr["referencia"].ToString();
                    ett.TipoEquipamento = dr["tipo_equip"].ToString();
                    ett.Enquadramento = dr["enquadrame"].ToString();
                    ett.QtdeFaixas = Convert.ToInt32(dr["qtde_fxs_f"]);
                    ett.DataPublicacao = dr["data_publi"].ToString();
                    ett.Velocidade = dr["velocidade"].ToString();
                    ett.Lat = dr["lat"].ToString();
                    ett.Lon = dr["lon"].ToString();
                    ett.Bairro = dr["bairro"].ToString();

                    lstRetorno.Add(ett);
                }
            }

            return lstRetorno;
        }

        public List<TrajetosDTO> GetTrajetos(string[] radares, string dataConsulta, string connString)
        {
            List<TrajetosDTO> listaRetorno = new List<TrajetosDTO>();

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                string InClause = "";

                for (int i = 0; i < radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        InClause += radares[i];
                    }
                    else
                    {
                        InClause += "," + radares[i];
                    }
                }

                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select t.origem as \"codigoRadarOrigem\",\n" +
                "case \n" +
                    "when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'\n" +
                    "when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'\n" +
                    "when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'\n" +
                    "when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'\n" +
                    "end as \"periodoDia\",\n" +
                "avg((DATE_PART('day', t.data_final::timestamp - t.data_inicio::timestamp) * 24 + \n" +
                "DATE_PART('hour', t.data_final::timestamp - t.data_inicio::timestamp)) * 60 + \n" +
                "DATE_PART('minute', t.data_final::timestamp - t.data_inicio::timestamp)) as \"mediaMinutosTrajeto\", \n" +
                "avg(t.v0) as \"mediaVelOrigem\",\n" +
                "t.destino as \"codigoRadarDestino\",\n" +
                "avg(t.v1) as \"mediaVelDestino\"\n" +
                "from trajetos t\n" +
                "where data_inicio between ('" + dataConsulta + " 00:00:00') and ('" + dataConsulta + " 23:59:59')\n" +
                "and (t.origem in (" + InClause + ") or t.destino in (" + InClause + ")) group by t.viagem_id, \"codigoRadarOrigem\", \"periodoDia\", \"codigoRadarDestino\"";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    TrajetosDTO ett = new TrajetosDTO();
                    ett.codigoRadarOrigem = Convert.ToInt32(dr["codigoRadarOrigem"]);
                    ett.periodoDia = dr["periodoDia"].ToString();
                    ett.mediaMinutosTrajeto = Convert.ToDecimal(dr["mediaMinutosTrajeto"]);
                    ett.mediaVelOrigem = Convert.ToDecimal(dr["mediaVelOrigem"]);
                    ett.codigoRadarDestino = Convert.ToInt32(dr["codigoRadarDestino"]);
                    ett.mediaVelDestino = Convert.ToDecimal(dr["mediaVelDestino"]);

                    listaRetorno.Add(ett);
                }

            }

            return listaRetorno;
        }


        public List<VelocidadeMediaTrajetoDTO> GetVelocidadeMediaTrajeto(string dataConsulta, string[] radares, string connString)
        {
            List<VelocidadeMediaTrajetoDTO> lstRetorno = new List<VelocidadeMediaTrajetoDTO>();

            string InClause = "";

            InClause += "(";

            for (int i = 0; i < radares.Count(); i++)
            {
                if (i == 0)
                {
                    InClause += "'" + radares[i] + "'";
                }
                else
                {
                    InClause += ",'" + radares[i] + "'";
                }
            }

            InClause += ")";

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select  t.viagem_id,\n" +
                                            "t.origem,\n" +
                                            "case when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'\n" +
                                                 "when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'\n" +
                                                 "when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'\n" +
                                                 "when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'\n" +
                                                 "end as periodo_dia,\n" +
                                                    "avg((t.v0 + t.v1) / 2 ) as velocidade_media,\n" +
                                                    "t.destino\n" +
                                            "from trajetos t\n" +
                                            "where data_inicio between ('" + dataConsulta + " 00:00:00') and ('" + dataConsulta + " 23:59:59')\n" +
                                            "and (t.origem in " + InClause + " or t.destino in " + InClause + ")\n" +
                                            "group by t.viagem_id, t.origem, periodo_dia, t.destino;";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    VelocidadeMediaTrajetoDTO ett = new VelocidadeMediaTrajetoDTO();

                    ett.codigoRadarOrigem = Convert.ToInt32(dr["origem"]);
                    ett.periodoDia = dr["periodo_dia"].ToString();
                    ett.velocidadeMedia = Convert.ToDecimal(dr["velocidade_media"]);
                    ett.codigoRadarDestino = Convert.ToInt32(dr["destino"]);

                    lstRetorno.Add(ett);
                }
            }

            return lstRetorno;
        }

        public List<ViagensDTO> GetViagens(string dataConsulta, string[] Radares, string connString)
        {
            List<ViagensDTO> lstRetorno = new List<ViagensDTO>();

            string InClause = "";

            InClause += "(";

            for (int i = 0; i < Radares.Count(); i++)
            {
                if (i == 0)
                {
                    InClause += "'" + Radares[i] + "'";
                }
                else
                {
                    InClause += ",'" + Radares[i] + "'";
                }
            }

            InClause += ")";

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select id, inicio, data_inicio, final, data_final, tipo from viagens\n" +
                    "where data_inicio between ('" + dataConsulta + " 00:00:00') and ('" + dataConsulta + " 23:59:59')\n" +
                    "and (inicio in " + InClause + " or final in " + InClause + ");";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    ViagensDTO ett = new ViagensDTO();

                    ett.ViagemId = Convert.ToInt32(dr["id"]);
                    ett.CodigoRadarInicio = Convert.ToInt32(dr["inicio"]);
                    ett.DataHoraInicio = Convert.ToDateTime(dr["data_inicio"]);
                    ett.CodigoRadarFinal = Convert.ToInt32(dr["final"]);
                    ett.DataHoraFinal = Convert.ToDateTime(dr["data_final"]);
                    ett.TipoVeiculo = Convert.ToInt32(dr["tipo"]);

                    lstRetorno.Add(ett);
                }
            }

            return lstRetorno;
        }

        public List<DistanciaViagemDTO> GetDistanciaViagem(int radarInicio, int radarFinal, string connString)
        {
            List<DistanciaViagemDTO> listaRetorno = new List<DistanciaViagemDTO>();

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText =
                    "select br0.codigo as RadarInicio, " +
                    "br1.codigo as RadarFinal, " +
                    "ST_Distance(ST_Transform(concat('SRID=4326;POINT(', cast(br0.lat as varchar(20)), ' ', cast(br0.lon as varchar(20)), ')')::geometry, 3857), " +
                    "ST_Transform(concat('SRID=4326;POINT(', cast(br1.lat as varchar(20)), ' ', cast(br1.lon as varchar(20)), ')')::geometry, 3857)) * cosd(42.3521) as distancia " +
                    "from base_radares_lat_lon br0 inner join base_radares_lat_lon br1 " +
                    "on br1.codigo = " + radarFinal + " where br0.codigo = " + radarInicio + "; ";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    DistanciaViagemDTO ett = new DistanciaViagemDTO();

                    ett.codigoRadarInicio = Convert.ToInt32(dr["RadarInicio"]);
                    ett.codigoRadarFinal = Convert.ToInt32(dr["RadarFinal"]);
                    ett.distancia = Convert.ToDecimal(dr["distancia"]);

                    listaRetorno.Add(ett);
                }
            }

            return listaRetorno;
        }


        // ===== FUNCOES ========
        public async Task LogRequest(string Usuario, string Endpoint, long TempoRequisicao, string connString)
        {
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();

                comm.CommandType = CommandType.Text;
                comm.CommandText = "INSERT INTO LogRequest (Usuario, Endpoint, TempoRequisicao, DataRequest) VALUES ('" + Usuario + "', '" + Endpoint + "', " + TempoRequisicao.ToString() + ", now());";

                conn.Open();

                await comm.ExecuteNonQueryAsync();

            }
        }

        public async Task<int> QtdRequestsDia(string Usuario, string connString)
        {
            int Requests = 0;

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();

                comm.CommandType = CommandType.Text;
                comm.CommandText = "SELECT COUNT(1) AS Requests FROM LogRequest WHERE Usuario = '" + Usuario + "' and cast(datarequest as date) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = await comm.ExecuteReaderAsync();

                while (dr.Read())
                {
                    Requests = Convert.ToInt32(dr["Requests"]);
                }

            }

            return Requests;
        }


        // ======= CSV ========
        public byte[] GetLocalizacaoRadaresCSV(string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("lote;codigo;endereco;sentido;referencia;tipo_equip;enquadrame;qtde_fxs_f;data_publi;velocidade;lat;lon;bairro");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select lote, codigo, endereco, sentido, referencia, tipo_equip, enquadrame, qtde_fxs_f,  \n" +
                                   "data_publi, velocidade, lat, lon, bairro  from \"BaseRadares\" b;";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = String.Empty;
                    linha += dr["lote"].ToString() + ";";
                    linha += dr["codigo"].ToString() + ";";
                    linha += dr["endereco"].ToString() + ";";
                    linha += dr["sentido"].ToString() + ";";
                    linha += dr["referencia"].ToString() + ";";
                    linha += dr["tipo_equip"].ToString() + ";";
                    linha += dr["enquadrame"].ToString() + ";";
                    try
                    {
                        linha += dr["qtde_fxs_f"].ToString() + ";";
                    }
                    catch { linha += ";"; }

                    linha += dr["data_publi"].ToString() + ";";
                    linha += dr["velocidade"].ToString() + ";";
                    linha += dr["lat"].ToString().Replace(".",",") + ";";
                    linha += dr["lon"].ToString().Replace(".",",") + ";";
                    linha += dr["bairro"].ToString() + ";";


                    sb.AppendLine(linha);
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());
            return csv;
        }

        public byte[] GetRadaresTipoEnquadramentoCSV(string[] Enquadramentos, string connString) // TODO: usar o parametro para a requisicao
        {
            List<BaseRadaresDTO> lstRadares = new List<BaseRadaresDTO>();
            StringBuilder sb = new StringBuilder();
            string linha = String.Empty;

            sb.AppendLine("lote;codigo;endereco;sentido;referencia;tipo_equip;enquadrame;qtde_fxs_f;data_publi;velocidade;lat;lon;bairro;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select lote, codigo, endereco, sentido, referencia, tipo_equip, enquadrame, qtde_fxs_f,  \n" +
                                   "data_publi, velocidade, lat, lon, bairro  from \"BaseRadares\" b;";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    BaseRadaresDTO ett = new BaseRadaresDTO();
                    ett.Lote = Convert.ToInt32(dr["lote"]);
                    ett.Codigo = dr["codigo"].ToString();
                    ett.Endereco = dr["endereco"].ToString();
                    ett.Sentido = dr["sentido"].ToString();
                    ett.Referencia = dr["referencia"].ToString();
                    ett.TipoEquipamento = dr["tipo_equip"].ToString();
                    ett.Enquadramento = dr["enquadrame"].ToString();

                    try
                    {
                        ett.QtdeFaixas = Convert.ToInt32(dr["qtde_fxs_f"]);
                    }
                    catch { ett.QtdeFaixas = 0; }

                    ett.DataPublicacao = dr["data_publi"].ToString();
                    ett.Velocidade = dr["velocidade"].ToString();
                    ett.Lat = dr["lat"].ToString().Replace(".", ",");
                    ett.Lon = dr["lon"].ToString().Replace(".", ",");
                    ett.Bairro = dr["bairro"].ToString();

                    lstRadares.Add(ett);
                }
            }

            lstRadares = lstRadares.Where(d => Enquadramentos.Any(e => d.Enquadramento.Split("-").Contains(e))).ToList();

            foreach (BaseRadaresDTO ett in lstRadares)
            {
                linha += ett.Lote.ToString() + ";";
                linha += ett.Codigo + ";";
                linha += ett.Endereco + ";";
                linha += ett.Sentido + ";";
                linha += ett.Referencia + ";";
                linha += ett.TipoEquipamento + ";";
                linha += ett.Enquadramento + ";";
                linha += ett.QtdeFaixas.ToString() + ";";
                linha += ett.DataPublicacao + ";";
                linha += ett.Velocidade + ";";
                linha += ett.Lat.ToString().Replace(".", ",") + ";";
                linha += ett.Lon.ToString().Replace(".", ",") + ";";
                linha += ett.Bairro + ";";

                sb.AppendLine(linha);
                linha = String.Empty;
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetRadaresLoteCSV(int lote, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("lote;codigo;endereco;sentido;referencia;tipo_equip;enquadrame;qtde_fxs_f;data_publi;velocidade;lat;lon;bairro;");


            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select lote, codigo, endereco, sentido, referencia, tipo_equip, enquadrame, qtde_fxs_f,  \n" +
                                   "data_publi, velocidade, lat, lon, bairro  from \"BaseRadares\" b where lote = '" + lote + "';"; //TODO: tirar a concatenacao pra evitar SQL injection

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = String.Empty;
                    linha += dr["lote"].ToString() + ";";
                    linha += dr["codigo"].ToString() + ";";
                    linha += dr["endereco"].ToString() + ";";
                    linha += dr["sentido"].ToString() + ";";
                    linha += dr["referencia"].ToString() + ";";
                    linha += dr["tipo_equip"].ToString() + ";";
                    linha += dr["enquadrame"].ToString() + ";";

                    try
                    {
                        linha += dr["qtde_fxs_f"].ToString() + ";";
                    }
                    catch { linha += ";"; }

                    linha += dr["data_publi"].ToString() + ";";
                    linha += dr["velocidade"].ToString() + ";";
                    linha += dr["lat"].ToString().Replace(".", ",") + ";";
                    linha += dr["lon"].ToString().Replace(".", ",") + ";";
                    linha += dr["bairro"].ToString() + ";";

                    sb.AppendLine(linha);
                }

                byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

                return csv;
            }
        }

        public byte[] GetFluxoVeiculosRadaresCSV(string[] Radares, string DataConsulta, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo;data_hora;tipo_veiculo;contagem;autuacoes;placas;qtde_faixas;lat;lon;bairro;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {

                string inclause = AssembleClause(Radares);
                string q = "select c.localidade as codigo, c.data_e_hora as data_hora, c.tipo as tipo_veiculo, c.contagem, c.autuacoes, c.placas, b.qtde_fxs_f as qtde_faixas,\n" +
                                   "b.velocidade, b.lat, b.lon, b.bairro\n" +
                                   "from contagens c inner join \"BaseRadares\" b \n" +
                                   "on b.codigo like concat('%', c.localidade, '%')\n" +
                                   "where cast(c.data_e_hora as date) = @d::date ";

                if (inclause.Length > 0)
                {
                    q += "and c.localidade in (" + inclause + ")";

                }
                conn.Open();
                using (var cmd = new Npgsql.NpgsqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("d", DataConsulta);
                    Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string linha = "";
                        linha += dr["codigo"].ToString() + ";";
                        linha += dr["data_hora"].ToString() + ";";
                        linha += dr["tipo_veiculo"].ToString() + ";";
                        linha += dr["contagem"].ToString() + ";";
                        linha += dr["autuacoes"].ToString() + ";";
                        linha += dr["placas"].ToString() + ";";
                        linha += dr["qtde_faixas"].ToString() + ";";
                        linha += dr["lat"].ToString().Replace(".", ",") + ";";
                        linha += dr["lon"].ToString().Replace(".", ",") + ";";
                        linha += dr["bairro"].ToString() + ";";

                        sb.AppendLine(linha);
                    }
                }
            }
            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetTipoVeiculosRadaresCSV(string[] Radares, string DataConsulta, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo;data_hora;contagem;autuacoes;placas;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select localidade as codigo, data_e_hora as data_hora, sum(contagem) as contagem, sum(autuacoes) as autuacoes, sum(placas) as placas from contagens where localidade in (";

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        comm.CommandText += "'" + Radares[i] + "'"; //TODO: tirar a concatenacao pra evitar SQL injection
                    }
                    else
                    {
                        comm.CommandText += ",'" + Radares[i] + "'";
                    }
                }

                comm.CommandText += ") and cast(data_e_hora as date) = '" + DataConsulta + "' group by localidade, data_e_hora order by codigo, data_e_hora;";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = "";

                    linha += dr["codigo"].ToString() + ";";
                    linha += dr["data_hora"].ToString() + ";";
                    linha += dr["contagem"].ToString() + ";";
                    linha += dr["autuacoes"].ToString() + ";";
                    linha += dr["placas"].ToString() + ";";

                    sb.AppendLine(linha);
                }

            }
            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetInfracoesPorRadarCSV(string[] Radares, string DataConsulta, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo_radar;data_hora;tipo_veiculo;contagem;autuacoes;placas;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select id,data_e_hora,localidade,tipo,contagem,autuacoes,placas from contagens where cast(data_e_hora as date) = '" + DataConsulta + "' and localidade in ("; //TODO: tirar a concatenacao pra evitar SQL injection

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        comm.CommandText += "'" + Radares[i] + "'";
                    }
                    else
                    {
                        comm.CommandText += ",'" + Radares[i] + "'";
                    }
                }

                comm.CommandText += ");";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = "";
                    linha += dr["localidade"].ToString() + ";";
                    linha += dr["data_e_hora"].ToString() + ";";
                    linha += dr["tipo"].ToString() + ";";
                    linha += dr["contagem"].ToString() + ";";
                    linha += dr["autuacoes"].ToString() + ";";
                    linha += dr["placas"].ToString() + ";";

                    sb.AppendLine(linha);
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetAcuraciaIdentificacaoRadaresCSV(string[] Radares, string DataConsulta, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo;data_hora;tipo_veiculo;contagem;placas;acuracia_identificacao;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select localidade as codigo, data_e_hora, tipo, contagem, placas, cast(placas as decimal)/cast(contagem as decimal) as acuracia_identificacao\n" +
                                   "from contagens where cast(data_e_hora as date) = '" + DataConsulta + "' and localidade in ("; //TODO: tirar a concatenacao pra evitar SQL injection

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        comm.CommandText += "'" + Radares[i] + "'"; //TODO: tirar a concatenacao pra evitar SQL injection
                    }
                    else
                    {
                        comm.CommandText += ",'" + Radares[i] + "'";
                    }
                }

                comm.CommandText += ");";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = "";

                    linha += dr["codigo"].ToString() + ";";
                    linha += dr["data_e_hora"].ToString() + ";";
                    linha += dr["tipo"].ToString() + ";";
                    linha += dr["contagem"].ToString() + ";";
                    linha += dr["placas"].ToString() + ";";
                    linha += dr["acuracia_identificacao"].ToString().Replace(".", ",") + ";";

                    sb.AppendLine(linha);
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetPerfilVelocidadesRadarCSV(int VelocidadeMin, int VelocidadeMax, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("lote;codigo;endereco;sentido;referencia;tipo_enquadramento;enquadramento;qtde_faixas;data_publicacao;velocidade;lat;lon;bairro;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                comm.CommandType = CommandType.Text;
                comm.CommandText = "select lote, codigo, endereco, sentido, referencia, tipo_equip, enquadrame, qtde_fxs_f,  \n" +
                                   "data_publi, velocidade, lat, lon, bairro  from \"BaseRadares\" b where velocidade_carro_moto between " + VelocidadeMin.ToString() + " and " + VelocidadeMax + ";"; //TODO: tirar a concatenacao pra evitar SQL injection

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = "";

                    linha += dr["lote"].ToString() + ";";
                    linha += dr["codigo"].ToString() + ";";
                    linha += dr["endereco"].ToString() + ";";
                    linha += dr["sentido"].ToString() + ";";
                    linha += dr["referencia"].ToString() + ";";
                    linha += dr["tipo_equip"].ToString() + ";";
                    linha += dr["enquadrame"].ToString() + ";";
                    linha += dr["qtde_fxs_f"].ToString() + ";";
                    linha += dr["data_publi"].ToString() + ";";
                    linha += dr["velocidade"].ToString() + ";";
                    linha += dr["lat"].ToString().Replace(".", ",") + ";";
                    linha += dr["lon"].ToString().Replace(".", ",") + ";";
                    linha += dr["bairro"].ToString() + ";";

                    sb.AppendLine(linha);
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetTrajetosCSV(string DataConsulta, string[] Radares, string connString)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("codigo_radar_origem;media_vel_origem;periodo_dia;media_minutos_trajeto;codigo_radar_destino;media_vel_destino;");
                string InClause = "";

                InClause += "(";

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        InClause += "'" + Radares[i] + "'";
                    }
                    else
                    {
                        InClause += ",'" + Radares[i] + "'";
                    }
                }

                InClause += ")";

                using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
                {
                    Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                    comm.CommandTimeout = 420;

                    comm.CommandType = CommandType.Text;
                    comm.CommandText = "select t.viagem_id,\n" +
                                        "t.origem,\n" +
                                        "case when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'\n" +
                                             "when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'\n" +
                                             "when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'\n" +
                                             "when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'\n" +
                                             "end as periodo_dia,\n" +
                                                "avg((DATE_PART('day', t.data_final::timestamp - t.data_inicio::timestamp) * 24 + \n" +
                                                       "DATE_PART('hour', t.data_final::timestamp - t.data_inicio::timestamp)) * 60 +\n" +
                                                       "DATE_PART('minute', t.data_final::timestamp - t.data_inicio::timestamp)) as media_minutos_trajeto,\n" +
                                                "avg(t.v0) as media_v0,\n" +
                                                "t.destino,\n" +
                                                "avg(t.v1) as media_v1\n" +
                                        "from trajetos t\n" +
                                        "where cast(data_inicio as date) = '" + DataConsulta + "'\n" + //TODO: tirar a concatenacao pra evitar SQL injection
                                        "and (t.origem in " + InClause + " or t.destino in " + InClause + ")\n" +
                                        "group by t.viagem_id, t.origem, periodo_dia, t.destino;";

                    conn.Open();

                    Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                    while (dr.Read())
                    {
                        string linha = "";

                        linha += Convert.ToInt32(dr["origem"]) + ";";
                        linha += dr["media_v0"].ToString().Replace(".", ",") + ";";
                        linha += dr["periodo_dia"].ToString() + ";";
                        linha += dr["media_minutos_trajeto"].ToString().Replace(".", ",") + ";";
                        linha += Convert.ToInt32(dr["destino"]) + ";";
                        linha += dr["media_v1"].ToString().Replace(".", ",") + ";";

                        sb.AppendLine(linha);
                    }
                }
            
            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetVelocidadeMediaTrajetoCSV(string DataConsulta, string[] Radares, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("codigo_radar_origem;periodo_dia;velocidade_media;codigo_radar_destino;");

            string InClause = AssembleClause(Radares);
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;
                comm.CommandType = CommandType.Text;
                comm.CommandText = "select  t.viagem_id,\n" +
                                            "t.origem,\n" +
                                            "case when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'\n" +
                                                 "when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'\n" +
                                                 "when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'\n" +
                                                 "when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'\n" +
                                                 "end as periodo_dia,\n" +
                                                    "avg((t.v0 + t.v1) / 2 ) as velocidade_media,\n" +
                                                    "t.destino\n" +
                                            "from trajetos t\n" +
                                            "where cast(data_inicio as date) = '" + DataConsulta + "'\n" + //TODO: tirar a concatenacao pra evitar SQL injection

                                            "and (t.origem in " + InClause + " or t.destino in " + InClause + ")\n" +
                                            "group by t.viagem_id, t.origem, periodo_dia, t.destino;";

                conn.Open();

                Npgsql.NpgsqlDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    string linha = "";

                    linha += dr["origem"].ToString() + ";";
                    linha += dr["periodo_dia"].ToString() + ";";
                    linha += dr["velocidade_media"].ToString().Replace(".", ",") + ";";
                    linha += dr["destino"].ToString() + ";";

                    sb.AppendLine(linha);
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetViagensCSV(string DataConsulta, string[] Radares, string connString)
        {
            string InClause = "";
            if (Radares.Length > 0)
            {
                InClause += "(";

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        InClause += "'" + Radares[i] + "'"; //TODO: tirar a concatenacao pra evitar SQL injection (isso aí foi inacreditável, milenio)

                    }
                    else
                    {
                        InClause += ",'" + Radares[i] + "'";
                    }
                }

                InClause += ")";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("viagem_id;codigo_radar_inicio;data_hora_inicio;codigo_radar_final;data_hora_final;tipo_veiculo;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                Npgsql.NpgsqlCommand comm = conn.CreateCommand();
                comm.CommandTimeout = 420;

                //comm.CommandType = CommandType.Text;
                string q = "";
                if (InClause.Length > 0)
                {
                    q = "select id, inicio, data_inicio, final, data_final, tipo from viagens where cast(data_inicio as date) = @d::date " +
                        "and (inicio in " + InClause + " or final in " + InClause + ");";
                }
                else
                {
                    q = "select id, inicio, data_inicio, final, data_final, tipo from viagens where cast(data_inicio as date) = @d::date";
                }
                conn.Open();
                using (var cmd = new Npgsql.NpgsqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("d", DataConsulta);
                    Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string linha = "";

                        linha += dr["id"].ToString() + ";";
                        linha += dr["inicio"].ToString() + ";";
                        linha += dr["data_inicio"].ToString() + ";";
                        linha += dr["final"].ToString() + ";";
                        linha += dr["data_final"].ToString() + ";";
                        linha += dr["tipo"].ToString() + ";";
                        sb.AppendLine(linha);
                    }
                }
            }

            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }

        public byte[] GetDistanciaViagemCSV(int radarInicial, int radarFinal, string connString)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("inicio;final;distancia;");

            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(connString))
            {
                string q = "select dist from route r where origem=@o and destino=@d";
                conn.Open();
                using (var cmd = new Npgsql.NpgsqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("o", radarInicial);
                    cmd.Parameters.AddWithValue("d", radarFinal);
                    Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string linha = "";
                        linha += dr["inicio"].ToString() + ";";
                        linha += dr["final"].ToString() + ";";
                        linha += dr["dist"].ToString().Replace(".", ",") + ";";

                        sb.AppendLine(linha);
                    }
                }
            }


            byte[] csv = Encoding.Unicode.GetBytes(sb.ToString());

            return csv;
        }
        string AssembleClause(string[] Radares)
        {
            string InClause = "";
            if (Radares.Length > 0)
            {
                InClause += "(";

                for (int i = 0; i < Radares.Count(); i++)
                {
                    if (i == 0)
                    {
                        InClause += "'" + Radares[i] + "'"; //TODO: tirar a concatenacao pra evitar SQL injection (isso aí foi inacreditável, milenio)

                    }
                    else
                    {
                        InClause += ",'" + Radares[i] + "'";
                    }
                }

                InClause += ")";
            }
            return InClause;

        }
    }
}
