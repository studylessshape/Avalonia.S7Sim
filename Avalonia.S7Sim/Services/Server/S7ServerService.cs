using Avalonia.S7Sim.Messages;
using Avalonia.S7Sim.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia.S7Sim.Services;

public class S7ServerService : IDisposable, IS7ServerService
{
    private readonly IList<RunningServerItem> _runningItems = [];
    private readonly ILogger<S7ServerService> _logger;

    public IList<RunningServerItem> RunningItems => _runningItems;

    protected virtual FutureTech.Snap7.S7Server? S7Server { get; set; }

    public S7ServerService(ILogger<S7ServerService> logger)
    {
        this._logger = logger;
    }

    public Task<bool> StartServerAsync(IPAddress? address, IEnumerable<AreaConfig> areaConfigs)
    {
        try
        {
            S7Server = new FutureTech.Snap7.S7Server();

            if (areaConfigs == null || areaConfigs.Count() == 0)
            {
                MessageHelper.ShowMessage("当前 DBConfigs 为 NULL !");
                return Task.FromResult(false);
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
                        S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaDB, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    case AreaKind.MB:
                        S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaMK, area.BlockNumber, ref buffer, area.BlockSize);
                        break;
                    default:
                        throw new NotImplementedException($"未知的区域类型={area}");
                }
            }
            S7Server.StartTo(address?.ToString() ?? "127.0.0.1");
            MessageHelper.SendLogMessage(new LogMessage { Message = "[+]服务启动..." });

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            var msg = $"启动服务器出错：{ex.Message}";
            _logger.LogError(msg);
            MessageHelper.ShowMessage(msg);
            return Task.FromResult(false);
        }
    }

    public Task<bool> StopServerAsync()
    {
        try
        {
            S7Server?.Stop();
            S7Server = null;
            MessageHelper.SendLogMessage(new LogMessage() { Message = "[!]服务停止...", Level = Controls.Notifications.NotificationType.Warning });
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            var msg = $"停止服务器出错：{ex.Message}";
            _logger.LogError(msg);
            MessageHelper.ShowMessage(msg);
            return Task.FromResult(false);
        }
    }

    public void Dispose()
    {
        _ = this.StopServerAsync();
    }
}