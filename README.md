# LogTransfer

Sistema para transferência e ingestão de logs Android em alta performance usando TCP sockets e SQL Server.

O projeto foi otimizado para processar **arquivos de log muito grandes**, reduzindo drasticamente o tempo total de ingestão.

---

## Estrutura do Projeto

### LogTransfer.Core
Projeto compartilhado entre client e server.
- `LogEntry`: modelo do log
- `SocketProtocol`: definições do protocolo (porta, encoding, batch size)

---

### LogTransfer.Client
Responsável por ler os arquivos de log e enviá-los ao servidor.
- Lê arquivos grandes em streaming
- Envia linha a linha via TCP

Arquivos principais:
- `FileScanner`
- `LogSender`
- `Program.cs`

Execução:
'''LogTransfer.Client <logPath> <serverIp> [serverPort]'''


---

### LogTransfer.Server
Responsável por receber, processar e persistir os logs.
- Aceita conexões TCP
- Faz parsing otimizado das linhas
- Insere dados no banco em lote

Arquivos principais:
- `LogSocketHandler`
- `LogParser`
- `LogRepository`
- `DatabaseContext`

---

## Parsing e Performance

Inicialmente o parser utilizava `string.Split`, o que gerava muitas alocações e impactava a performance.

O parser foi otimizado para:
- Identificar campos por **índices**
- Evitar alocações desnecessárias
- Reduzir pressão no GC

Resultado prático:
- Tempo total caiu de ~12 minutos para ~2 minutos
- Ganho significativo apenas com a mudança no parser

---

## Banco de Dados

- SQL Server / SQL Express
- Inserção usando `SqlBulkCopy`
- Inserções feitas em batches configuráveis

---

## Execução

1. Inicie o projeto `LogTransfer.Server`
2. Execute o `LogTransfer.Client` passando o caminho da pasta de logs
3. Acompanhe o progresso pelo console do servidor

---

## Observação

O projeto foi desenvolvido com foco em **performance, simplicidade e clareza de arquitetura**.