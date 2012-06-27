using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Palo.ChatLibrary.UserManagements
{
    public class LoggedUser
    {
        #region Properties

        /// <summary>
        /// Unikatni cislo uzivatele.
        /// </summary>
        public Guid Id { private set; get; }

        /// <summary>
        /// Nick name uzivatele.
        /// </summary>
        public string DisplayName { private set; get; }

        /// <summary>
        /// Socket, na kterym visi.
        /// </summary>
        [JsonIgnore]
        public Socket Sock { private set; get; }

        /// <summary>
        /// Prazdny uzivatel.
        /// </summary>
        [JsonIgnore]
        public static LoggedUser Empty 
        {
            get { return new LoggedUser(Guid.Empty, null, null); }
        }

        /// <summary>
        /// Je uzivatel prazdny.
        /// </summary>
        /// <returns>true</returns>
        [JsonIgnore]
        public bool IsEmpty
        {
            get { return Id == Guid.Empty; }
        }

        #endregion //  Properties

        #region Constructors

        /// <summary>
        /// Privatni konstruktor, pro potreby deserializace pomoci JSON knihovny.
        /// </summary>
        /// <param name="id">id uzivatele</param>
        /// <param name="displayName">Jmeno</param>
        /// <param name="sock">socket</param>
        [JsonConstructor]
        private LoggedUser(Guid id, string displayName, Socket sock)
        {
            Id = id;
            DisplayName = displayName;
            Sock = sock;
        }

        /// <summary>
        /// Konstruktor, instancujici objekt.
        /// </summary>
        /// <param name="displayName">nickname uzivatele</param>
        /// <param name="sock">socket, na kterym se uzivatel nachazi</param>
        public LoggedUser(string displayName, Socket sock)
        {
            Id = Guid.NewGuid();
            Sock = sock;
            DisplayName = displayName;
        }

        #endregion // Constructors

        #region Overrided methods

        /// <summary>
        /// Porovnani dvou objektu.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true, pokud jsou stejny</returns>
        public override bool Equals(object obj)
        {
            if (obj is LoggedUser)
                return Id == ((LoggedUser)obj).Id;
            else
                return false;
        }

        /// <summary>
        /// Prepsany hashCode kvuli porovnani v HashSetu.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int tmp = 0;
            if (Id != null) tmp += Id.GetHashCode();
            if (DisplayName != null) tmp += DisplayName.GetHashCode();

            return tmp;
        }

        #endregion
    }
}
