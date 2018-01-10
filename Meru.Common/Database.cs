using Npgsql;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using System;
using System.Data;

namespace Meru.Database
{
	public class DbClient
	{
		private static DbClient _client = null;

		public ICacheClient cache = null;
		NpgsqlConnectionStringBuilder database = null;
		public ISerializer serializer = null;

		public DbClient(DbClientConfiguration config)
		{
			serializer = config.serializer;

			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(config.redisAddress);
			cache = new StackExchangeRedisCacheClient(redis, serializer);

			database = new NpgsqlConnectionStringBuilder(config.postgresAddress);

			if(_client == null)
			{
				_client = this;
			}
		}

		public IDbConnection CreateNew()
		{
			return new NpgsqlConnection(database.ConnectionString);
		}

		/// <summary>
		/// Cheap version for a global create database conn.
		/// </summary>
		/// <returns></returns>
		public static IDbConnection Create()
		{
			if (_client == null)
				return null;

			return _client.CreateNew();
		}
		public static ICacheClient Cache => _client.cache;
	}

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
