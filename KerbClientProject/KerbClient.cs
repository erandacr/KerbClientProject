using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Net.Http;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;



namespace KerbClientProject
{
    class KerbClient
    {
        // Name of the serivce we are trying to invoke
        static string serviceName = "HTTP/apimserver.example.com@EXAMPLE.COM";
        // API parameters
        static string URI = "https://idp.example.com:9443/oauth2/token";
        static string username = "1ouL2fO6SxlfD2LDw125cTo0vQka";
        static string password = "uUEtm89tY6QZuZUmqZfL92BDFeAa";

        static string realm_Name = "example.com";

        static void Main()
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);
            var domain = Domain.GetCurrentDomain().ToString();

            using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
            {
                //string spn = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, serviceName).UserPrincipalName;
                KerberosSecurityTokenProvider tokenProvider = new KerberosSecurityTokenProvider(serviceName, System.Security.Principal.TokenImpersonationLevel.Identification, CredentialCache.DefaultNetworkCredentials);
                KerberosRequestorSecurityToken securityToken = tokenProvider.GetToken(TimeSpan.FromMinutes(5)) as KerberosRequestorSecurityToken;
                string serviceToken = Convert.ToBase64String(securityToken.GetRequest());
                string encodedToken = HttpUtility.UrlEncode(serviceToken);
                Console.WriteLine("Response:  " + HttpPostq(buildRequest(encodedToken)));                 
            }
        }

        static string buildRequest(string token)
        {

            return "grant_type=kerberos&kerberos_realm=" + realm_Name + "&kerberos_token=" + token;
        }

        static string HttpPostq(string Parameters)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);

            // Add these, as we're doing a POST
            setBasicAuthHeader(req, username, password);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; 

            // We need to count how many bytes we're sending
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();

            // Retriving the response
            System.Net.WebResponse resp = null;
            try
            {
                resp = req.GetResponse();
            } catch (System.Net.WebException ex) {
                Console.WriteLine("Error in retriving the OAuth2 token: " + ex);
            }
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        static void setBasicAuthHeader(WebRequest request, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
        }

    }
}
