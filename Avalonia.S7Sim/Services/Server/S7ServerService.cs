using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Models;
using Avalonia.S7Sim.Services;
using Avalonia.S7Sim.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services;

public class S7ServerService : IDisposable, IS7ServerService
{
    private readonly IList<RunningServerItem> _runningItems = [];
    //private readonly MsgLoggerVM _loggerVM;
    private readonly ILogger<S7ServerService> _logger;
    protected virtual IMediator _mediator { get; set; }
    protected virtual FutureTech.Snap7.S7Server S7Server { get; set; }

    public S7ServerService(IMediator mediator, ILogger<S7ServerService> logger)
    {
        this._mediator = mediator;
        //this._runningVM = runningVM;
        //this._loggerVM = loggerVM;
        this._logger = logger;
    }


    public async Task StartServerAsync(IPAddress? address, AreaConfig[] areaConfigs)
    {
        try
        {
            this.S7Server = new FutureTech.Snap7.S7Server();

            if (areaConfigs == null || areaConfigs.Length == 0)
            {
                var msg = new MessageNotification() { Message = "当前 DBConfigs 为 NULL !" };
                await this._mediator.Publish(msg);
                return;
            }

            _runningItems.Clear();

            foreach (var area in areaConfigs)
            {
                var buffer = new byte[area.BlockSize];
                _runningItems.Add(new RunningServerItem
                {
                    AreaKind = area.AreaKind,
                    BlockNumber = area.BlockNumber,
                    BlockSize = area.BlockSize,
                    Bytes = buffer,
                });
                switch (area.AreaKind)
                {
                    case AreaKind.DB:
                        this.S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaDB, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    case AreaKind.MB:
                        this.S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaMK, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    default:
                        throw new NotImplementedException($"未知的区域类型={area}");
                }
            }
            this.S7Server.StartTo(address?.ToString() ?? "127.0.0.1");

            //this._loggerVM.AddLogMsg(new LogMessage(DateTime.Now, LogLevel.Information, "[+]服务启动..."));
        }
        catch (Exception ex)
        {
            var msg = $"启动服务器出错：{ex.Message}";
            this._logger.LogError(msg);
            await this._mediator.Publish(new MessageNotification
            {
                Message = msg
            });
        }
    }

    public async Task StopServerAsync()
    {
        try
        {
            this.S7Server?.Stop();
            this.S7Server = null;
            //this._loggerVM.AddLogMsg(new LogMessage(DateTime.Now, LogLevel.Information, "[!]服务停止..."));
        }
        catch (Exception ex)
        {
            var msg = $"停止服务器出错：{ex.Message}";
            this._logger.LogError(msg);
            await this._mediator.Publish(new MessageNotification { Message = msg });
        }
    }

    public void Dispose()
    {
        _ = this.StopServerAsync();
    }

    public Task StartServerAsync()
    {
        throw new NotImplementedException();
    }
}