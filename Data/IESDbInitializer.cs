using IES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IES.Data
{
    public class IESDbInitializer
    {
        public static void Initialize(IESContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (context.Departamentos.Any())
            {
                return;
            }

            var instituicoes = new Instituicao[]
            {
                new Instituicao { Nome = "Ciência da Computação", Endereco = "Paraná"},
                new Instituicao { Nome = "Ciência de Alimentos" , Endereco="Acre"}
            };

            foreach (var d in instituicoes)
            {
                context.Instituicoes.Add(d);
            }
            context.SaveChanges();

            var departamentos = new Departamento[]
            {
                new Departamento {Nome="Ciência", InstituicaoId = 1},
                new Departamento {Nome="Ciência e Alimentos", InstituicaoId = 2}
            };

            foreach (var d in departamentos)
            {
                context.Departamentos.Add(d);
            }
            context.SaveChanges();
        }
    }
}
