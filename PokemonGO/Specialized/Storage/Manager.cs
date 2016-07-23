using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Storage
{
    public class Manager
    {
        private string Source;

        public Manager(string Source)
        {
            Init(Source);
        }

        private void Init(string Source)
        {
            this.Source = Source;

            if (!File.Exists(Source))
            {
                Setup();
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(string.Format("data source={0}", Source));
        }

        private void Setup()
        {
            try
            {
                SQLiteConnection.CreateFile(Source);

                using (var Connection = GetConnection())
                {
                    Connection.Open();

                    using (var Command = new SQLiteCommand(Connection))
                    {
                        Command.CommandText = "CREATE TABLE IF NOT EXISTS Pokemons (ID TEXT NOT NULL PRIMARY KEY, Pokemon INTEGER NOT NULL, Latitude NUMERIC NOT NULL, Longitude NUMERIC NOT NULL, Expiration TEXT NOT NULL, Spawn TEXT NOT NULL, Seen TEXT NOT NULL)";

                        Command.ExecuteNonQuery();
                    }

                    Connection.Close();
                }
            }
            catch { }
        }

        internal void Insert(MapPokemon Pokemon)
        {
            try
            {
                using (var Connection = GetConnection())
                {
                    Connection.Open();

                    using (var Command = new SQLiteCommand(Connection))
                    {
                        Command.CommandText = string.Format("INSERT OR IGNORE INTO Pokemons (ID, Pokemon, Latitude, Longitude, Expiration, Spawn, Seen) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', datetime('now', 'localtime'))", Pokemon.EncounterId, (int)Pokemon.PokemonId, Pokemon.Latitude, Pokemon.Longitude, Pokemon.ExpirationTimestampMs, Pokemon.SpawnpointId);
                        
                        Command.ExecuteNonQuery();
                    }

                    Connection.Close();
                }
            }
            catch (Exception e)
            { }
        }
    }
}
