using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAP.MicroService.Jogos.Dominio.Models
{
    public class ResultadoAgregado
    {
        public string Chave { get; set; }
        public decimal ReceitaTotal { get; set; }
        public int TotalVendas { get; set; }
    }
}
