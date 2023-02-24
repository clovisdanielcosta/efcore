using System;
using System.Collections.Generic;
using System.Linq;
using CursoEFCore.Domain;
using CursoEFCore.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new Data.ApplicationContext();
            
            //db.Database.Migrate();
            // var existe = db.Database.GetPendingMigrations().Any();
            // if(existe)
            // {
            //     Console.WriteLine("Existe migrações pendentes");
            // } else
            // {
            //     Console.WriteLine("Não existe migrações pendentes");
            // }
            // InserirDados();
            // InserirDadosEmMassa();
            // ConsultaDados();
            // CadastrarPedido();
            //  ConsultaPedidoCarregamentoAdiantado();
            // AtualizarDados();
            RemoverRegistro();
        }
        private static void RemoverRegistro()
        {
            using var db = new Data.ApplicationContext();
            // var cliente = db.Clientes.Find(4);
            var cliente = new Cliente {Id = 5};
            // db.Clientes.Remove(cliente);
            // db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;
            db.SaveChanges();
        }
        private static void AtualizarDados()
        {
            using var db = new Data.ApplicationContext();
            // var cliente = db.Clientes.FirstOrDefault(p => p.Id == 1);
            // var cliente = db.Clientes.Find(1);
            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new 
            {
                Nome = "Cliente Desconectado Passo 3",
                Telefone = "981125258"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);
            // cliente.Nome = "Cliente Alterado Passo 2";
            // db.Entry(cliente).State = EntityState.Modified; //Se quiser explicitar a alteração de todas as prpriedades da instância.
            // db.Clientes.Update(cliente); // Se quiser que atualize todas as colunas. 
            // Senão, atualiza apenas o campo acima "cliente.Nome".
            db.SaveChanges();

        }
        private static void ConsultaPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(p => p.Produto)
                .ToList();
            // var pedidos = db.Pedidos.Include("Itens") .ToList();

            Console.WriteLine(pedidos.Count);
       }
        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens  = new List<PedidoItem>
                {
                    new PedidoItem{
                        ProdutoId = produto.Id,
                        Desconto = 0,
                        Quantidade = 1,
                        Valor = 10,
                    }
                    
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }
        private static void InserirDadosEmMassa()
        {
             var produto = new Produto
            {
              Descricao = "Produto Teste",
              CodigoBarras = "1234567891231",
              Valor = 10m,
              TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
              Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Teste Daniel",
                CEP = "99999999",
                Cidade = "Warwick",
                Estado = "NY",
                Telefone = "99999511213"
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste Luisa",
                    CEP = "88588877",
                    Cidade = "Warwick",
                    Estado = "NY",
                    Telefone = "99999511215"
                },
                new Cliente
                {
                    Nome = "Teste Marcia",
                    CEP = "88588877",
                    Cidade = "Warwick",
                    Estado = "NY",
                    Telefone = "99998585883"
                },
            };

            using var db = new Data.ApplicationContext();
            // db.AddRange(produto, cliente);
            db.AddRange(listaClientes);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total de registros inseridos em massa: {registros}");

        }
        private static void InserirDados()
        {
            var produto = new Produto
            {
              Descricao = "Produto Teste",
              CodigoBarras = "1234567891231",
              Valor = 10m,
              TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
              Ativo = true
            };

            using var db = new Data.ApplicationContext();
            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();
            Console.Write($"Total de Registo(s): {registros}");

        }
        private static void ConsultaDados()
        {
            using var db = new Data.ApplicationContext();
            // var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            // var consultaPorMetodo = db.Clientes.AsNoTracking().Where(p => p.Id > 0).ToList();
            var consultaPorMetodo = db.Clientes
                .Where(p => p.Id > 0)
                .OrderBy(p => p.Nome)
                .ToList();

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id} | {cliente.Nome}");
                // db.Clientes.Find(cliente.Id); //Consulta objetos na memória primeiro se não houver "AsNoTracking()"
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }
        }
}
