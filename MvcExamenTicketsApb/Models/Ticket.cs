using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MvcExamenTicketsApb.Models {
    public class Ticket {
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha{ get; set; }
        public string Importe { get; set; }
        public string Producto { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
    }
}
