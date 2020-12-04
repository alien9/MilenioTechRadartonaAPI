--  ************************************
-- 	SCRIPTS DE CRIAÇÃO DE ÍNDICES 
--  ************************************

-- contagens
CREATE INDEX contagens_data_e_hora_idx ON public.contagens (data_e_hora);
CREATE INDEX contagens_localidade_idx ON public.contagens (localidade);
--DROP INDEX contagens_data_e_hora_localidade

-- BaseRadares
CREATE INDEX BaseRadares_codigo ON public."BaseRadares" (codigo);

-- trajetos
-- t.viagem_id, "codigoRadarOrigem", "periodoDia", "codigoRadarDestino"
CREATE INDEX trajetos_origem_idx ON public.trajetos (origem);
CREATE INDEX trajetos_destino_idx ON public.trajetos (destino);
CREATE INDEX trajetos_data_idx ON public.trajetos (data_inicio);
CREATE INDEX trajetos_proc ON public.trajetos (viagem_id, origem, destino);
--DROP INDEX trajetos_proc

-- viagens
CREATE INDEX viagens_inicio_data_inicio ON public.viagens (inicio, data_inicio);
CREATE INDEX viagens_final_data_inicio ON public.viagens (final, data_inicio);

CREATE INDEX viagens_proc ON public.viagens (id, inicio, data_inicio, final, data_final, tipo);

-- PRÉ EXISTENTES
CREATE INDEX idx_data ON public.trajetos USING btree (data_inicio, data_final);
CREATE INDEX idx_orgem_destino ON public.trajetos USING btree (origem, destino);
DROP INDEX idx_data;
DROP INDEX idx_orgem_destino;

CREATE INDEX idx_viagens_tipo_veiculo ON public.viagens USING btree (id, tipo);
CREATE INDEX viagens_inicio_data_inicio ON public.viagens USING btree (inicio, data_inicio);
CREATE INDEX viagens_final_data_inicio ON public.viagens USING btree (final, data_inicio);
DROP INDEX idx_viagens_tipo_veiculo;
DROP INDEX viagens_inicio_data_inicio;
DROP INDEX viagens_final_data_inicio;
--

-- MOSTRA ÍNDICES JÁ CRIADOS
SELECT
    indexname,
    indexdef
FROM
    pg_indexes
WHERE
    tablename = 'contagens';

--  ************************************
-- 		  PROCEDURES DE INSERÇÃO
--  ************************************

-- FluxoVeiculosRadares PROCEDURE
CREATE OR REPLACE FUNCTION InsereFluxoVeiculosRadares(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select c.localidade as codigo, c.data_e_hora as data_hora, c.tipo as tipo_veiculo, c.contagem, c.autuacoes, c.placas, b."qtde_fxs_f" as qtdeFaixas,
				b.velocidade, b.lat, b.lon, b.bairro
				from "contagens" c inner join "BaseRadares" b 
				on b.codigo like concat('%', c.localidade, '%')
				where c.localidade = l
				and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."FluxoVeiculosRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'FluxoVeiculosRadares Processado';
END;
$$ LANGUAGE 'plpgsql';

-- TipoVeiculosRadares PROCEDURE
CREATE OR REPLACE FUNCTION InsereTipoVeiculosRadares(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (				
				select localidade as codigo, data_e_hora as data_hora, sum(contagem) as contagem, sum(autuacoes) as autuacoes,
				sum(placas) as placas from contagens where localidade = l and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				group by localidade, data_e_hora order by codigo, data_e_hora
			) r);
			INSERT INTO camadavisualizacao."TipoVeiculosRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'TipoVeiculosRadares Processado';
END;
$$ LANGUAGE 'plpgsql';

-- InfracoesRadares PROCEDURE
CREATE OR REPLACE FUNCTION InsereInfracoesRadares(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select data_e_hora as "dataHora", localidade as "codigoRadar", tipo as "tipoVeiculo", contagem, autuacoes, placas 
					from contagens where data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59') and localidade = l
			) r);
			INSERT INTO camadavisualizacao."InfracoesRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'InfracoesRadares Processado';
END;
$$ LANGUAGE 'plpgsql';

-- AcuraciaIdentificacaoRadares PROCEDURE
CREATE OR REPLACE FUNCTION InsereAcuraciaIdentificacaoRadares(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select localidade as "codigoRadar", data_e_hora as "dataHora", tipo as "tipoVeiculo", contagem, placas, cast(placas as decimal)/cast(contagem as decimal) as "acuraciaIdentificacao"
                	from contagens where localidade = l and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."AcuraciaIdentificacaoRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'AcuraciaIdentificacaoRadares Processado';
END;
$$ LANGUAGE 'plpgsql';

-- Trajetos PROCEDURE
CREATE OR REPLACE FUNCTION InsereTrajetos(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select t.origem as "codigoRadarOrigem",
				case 
					when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'
					when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'
					when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'
					when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'
					end as "periodoDia",
				avg((DATE_PART('day', t.data_final::timestamp - t.data_inicio::timestamp) * 24 + 
				DATE_PART('hour', t.data_final::timestamp - t.data_inicio::timestamp)) * 60 +
				DATE_PART('minute', t.data_final::timestamp - t.data_inicio::timestamp)) as "mediaMinutosTrajeto",
				avg(t.v0) as "mediaVelOrigem",
				t.destino as "codigoRadarDestino",
				avg(t.v1) as "mediaVelDestino"
				from trajetos t
				where data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				and (t.origem = l or t.destino = l)
				group by t.viagem_id, "codigoRadarOrigem", "periodoDia", "codigoRadarDestino"
			) r);
			INSERT INTO camadavisualizacao."Trajetos" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
-- 			EXIT;
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'Trajetos Processado';
END;
$$ LANGUAGE 'plpgsql';

-- VelocidadeMediaTrajeto PROCEDURE
CREATE OR REPLACE FUNCTION InsereVelocidadeMediaTrajeto(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select t.origem as "codigoRadarOrigem",
				case 
					when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'
					when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'
					when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'
					when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'
					end as "periodoDia",
					avg((t.v0 + t.v1) / 2 ) as "velocidadeMedia",
					t.destino as "codigoRadarDestino"
				from trajetos t
				where data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				and (t.origem = l or t.destino = l)
				group by t.viagem_id, "codigoRadarOrigem", "periodoDia", "codigoRadarDestino"
			) r);
			INSERT INTO camadavisualizacao."VelocidadeMediaTrajeto" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
-- 			EXIT;
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'VelocidadeMediaTrajeto Processado';
END;
$$ LANGUAGE 'plpgsql';

-- Viagens PROCEDURE
CREATE OR REPLACE FUNCTION InsereViagens(DataInicial TIMESTAMP, DataFinal TIMESTAMP)
RETURNS void AS $$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = DataInicial;
	UltimaDataConsulta TIMESTAMP = DataFinal;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
-- 				select id as "viagemId", inicio as "codigoRadarInicio", data_inicio as "dataHoraInicio", final as "codigoRadarFinal", data_final as "dataHoraFinal", tipo as "tipoVeiculo" 
-- 				from viagens where cast(data_inicio as date) = DataConsulta and (inicio = l or final = l)
				select id as "viagemId", inicio as "codigoRadarInicio", data_inicio as "dataHoraInicio", final as "codigoRadarFinal", data_final as "dataHoraFinal", tipo as "tipoVeiculo" 
				from viagens where (inicio = l or final = l) and data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."Viagens" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			EXIT;
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'Viagens Processado';
END;
$$ LANGUAGE 'plpgsql';
--  ************************************
-- 	AQUI COMEÇAM OS SCRIPTS DE INSERÇÃO	 
--  ************************************

-- LocalizacaoRadares
-- RODOU EM 127 ms
DO
$BODY$
DECLARE
    omgjson json := (SELECT json_agg(t) FROM (SELECT lote, codigo, endereco, sentido, referencia, tipo_equip as "tipoEquipamento",
		enquadrame as "enquadramento", qtde_fxs_f as "qtdeFaixas", data_publi as "dataPublicacao", velocidade, lat, lon, bairro,
		velocidade_cam_oni, velocidade_carro_moto FROM public."BaseRadares") t);
BEGIN
	INSERT INTO camadavisualizacao."LocalizacaoRadares" ("JsonRetorno") VALUES (omgjson);
	RAISE NOTICE 'LocalizacaoRadares Processado';
END;
$BODY$ language plpgsql

-- RadaresTipoEnquadramento
--RODOU EM 185 MS
DO
$do$
DECLARE
   m   varchar;
   arr varchar[] := array['R', 'F', 'A', 'P', 'VER', 'V', 'Z', 'PR', 'C', 'EX', 'VR', 'MOTO', 'CP'];
   omgjson json;
BEGIN
	FOREACH m IN ARRAY arr
	LOOP 
		omgjson := (SELECT json_agg(t) FROM (SELECT lote, codigo, endereco, sentido, referencia, tipo_equip as "tipoEquipamento", enquadrame as "enquadramento",
			qtde_fxs_f as "qtdeFaixas", data_publi as "dataPublicacao", velocidade, lat, lon, bairro, velocidade_cam_oni, velocidade_carro_moto
			FROM public."BaseRadares" WHERE replace(enquadrame, ' ', '') LIKE m
			OR replace(enquadrame, ' ', '') LIKE m || '-%' OR replace(enquadrame, ' ', '') LIKE '%-' || m || '-%' OR
			replace(enquadrame, ' ', '') LIKE '%-' || m) t);
		INSERT INTO camadavisualizacao."RadaresTipoEnquadramento" ("Enquadramento","JsonRetorno") VALUES (m, omgjson);
	END LOOP;
	RAISE NOTICE 'RadaresTipoEnquadramento Processado';
END
$do$

-- RadaresZonaConcessao
-- RODOU EM 87 MS
DO
$do$
DECLARE
   i integer;
   arr integer[] := array[1,2,3,4];
   omgjson json;
BEGIN
	FOR i IN 1 .. array_upper(arr, 1)
	LOOP 
		omgjson := (SELECT json_agg(t) FROM (SELECT lote, codigo, endereco, sentido, referencia, tipo_equip as "tipoEquipamento", enquadrame as "enquadramento",
			qtde_fxs_f as "qtdeFaixas", data_publi as "dataPublicacao", velocidade, lat, lon, bairro, velocidade_cam_oni, velocidade_carro_moto
			FROM public."BaseRadares" WHERE lote = arr[i]) t);
 		INSERT INTO camadavisualizacao."RadaresZonaConcessao" ("ZonaConcessao","JsonRetorno") VALUES (arr[i], omgjson);
-- 		RAISE NOTICE 'Saida: [%](%)', i, omgjson;
	END LOOP;
	RAISE NOTICE 'RadaresZonaConcessao Processado';
END
$do$

-- FluxoVeiculosRadares
-- TEMPO DE EXECUÇÃO PARA FEV-2018: 22 minutos
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2019-02-01';
	UltimaDataConsulta TIMESTAMP = '2019-02-28';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select c.localidade as codigo, c.data_e_hora as data_hora, c.tipo as tipo_veiculo, c.contagem, c.autuacoes, c.placas, b."qtde_fxs_f" as qtdeFaixas,
				b.velocidade, b.lat, b.lon, b.bairro
				from "contagens" c inner join "BaseRadares" b 
				on b.codigo like concat('%', c.localidade, '%')
				where c.localidade = l
				and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."FluxoVeiculosRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'FluxoVeiculosRadares Processado';
END;
$BODY$ language plpgsql

-- TipoVeiculosRadares
-- TEMPO PARA FEV 2018: 20 SEGS
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2019-02-01';
	UltimaDataConsulta TIMESTAMP = '2019-02-28';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (				
				select localidade as codigo, data_e_hora as data_hora, sum(contagem) as contagem, sum(autuacoes) as autuacoes,
				sum(placas) as placas from contagens where localidade = l and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				group by localidade, data_e_hora order by codigo, data_e_hora
			) r);
			INSERT INTO camadavisualizacao."TipoVeiculosRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'TipoVeiculosRadares Processado';
END;
$BODY$ language plpgsql

-- InfracoesRadares
-- TEMPO PARA RODAR FEV 2018: 30 SEGS
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2019-02-01';
	UltimaDataConsulta TIMESTAMP = '2019-02-28';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select data_e_hora as "dataHora", localidade as "codigoRadar", tipo as "tipoVeiculo", contagem, autuacoes, placas 
					from contagens where data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59') and localidade = l
			) r);
			INSERT INTO camadavisualizacao."InfracoesRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'InfracoesRadares Processado';
END;
$BODY$ language plpgsql

-- AcuraciaIdentificacaoRadares
-- TEMPO PARA FEV 2018: 31 SEGS
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2019-02-01';
	UltimaDataConsulta TIMESTAMP = '2019-02-28';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select localidade as "codigoRadar", data_e_hora as "dataHora", tipo as "tipoVeiculo", contagem, placas, cast(placas as decimal)/cast(contagem as decimal) as "acuraciaIdentificacao"
                	from contagens where localidade = l and data_e_hora between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."AcuraciaIdentificacaoRadares" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'AcuraciaIdentificacaoRadares Processado';
END;
$BODY$ language plpgsql

-- PerfilVelocidadesRadar
-- NOTE: ESTA BASE NÃO ESTÁ SENDO USADA, ROTA ESTÁ FUNCIONANDO COMO ESTAVA ANTES

-- Trajetos
-- TEMPO DE EXECUÇÃO PARA DIA 2018-02-02: 12 minutos
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2018-02-02';
	UltimaDataConsulta TIMESTAMP = '2018-02-02';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select t.origem as "codigoRadarOrigem",
				case 
					when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'
					when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'
					when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'
					when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'
					end as "periodoDia",
				avg((DATE_PART('day', t.data_final::timestamp - t.data_inicio::timestamp) * 24 + 
				DATE_PART('hour', t.data_final::timestamp - t.data_inicio::timestamp)) * 60 +
				DATE_PART('minute', t.data_final::timestamp - t.data_inicio::timestamp)) as "mediaMinutosTrajeto",
				avg(t.v0) as "mediaVelOrigem",
				t.destino as "codigoRadarDestino",
				avg(t.v1) as "mediaVelDestino"
				from trajetos t
				where data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				and (t.origem = l or t.destino = l)
				group by t.viagem_id, "codigoRadarOrigem", "periodoDia", "codigoRadarDestino"
			) r);
			INSERT INTO camadavisualizacao."Trajetos" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
-- 			EXIT;
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'Trajetos Processado';
END;
$BODY$ language plpgsql	

-- VelocidadeMediaTrajeto
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2018-02-01';
	UltimaDataConsulta TIMESTAMP = '2018-02-01';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select t.origem as "codigoRadarOrigem",
				case 
					when date_part('hour', t.data_inicio) between 0 and 4 then 'madrugada'
					when date_part('hour', t.data_inicio) between 5 and 12 then 'manha'
					when date_part('hour', t.data_inicio) between 13 and 18 then 'tarde'
					when date_part('hour', t.data_inicio) between 18 and 23 then 'noite'
					end as "periodoDia",
					avg((t.v0 + t.v1) / 2 ) as "velocidadeMedia",
					t.destino as "codigoRadarDestino"
				from trajetos t
				where data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
				and (t.origem = l or t.destino = l)
				group by t.viagem_id, "codigoRadarOrigem", "codigoRadarDestino", "periodoDia"
			) r);
			INSERT INTO camadavisualizacao."VelocidadeMediaTrajeto" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
-- 			EXIT;
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'VelocidadeMediaTrajeto Processado';
END;
$BODY$ language plpgsql	

-- Viagens
-- tempo p rodar dia 2019-02-01: 12 minutos
DO
$BODY$
DECLARE
	l integer;
    j json;
	DataConsulta TIMESTAMP = '2019-02-01';
	UltimaDataConsulta TIMESTAMP = '2019-02-01';
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(localidade) FROM public.contagens);
	
	WHILE DataConsulta <= UltimaDataConsulta LOOP
		FOREACH l IN ARRAY arr
		LOOP
			j := (SELECT json_agg(r)								   
			FROM (
				select id as "viagemId", inicio as "codigoRadarInicio", data_inicio as "dataHoraInicio", final as "codigoRadarFinal", data_final as "dataHoraFinal", tipo as "tipoVeiculo" 
				from viagens where (inicio = l or final = l) and data_inicio between (DataConsulta + time '00:00:00') and (DataConsulta + time '23:59:59')
			) r);
			INSERT INTO camadavisualizacao."Viagens" ("Radares","DataConsulta","JsonRetorno") VALUES (l, DataConsulta, j);
			RAISE NOTICE 'Inserção Realizada: (%)[%]', DataConsulta, l;
		END LOOP;
		DataConsulta := DataConsulta + INTERVAL '1 day';
	END LOOP;
	RAISE NOTICE 'Viagens Processado';
END;
$BODY$ language plpgsql	

-- DistanciaViagem
-- TEMPO PARA: 5 SEGUNDOS
DO
$BODY$
DECLARE
	l1 integer;
	l2 integer;
    j json;
	arr integer[];
BEGIN
	arr := ARRAY(SELECT DISTINCT(codigo) FROM public.base_radares_lat_lon);
	FOREACH l1 IN ARRAY arr LOOP
		FOREACH l2 IN ARRAY arr LOOP
			j := (SELECT json_agg(r)								   
				FROM (
					select br0.codigo as codigoRadarInicio,
					br1.codigo as codigoRadarFinal,
					ST_Distance(ST_Transform(concat('SRID=4326;POINT(', cast(br0.lat as varchar(20)), ' ', cast(br0.lon as varchar(20)), ')')::geometry, 3857),
					ST_Transform(concat('SRID=4326;POINT(', cast(br1.lat as varchar(20)), ' ', cast(br1.lon as varchar(20)), ')')::geometry, 3857)) * cosd(42.3521) as distancia
					from base_radares_lat_lon br0 inner join base_radares_lat_lon br1
					on br1.codigo = l1 where br0.codigo = l2
				) r);
			INSERT INTO camadavisualizacao."DistanciaViagem" ("RadarInicial", "RadarFinal", "JsonRetorno") VALUES (l1, l2, j);
			EXIT;
			RAISE NOTICE 'Inserção Realizada: [%],[%]', l1,l2;
		END LOOP;
	END LOOP;
	RAISE NOTICE 'DistanciaViagem Processado';
END;
$BODY$ language plpgsql	
