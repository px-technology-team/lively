﻿using System;

namespace Lively.Services
{
    public interface IRunnerService : IDisposable
    {
        void ShowUI();
        void CloseUI();
        void ShowControlPanel();
        void ShowCustomisWallpaperePanel();
        bool IsVisibleUI { get; }
    }
}