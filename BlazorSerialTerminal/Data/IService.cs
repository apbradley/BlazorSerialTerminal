using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlazorSerialTerminal.Data
{
    public interface IService
    {
        int Counter { get; set; }
        ILogger Logger { get; }
    }
}
