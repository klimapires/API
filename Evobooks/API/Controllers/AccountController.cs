
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using API.Models;
using API.Providers;
using API.Results;
using DTO.User;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using FBXStructure.Specific;
using System.Runtime.Serialization.Formatters.Binary;
using FBXStructure.Basic;
using System.IO;
using System.Linq;
using FBXParser;
using System.Web.Http.Cors;
using System.Net.Mail;

namespace API.Controllers
{
    [Authorize]
    [RoutePrefix("Account")]

    public class AccountController : ApiController
    {
        DBContext db = new DBContext();
        #region Variables
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }
        #endregion

        #region Methods
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        // POST Account/Login
        /// <summary>
        /// Faz login no sistema e retorna, entre outros dados, um acces token, nome de usuário e dada de expiração do token.
        /// Incluir token no cabeçalho de Requests, key: Authorize, value: 'Bearer ' + token.
        /// </summary>
        /// <param name="login">Dados do usuário que está logando.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Login")]
        public async Task<HttpResponseMessage> Login(UserLogin login)
        {
            if (!ModelState.IsValid)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var request = HttpContext.Current.Request;
            var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", login.Username),
                    new KeyValuePair<string, string>("password", login.Password)
                };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var jsString = await tokenServiceResponse.Content.ReadAsStringAsync();

                dynamic ccx = JsonConvert.DeserializeObject(jsString);
                string responseString = "";
                foreach (var it in ((Newtonsoft.Json.Linq.JObject)ccx))
                {
                    responseString += (responseString == "" ? "" : ", ") + "\"" + it.Key + "\":\"" + it.Value + "\"";
                }
                responseString = "{" + responseString + "}";
                var responseCode = tokenServiceResponse.StatusCode;
                var responseMsg = new HttpResponseMessage(responseCode)
                {
                    Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                };
                if (responseMsg.IsSuccessStatusCode)
                {
                    ApplicationUser user = await UserManager.FindByNameAsync(login.Username);
                    bool hasRegistered = user != null;
                    if (hasRegistered)
                    {
                        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                        ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                           OAuthDefaults.AuthenticationType);
                        ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                            CookieAuthenticationDefaults.AuthenticationType);


                        AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                        Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
                    }
                }
                return responseMsg;
            }
        }





        /// <summary>
        /// Muda a senha de um usuário logado.
        /// </summary>
        /// <param name="passInfo"> Informação sobre a senha</param>
        /// <returns></returns>
        // POST Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePassword passInfo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), passInfo.OldPassword,
                passInfo.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        [Route("ForgetPassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ForgetPassword(string email)
        {

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Forneça um e-mail");

            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("O usuário não existe");

            string newPassword = GenerateNewPassword();



            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            UserManager.RemovePassword(user.Id);
            IdentityResult result = await UserManager.AddPasswordAsync(user.Id, newPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("recoveryii.evobooks@outlook.com", "Evobooks");
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage("recoveryii.evobooks@outlook.com", email.Trim());
                mail.Subject = "Mudança de senha - Evobooks";
                mail.Body = @"Olá,

                Sua senha foi modificada com sucesso. Utilize a seguinte senha para logar apartir de agora:

                " + newPassword + @"

                Atenciosamente,

                Equipe Evobooks";

                client.Send(mail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }


        private string GenerateNewPassword()
        {
            string password = "Aa!";
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                    password += System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)r.Next(48, 57) });
                else
                    password += System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)r.Next(65, 90) });

            }
            return password;

        }

        /// <summary>
        /// Cadastra um novo usuário 
        /// </summary>
        /// <param name="data">Dados do usuário</param>
        /// <returns></returns>
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(User data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (db.Users.Any(usr => usr.Email == data.Email || usr.UserName == data.Username))
                return BadRequest("Já existe um usuário com esse e-mail ou senha.");


            var user = new ApplicationUser() { UserName = data.Username, Email = data.Email };
            IdentityResult result = await UserManager.CreateAsync(user, data.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("UserData")]
        public async Task<IHttpActionResult> AttUserData(UserInfo data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return Unauthorized();

            if (!string.IsNullOrWhiteSpace(data.City))
                user.City = data.City;
            if (!string.IsNullOrWhiteSpace(data.Country))
                user.Country = data.Country;
            if (!string.IsNullOrWhiteSpace(data.Phone))
                user.PhoneNumber = data.Phone;
            if (!string.IsNullOrWhiteSpace(data.StudyInstitution))
                user.FormationInstitution = data.StudyInstitution;
            if (!string.IsNullOrWhiteSpace(data.Course))
                user.FormationCourse = data.Course;
            if (!string.IsNullOrWhiteSpace(data.Formation))
                user.Formation = data.Formation;
            if (!string.IsNullOrWhiteSpace(data.WorkInstitution))
                user.WorkInstitution = data.WorkInstitution;

            UserManager.Update(user);


            return Ok();
        }

        #endregion

        #region Helpers



        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion




    }
}
