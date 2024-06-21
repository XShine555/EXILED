// -----------------------------------------------------------------------
// <copyright file="ChangingAhpEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using static PlayerStatsSystem.AhpStat;

    /// <summary>
    /// Contains all information before a player's <see cref="RoleTypeId" /> changes.
    /// </summary>
    public class ChangingAhpEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAhpEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="ahpProcess">
        /// <inheritdoc cref="AhpProcess" />
        /// </param>
        public ChangingAhpEventArgs(Player player, AhpProcess ahpProcess)
        {
            Player = player;
            AhpProcess = ahpProcess;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets the current instance of the <see cref="PlayerStatsSystem.AhpStat.AhpProcess" />.
        /// </summary>
        public AhpProcess AhpProcess { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}