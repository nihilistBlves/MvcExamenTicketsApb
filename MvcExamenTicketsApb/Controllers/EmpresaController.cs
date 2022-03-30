using MvcExamenTicketsApb.Models;
using Microsoft.AspNetCore.Mvc;
using MvcExamenTicketsApb.Filters;
using MvcExamenTicketsApb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MvcExamenTicketsApb.Controllers {
    public class EmpresaController : Controller {
        private ServiceApiEmpresa serviceApiEmpresa;
        private ServiceBlob serviceBlob;
        public EmpresaController(ServiceApiEmpresa serviceApiEmpresa, ServiceBlob serviceBlob) {
            this.serviceApiEmpresa = serviceApiEmpresa;
            this.serviceBlob = serviceBlob;
        }
        [AuthorizeUsuarios]
        public IActionResult FindTicket() {
            return View();
        }
        [HttpPost]
        [AuthorizeUsuarios]
        public async Task<IActionResult> FindTicket(int idticket) {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Ticket ticket = await this.serviceApiEmpresa.FindTicketAsync(token, idticket);
            return View(ticket);
        }
        public IActionResult CreateUsuario() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUsuario(Usuario usuario) {
            await this.serviceApiEmpresa.CreateUsuarioAsync(usuario);
            return RedirectToAction("LogIn", "Manage");
        }
        [AuthorizeUsuarios]
        public IActionResult CreateTicket() {
            return View();
        }
        [HttpPost]
        [AuthorizeUsuarios]
        public async Task<IActionResult> CreateTicket(DateTime fecha, string importe, string producto, IFormFile file) {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Usuario usuario = await this.serviceApiEmpresa.FindUsuarioAsync(token);
            using (Stream stream = file.OpenReadStream()) {
                await this.serviceBlob.UploadBlobAsync(file.FileName, stream);
            }
            BlobClass blob = await this.serviceBlob.FindBlobByName(file.FileName);
            Ticket ticket = new Ticket {
                IdTicket = 1,
                IdUsuario = usuario.IdUsuario,
                Fecha = fecha,
                Importe = importe,
                Producto = producto,
                Filename = file.FileName,
                Url = blob.Url
            };
            await this.serviceApiEmpresa.CreateTicketAsync(ticket, token);
            return RedirectToAction("TicketsUsuario");
        }
        [AuthorizeUsuarios]
        public async Task<IActionResult> TicketsUsuario() {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            List<Ticket> tickets = await this.serviceApiEmpresa.GetTicketsByUsuarioAsync(token);
            return View(tickets);
        }
        [AuthorizeUsuarios]
        public async Task<IActionResult> ProcessTicket(int idticket) {
            string token = HttpContext.User.FindFirst("TOKEN").Value;
            Ticket ticket = await this.serviceApiEmpresa.FindTicketAsync(token, idticket);
            await this.serviceApiEmpresa.ProcessTicket(token, ticket);
            return RedirectToAction("TicketsUsuario");
        }
    }
}
