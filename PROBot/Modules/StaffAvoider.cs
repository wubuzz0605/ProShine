﻿using PROProtocol;
using System;

namespace PROBot.Modules
{
    public class StaffAvoider
    {
        public event Action<bool> StateChanged;

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    StateChanged?.Invoke(value);
                }
            }
        }

        private BotClient _bot;

        public StaffAvoider(BotClient bot)
        {
            _bot = bot;
            _bot.ClientChanged += Bot_ClientChanged;
        }

        private void Bot_ClientChanged()
        {
            if (_bot.Game != null)
            {
                _bot.Game.PlayerAdded += Game_PlayerAdded;
                _bot.Game.PlayerUpdated += Game_PlayerUpdated;
            }
        }

        private void Game_PlayerAdded(PlayerInfos player)
        {
            if (player.Name.Contains("["))
            {
                int distance = _bot.Game.DistanceTo(player.PosX, player.PosY);
                string message = player.Name + " appears on the map at " + distance + " cells from you";
                if (player.IsAfk)
                {
                    message += ", but is AFK";
                }
                else if (IsEnabled)
                {
                    _bot.Logout(false);
                    message += ", disconnecting";
                }
                _bot.LogMessage(message + ".");
            }
        }

        private void Game_PlayerUpdated(PlayerInfos player)
        {
            if (player.Name.Contains("["))
            {
                int distance = _bot.Game.DistanceTo(player.PosX, player.PosY);
                if (!player.IsAfk && IsEnabled)
                {
                    _bot.LogMessage(player.Name + " is at " + distance + " cells from you and is not AFK, disconnecting.");
                    _bot.Logout(false);
                }
            }
        }
    }
}
