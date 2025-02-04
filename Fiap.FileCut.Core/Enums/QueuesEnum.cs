using System.ComponentModel;
using Fiap.FileCut.Core.Attributes;

namespace Fiap.FileCut.Core.Enums;

public enum Queues
{
    [Description("Fila de Informação")]
    [MessageQueueNameAttribute("FIAP-FILECUT-INFORMATION-QUEUE")]
    INFORMATION = 1,

    [Description("Fila de Processamento")]
    [MessageQueueNameAttribute("FIAP-FILECUT-PROCESS-QUEUE")]
    PROCESS = 2,

    [Description("Fila de Empacotamento")]
    [MessageQueueNameAttribute("FIAP-FILECUT-PACK-QUEUE")]
    PACK = 3,
}