using Patrimonios.Data;
using Patrimonios.Models;
using System;

namespace Patrimonios.Services
{
    public class LogService
    {
        private readonly PatrimoniosContext _context;

        public LogService(PatrimoniosContext context)
        {
            _context = context;
        }

        public void RegistrarLog(string usuario, string acao, string descricao)
        {
            var log = new LogEntry
            {
                Usuario = usuario,
                Acao = acao,
                Timestamp = DateTime.Now,
                Descricao = descricao
            };

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}
