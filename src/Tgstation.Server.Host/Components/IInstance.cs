﻿using Microsoft.Extensions.Hosting;

namespace Tgstation.Server.Host.Components
{
	interface IInstance : IHostedService
	{
		IRepositoryManager RepositoryManager { get; }

		IByond Byond { get; }

		IDreamMaker DreamMaker { get; }

		IDreamDaemon DreamDaemon { get; }

		IChat Chat { get; }

		IConfiguration Configuration { get; }

		Api.Models.Instance GetMetadata();

		void Rename(string newName);
	}
}