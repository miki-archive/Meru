using Npgsql;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Data;

namespace Meru.Database
{
	public class DbClient
	{
		// TODO: remove somehow.
		private static DbClient client = null;

		public static ICacheClient Cache => client.cache;
		private ICacheClient cache;

		NpgsqlConnectionStringBuilder database = null;
		ISerializer serializer = null;


		public DbClient(DbClientConfiguration config)
		{
			serializer = config.serializer;

			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config.redisAddress);
			cache = new StackExchangeRedisCacheClient(redis, serializer);

			database = new NpgsqlConnectionStringBuilder(config.postgresAddress);

			if(client == null)
			{
				client = this;
			}
		}

		/// <summary>
		/// Creates a new connection to the database
		/// </summary>
		public IDbConnection CreateNew()
		{
			// TODO: abstractify this
			return new NpgsqlConnection(database.ConnectionString);
		}

		/// <summary>
		/// Cheap version for a global create database conn.
		/// </summary>
		/// <returns></returns>
		public static IDbConnection Create()
		{
			if (client == null)
				return null;

			return client.CreateNew();
		}
	}

	// TODO: abstractify this
	public class DbClientConfiguration
	{
		public string redisAddress = "127.0.0.1";
		public string postgresAddress = "Server=127.0.0.1;";

		[NonSerialized]
		public ISerializer serializer = null;
	}

	public class DbConfiguration
	{
		public string HostName;
		public string Username;
		public string Password;
		public int Port;
	}
}
