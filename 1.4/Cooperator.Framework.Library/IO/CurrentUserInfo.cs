using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Text;

namespace Cooperator.Framework.Library.IO
{
    /// <summary>
    /// 
    /// </summary>
    public static class CurrentUserInfo
    {
        //Defines the .dll file to import as well as the method to use.

        [DllImport("Advapi32.dll", EntryPoint = "GetUserName", ExactSpelling = false, SetLastError = true)]
        //This specifies the exact method we are going to call from within our imported .dll
        private static extern bool _getUserName(
            [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
            [MarshalAs(UnmanagedType.LPArray)] Int32[] nSize);

        /// <summary>
        /// This method is used to Get Current User Name in Win9x
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            byte[] str = new byte[256];
            Int32[] len = new Int32[1];
            len[0] = 256;
            _getUserName(str, len);
            string name = System.Text.Encoding.ASCII.GetString(str);
            return name.Substring(0, name.IndexOf(Convert.ToChar(0)));
        }



        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="domainName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public static WindowsIdentity GetImpersonatedUser(string userName, string domainName, string password)
        {

            const int LOGON32_PROVIDER_DEFAULT = 0;
            //This parameter causes LogonUser to create a primary token.
            const int LOGON32_LOGON_INTERACTIVE = 2;

            IntPtr tokenHandle = new IntPtr(0);

            try
            {

                tokenHandle = IntPtr.Zero;

                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    ref tokenHandle);

                if (false == returnValue)
                {
                    return null;
                }

            }
            catch 
            {
                return null;
            }

            return new WindowsIdentity(tokenHandle);
        }

    }
}
