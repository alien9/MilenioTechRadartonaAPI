# Manuten��o do Banco de Dados

O banco de dados possui arquitetura utilizando 2 schemas(**public** e **camadavisualizacao**), na camada **public** est�o as tabelas de funcionamento da API e tamb�m as bases fornecidas durante a radartona, as mesmas s�o consumidas pelo Banco de dados para pr� processar os dados a partir das tabelas baseRadares, contagens, trajetos e viagens.

Essas tabelas passam por um pr� processamento, desse modo, os dados s�o transformados em json a partir da data da consulta e do id do radar, esse pr� processamento � realizado por PROCEDURES SQL no banco de dados, assim, s�o gerados as tabelas no schema **camadavisualizacao** que ser�o consumidos pela API na hora de apresentar dados para o usu�rio

Ap�s a inser��o de dados nas tabelas � necess�rio rodar as seguintes procedures para que a **camada de visualiza��o** seja atualizada. As procedures s�o baseadas na data em que os dados de backup foram inseridos.

### Tabela contagens

```sql
-- DataInicial e DataFinal s�o dados do tipo 'yyyy-MM-dd'
InsereFluxoVeiculosRadares(DataInicial, DataFinal)
InsereTipoVeiculosRadares(DataInicial, DataFinal)
InsereInfracoesRadares(DataInicial, DataFinal)
InsereAcuraciaIdentificacaoRadares(DataInicial, DataFinal)
```

### Tabela Trajetos

```sql
-- DataInicial e DataFinal s�o dados do tipo 'yyyy-MM-dd'
InsereTrajetos(DataInicial, DataFinal)
InsereVelocidadeMediaTrajeto(DataInicial, DataFinal)
```

### Tabela Viagens

```sql
-- DataInicial e DataFinal s�o dados do tipo 'yyyy-MM-dd'
InsereViagens(DataInicial, DataFinal)
```

# Tabela BaseRadares

Adicionar radares normalizados por c�digo com registros equivalentes na tabela **base_radares_lat_lon**