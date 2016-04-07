using Discord.API.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Discord.API
{
    public class DiscordClient
    {
        readonly RestClient restClient = new RestClient(@"https://discordapp.com/api/");

        Message lastest = null;
        string auth = "";
        
        public Error LastestError { get; set; }

        #region Authentication
        public bool Login(string username, string password)
        {
            var request = MakeRequest(@"auth/login", Method.POST);
            request.AddBody(new { email = username, password = password });

            var response = restClient.Execute(request);

            auth = ParseResponse<Authentication>(response).token;

            return LastestError != null;
        }

        public bool Logout()
        {
            var request = MakeRequest(@"auth/logout", Method.POST);
            var response = restClient.Execute(request);

            auth = "";

            return CheckSuccess(response);
        }
        #endregion

        #region Channels
        #region General
        public void CreateChannel()
        {
            throw new NotImplementedException();
        }

        public void EditChannel()
        {
            throw new NotImplementedException();
        }

        public void DeleteChannel()
        {
            throw new NotImplementedException();
        }

        public void BroadcastTyping()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Messages
        public List<Message> GetMessages(Channel channel, string before = null, string after = null, int limit = -1)
        {
            var resource = new StringBuilder(@"channels/" + channel.id + @"/messages");

            char prefix = '?';

            if (before != null)
            {
                resource.Append(prefix + @"before=" + before);
                prefix = '&';
            }

            if (after != null)
            {
                resource.Append(prefix + @"after=" + after);
                prefix = '&';
            } 

            if (limit >= 0)
            {
                resource.Append(prefix + @"limit=" + limit);
            }

            var request = MakeRequest(resource.ToString(), Method.GET);

            var response = restClient.Execute(request);
            var res = ParseResponse<List<Message>>(response);
            var first = res.FirstOrDefault();


            if (first != null)
            {
                lastest = first;
            }

            return res;
        }

        public List<Message> GetLatestMessages(Channel channel, int limit = -1)
        {
            return GetMessages(channel, after: (lastest != null) ? lastest.id : null, limit: limit);
        }

        public bool SendMessage(Channel channel, string str, bool tts = false)
        {
            var request = MakeRequest(@"channels/" + channel.id + @"/messages", Method.POST);
            request.AddBody(new { content = str, tts = tts });

            var response = restClient.Execute(request);

            return CheckSuccess(response);
        }

        public void EditMessage()
        {
            throw new NotImplementedException();
        }

        public void DeleteMessage()
        {
            throw new NotImplementedException();
        }

        public void AcknowledgeMessage()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Permissions
        public void CreateEditPermission()
        {
            throw new NotImplementedException();
        }

        public void DeletePermission()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Guilds
        #region General
        public void CreateGuild()
        {
            throw new NotImplementedException();
        }

        public void EditGuild()
        {
            throw new NotImplementedException();
        }

        public void LeaveGuild()
        {
            throw new NotImplementedException();
        }

        public void DeleteGuild()
        {
            throw new NotImplementedException();
        }

        public List<Guild> GetGuilds()
        {
            var request = MakeRequest(@"users/@me/guilds", Method.GET);
            var response = restClient.Execute(request);

            return ParseResponse<List<Guild>>(response);
        }

        public List<Channel> GetGuildChannels(Guild guild)
        {
            var request = MakeRequest(@"guilds/" + guild.id + @"/channels", Method.GET);

            var response = restClient.Execute(request);

            return ParseResponse<List<Channel>>(response);
        }
        #endregion

        #region Members
        public void EditMember()
        {
            throw new NotImplementedException();
        }

        public void KickMember()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Bans
        public void GetBans()
        {
            throw new NotImplementedException();
        }

        public void AddBan()
        {
            throw new NotImplementedException();
        }

        public void RemoveBan()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Roles
        public void CreateRole()
        {
            throw new NotImplementedException();
        }

        public void EditRole()
        {
            throw new NotImplementedException();
        }

        public void ReorderRoles()
        {
            throw new NotImplementedException();
        }

        public void DeleteRole()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Invites
        public void GetInvite()
        {
            throw new NotImplementedException();
        }

        public void AcceptInvite()
        {
            throw new NotImplementedException();
        }

        public void CreateInvite()
        {
            throw new NotImplementedException();
        }

        public void DeleteInvite()
        {
            throw new NotImplementedException();
        }

        public void GetGuildInvites()
        {
            throw new NotImplementedException();
        }

        public void GetChannelInvites()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Users
        #region General
        public void CreatePrivateChannel()
        {
            throw new NotImplementedException();
        }

        public void GetAvatar()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Profile
        public void EditProfile()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Status
        #region Maintenances
        public void ActiveMaintenances()
        {
            throw new NotImplementedException();
        }

        public void UpcomingMaintenances()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Voice
        #region General
        public void GetServerRegions()
        {
            throw new NotImplementedException();
        }

        public void MoveMember()
        {
            throw new NotImplementedException();
        }

        public void UnknownSpookyMethod()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region WebSockets
        #region General
        public void GetEndpoint()
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion


        private RestRequest MakeRequest(string resource, Method method)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;

            if (auth != null)
                request.AddHeader("Authorization", auth);

            return request;
        }

        private T ParseResponse<T>(IRestResponse response) where T : new()
        {
            if (CheckSuccess(response))
            {
                try
                {
                    LastestError = null;
                    return JsonConvert.DeserializeObject<T>(response.Content);
                }
                catch
                {
                    LastestError = new Error("JsonError: ", "Json deserialization faild!");
                }
            }

            return new T();
        }

        private bool CheckSuccess(IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    LastestError = null;
                    return true;
                /*
                case HttpStatusCode.Continue:
                case HttpStatusCode.SwitchingProtocols:
                case HttpStatusCode.NonAuthoritativeInformation:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.NotModified:
                case HttpStatusCode.UseProxy:
                case HttpStatusCode.Unused:
                case HttpStatusCode.TemporaryRedirect:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.Gone:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.UnsupportedMediaType:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.UpgradeRequired:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.HttpVersionNotSupported:
                */
                default:
                    LastestError = new Error(response.StatusCode.ToString(), response.ErrorMessage);
                    return false;
            }
        }
    }
}
