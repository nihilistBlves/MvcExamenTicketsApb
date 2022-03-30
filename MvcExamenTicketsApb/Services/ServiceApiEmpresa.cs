using MvcExamenTicketsApb.Models;
using MvcExamenTicketsApb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MvcExamenTicketsApb.Services {
    public class ServiceApiEmpresa {
        private Uri UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        public ServiceApiEmpresa(string urlapi) {
            this.UrlApi = new Uri(urlapi);
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }
        public async Task<string> GetTokenAsync(string username, string password) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel {
                    Username = username,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                string request = "/api/Auth/Login";
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode) {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    string token = jObject.GetValue("response").ToString();
                    return token;
                }
                else {
                    return null;
                }
            }
        }
        private async Task<T> CallApiAsync<T>(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode) {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else {
                    return default(T);
                }

            }
        }
        public async Task<Usuario> FindUsuarioAsync(string token) {
            string request = "/api/empresa/findusuario";
            Usuario usuario = await this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }
        public async Task<List<Ticket>> GetTicketsByUsuarioAsync(string token) {
            string request = "/api/empresa/ticketsusuario";
            List<Ticket> tickets = await this.CallApiAsync<List<Ticket>>(request, token);
            return tickets;
        }
        public async Task<Ticket> FindTicketAsync(string token, int idticket) {
            string request = "/api/empresa/findticket/"+idticket;
            Ticket ticket = await this.CallApiAsync<Ticket>(request, token);
            return ticket;
        }
        public async Task CreateUsuarioAsync(Usuario usuario) {
            using (HttpClient client = new HttpClient()) {
                string request = "/api/empresa/createusuario";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(usuario);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }
        public async Task CreateTicketAsync(Ticket ticket, string token) {
            using (HttpClient client = new HttpClient()) {
                string request = "/api/empresa/createticket";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                string json = JsonConvert.SerializeObject(ticket);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }
        public async Task ProcessTicket(string token, Ticket ticket) {
            using (HttpClient client = new HttpClient()) {
                string request = "/api/empresa/processticket";
                client.BaseAddress = this.UrlApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(ticket);
                StringContent content = new StringContent
                    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }
    }
}
