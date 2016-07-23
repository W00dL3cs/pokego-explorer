using AllEnum;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGO.Specialized.Protocol
{
    internal class Settings : ISettings
    {
        private string Username;
        private string Password;

        private double Latitude;
        private double Longitude;

        internal Settings(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }

        internal void SetDetails(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
        
        public AuthType AuthType
        {
            get
            {
                return AuthType.Ptc;
            }
        }

        public double DefaultAltitude
        {
            get
            {
                return 0;
            }
        }

        public double DefaultLatitude
        {
            get
            {
                return Latitude;
            }
        }

        public double DefaultLongitude
        {
            get
            {
                return Longitude;
            }
        }

        public bool EvolveAllPokemonWithEnoughCandy
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GoogleRefreshToken
        {
            get
            {
                return null;
            }

            set
            {
                //throw new NotImplementedException();
            }
        }

        public ICollection<KeyValuePair<ItemId, int>> ItemRecycleFilter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int KeepMinCP
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float KeepMinIVPercentage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<PokemonId> PokemonsNotToTransfer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<PokemonId> PokemonsToEvolve
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PtcPassword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PtcUsername
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool TransferDuplicatePokemon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double WalkingSpeedInKilometerPerHour
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}