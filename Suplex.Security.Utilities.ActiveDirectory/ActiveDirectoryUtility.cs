using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Suplex.Security.Utilities.ActiveDirectory
{
    public class ActiveDirectoryUtility
    {
        public static List<string> GetGroupMembershipRecursive(string userName, string ldapRoot, SecureString authUser = null, SecureString authPassword = null)
        {
            //todo: this is a stub implementation, need to accept sAMAccountName or DN, uee alt AD filter
            return GetGroupMembershipSimple( userName, ldapRoot, authUser, authPassword );
        }

        public static List<string> GetGroupMembershipSimple(string userName, string ldapRoot, SecureString authUser = null, SecureString authPassword = null)
        {
            if( string.IsNullOrWhiteSpace( userName ) )
                throw new ArgumentNullException( nameof( userName ) );
            if( string.IsNullOrWhiteSpace( userName ) )
                throw new ArgumentNullException( nameof( ldapRoot ) );


            List<string> result = new List<string>();

            string[] user = userName.Split( '\\' );
            string sAMAccountName = user.Length > 1 ? user[1] : user[0];

            DirectoryEntry root = new DirectoryEntry( ldapRoot );
            if( authUser != null && authPassword != null )
            {
                root.Username = authUser.ToUnsecureString();
                root.Password = authPassword.ToUnsecureString();
            }
            DirectorySearcher groups = new DirectorySearcher( root );
            groups.Filter = $"sAMAccountName={sAMAccountName}";
            groups.PropertiesToLoad.Add( "memberOf" );

            SearchResult sr = groups.FindOne();

            if( sr != null )
                for( int i = 0; i <= sr.Properties["memberOf"].Count - 1; i++ )
                {
                    string group = sr.Properties["memberOf"][i].ToString();
                    result.Add( group.Split( ',' )[0].Replace( "CN=", "" ) );
                }

            return result;
        }
    }
}