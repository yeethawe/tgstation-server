﻿using System;
using Tgstation.Server.Api.Models;
using Tgstation.Server.Host.Components.Deployment;
using Tgstation.Server.Host.Components.Interop.Bridge;
using Tgstation.Server.Host.Models;
using Tgstation.Server.Host.System;

namespace Tgstation.Server.Host.Components.Watchdog
{
	/// <summary>
	/// Parameters necessary for duplicating a <see cref="ISessionController"/> session
	/// </summary>
	public sealed class ReattachInformation : ReattachInformationBase
	{
		/// <summary>
		/// The <see cref="IDmbProvider"/> used by DreamDaemon
		/// </summary>
		public IDmbProvider Dmb { get; set; }

		/// <summary>
		/// The <see cref="Interop.Bridge.RuntimeInformation"/> for the DMAPI.
		/// </summary>
		public RuntimeInformation RuntimeInformation { get; private set; }

		/// <inheritdoc />
		public override DreamDaemonSecurity? LaunchSecurityLevel
		{
			get => RuntimeInformation.SecurityLevel ?? base.LaunchSecurityLevel;
			set => throw new NotSupportedException();
		}

		/// <summary>
		/// <see langword="lock"/> <see cref="object"/> for accessing <see cref="RuntimeInformation"/>.
		/// </summary>
		readonly object runtimeInformationLock;

		/// <summary>
		/// Initializes a new isntance of the <see cref="ReattachInformation"/> <see langword="class"/>.
		/// </summary>
		/// <param name="dmb">The value of <see cref="Dmb"/>.</param>
		/// <param name="process">The <see cref="IProcess"/> used to get the <see cref="ReattachInformationBase.ProcessId"/>.</param>
		/// <param name="runtimeInformation">The value of <see cref="RuntimeInformation"/>.</param>
		/// <param name="port">The value of <see cref="ReattachInformationBase.Port"/>.</param>
		/// <param name="isPrimary">The value of <see cref="ReattachInformationBase.IsPrimary"/>.</param>
		internal ReattachInformation(
			IDmbProvider dmb,
			IProcess process,
			RuntimeInformation runtimeInformation,
			ushort port,
			bool isPrimary)
		{
			Dmb = dmb ?? throw new ArgumentNullException(nameof(dmb));
			ProcessId = process?.Id ?? throw new ArgumentNullException(nameof(process));
			RuntimeInformation = runtimeInformation ?? throw new ArgumentNullException(nameof(runtimeInformation));
			if (!runtimeInformation.SecurityLevel.HasValue)
				throw new ArgumentException("runtimeInformation must have a valid SecurityLevel!", nameof(runtimeInformation));

			base.LaunchSecurityLevel = runtimeInformation.SecurityLevel.Value;
			Port = port;
			IsPrimary = isPrimary;

			runtimeInformationLock = new object();
		}

		/// <summary>
		/// Construct a <see cref="ReattachInformation"/> from a given <paramref name="copy"/> and <paramref name="dmb"/>
		/// </summary>
		/// <param name="copy">The <see cref="Models.ReattachInformation"/> to copy values from</param>
		/// <param name="dmb">The value of <see cref="Dmb"/></param>
		public ReattachInformation(Models.ReattachInformation copy, IDmbProvider dmb) : base(copy)
		{
			Dmb = dmb ?? throw new ArgumentNullException(nameof(dmb));

			runtimeInformationLock = new object();
		}

		/// <summary>
		/// Set the <see cref="RuntimeInformation"/> post construction.
		/// </summary>
		/// <param name="runtimeInformation">The <see cref="Interop.Bridge.RuntimeInformation"/>.</param>
		public void SetRuntimeInformation(RuntimeInformation runtimeInformation)
		{
			if (runtimeInformation == null)
				throw new ArgumentNullException(nameof(runtimeInformation));

			lock (runtimeInformationLock)
			{
				if (RuntimeInformation != null)
					throw new InvalidOperationException("RuntimeInformation already set!");

				RuntimeInformation = runtimeInformation;
			}
		}
	}
}