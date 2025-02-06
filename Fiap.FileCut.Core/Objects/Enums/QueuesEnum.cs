using System.ComponentModel;
using Fiap.FileCut.Core.Attributes;

namespace Fiap.FileCut.Core.Objects.Enums;

public enum Queues
{
    [Description("Fila de Informação")]
    [MessageQueueName("FIAP-FILECUT-INFORMATION-QUEUE")]
    INFORMATION = 1,

    [Description("Fila de Processamento")]
    [MessageQueueName("FIAP-FILECUT-PROCESS-QUEUE")]
    PROCESS = 2,

    [Description("Fila de Empacotamento")]
    [MessageQueueName("FIAP-FILECUT-PACK-QUEUE")]
    PACK = 3,
}