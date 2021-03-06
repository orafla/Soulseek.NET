﻿// <copyright file="IDistributedConnectionManager.cs" company="JP Dillingham">
//     Copyright (c) JP Dillingham. All rights reserved.
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see https://www.gnu.org/licenses/.
// </copyright>

namespace Soulseek.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Soulseek.Diagnostics;
    using Soulseek.Messaging.Messages;
    using Soulseek.Network.Tcp;

    /// <summary>
    ///     Manages distributed <see cref="IMessageConnection"/> instances for the application.
    /// </summary>
    internal interface IDistributedConnectionManager : IDisposable, IDiagnosticGenerator
    {
        /// <summary>
        ///     Gets the current distributed branch level.
        /// </summary>
        int BranchLevel { get; }

        /// <summary>
        ///     Gets the current distributed branch root.
        /// </summary>
        string BranchRoot { get; }

        /// <summary>
        ///     Gets a value indicating whether child connections can be accepted.
        /// </summary>
        bool CanAcceptChildren { get; }

        /// <summary>
        ///     Gets the current list of child connections.
        /// </summary>
        IReadOnlyCollection<(string Username, IPEndPoint IPEndPoint)> Children { get; }

        /// <summary>
        ///     Gets the number of allowed concurrent child connections.
        /// </summary>
        int ChildLimit { get; }

        /// <summary>
        ///     Gets a value indicating whether a parent connection is established.
        /// </summary>
        bool HasParent { get; }

        /// <summary>
        ///     Gets the current parent connection.
        /// </summary>
        (string Username, IPEndPoint IPEndPoint) Parent { get; }

        /// <summary>
        ///     Gets a dictionary containing the pending connection solicitations.
        /// </summary>
        IReadOnlyDictionary<int, string> PendingSolicitations { get; }

        /// <summary>
        ///     Adds a new child connection using the details in the specified <paramref name="connectToPeerResponse"/> and
        ///     pierces the remote peer's firewall.
        /// </summary>
        /// <param name="connectToPeerResponse">The response that solicited the connection.</param>
        /// <returns>The operation context.</returns>
        Task AddChildConnectionAsync(ConnectToPeerResponse connectToPeerResponse);

        /// <summary>
        ///     Adds a new child connection from an incoming connection.
        /// </summary>
        /// <param name="username">The username from which the connection originated.</param>
        /// <param name="incomingConnection">The accepted connection.</param>
        /// <returns>The operation context.</returns>
        Task AddChildConnectionAsync(string username, IConnection incomingConnection);

        /// <summary>
        ///     Asynchronously connects to one of the specified <paramref name="parentCandidates"/>.
        /// </summary>
        /// <param name="parentCandidates">The list of parent connection candidates provided by the server.</param>
        /// <returns>The operation context.</returns>
        Task AddParentConnectionAsync(IEnumerable<(string Username, IPEndPoint IPEndPoint)> parentCandidates);

        /// <summary>
        ///     Asynchronously writes the specified bytes to each of the connected child connections.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The operation context.</returns>
        Task BroadcastMessageAsync(byte[] bytes, CancellationToken? cancellationToken = null);

        /// <summary>
        ///     Removes and disposes the parent and all child connections.
        /// </summary>
        void RemoveAndDisposeAll();

        /// <summary>
        ///     Sets the distributed <paramref name="branchLevel"/>.
        /// </summary>
        /// <param name="branchLevel">The distributed branch level.</param>
        void SetBranchLevel(int branchLevel);

        /// <summary>
        ///     Sets the distributed <paramref name="branchRoot"/>.
        /// </summary>
        /// <param name="branchRoot">The distributed branch root.</param>
        void SetBranchRoot(string branchRoot);
    }
}