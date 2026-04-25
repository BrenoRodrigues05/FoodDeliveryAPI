using FoodDeliveryAPI.Domains.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryAPI.Context
{
    public class FoodDeliveryAPIContext : IdentityDbContext <ApplicationUser>
    {
       public FoodDeliveryAPIContext(DbContextOptions<FoodDeliveryAPIContext> options) : base (options){ }

        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<PalavraProibida> PalavrasProibidas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
       {
            base.OnModelCreating(builder);

            builder.Entity<Pedido>()
              .HasMany(p => p.PedidoItens)
              .WithOne(p => p.Pedido)
              .HasForeignKey(p => p.PedidoId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasPrecision(10, 2);

            builder.Entity<Pedido>()
                .HasOne(p => p.Entregador)
                .WithMany(e => e.Pedidos)
                .HasForeignKey(p => p.EntregadorId);

             builder.Entity<PedidoItem>()
                .HasOne(pi => pi.Produto)
                .WithMany(e => e.PedidoItens)
                .HasForeignKey(pi => pi.ProdutoId);

            builder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId);

             builder.Entity<Cliente>()
                .HasOne(c => c.Endereco)
                .WithOne(e => e.Cliente)
                .HasForeignKey<Endereco>(e => e.ClienteId);

             builder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasPrecision(10, 2);

            builder.Entity<PalavraProibida>().HasData(
            new PalavraProibida("xxx") { Id = 1 },
            new PalavraProibida("idiota") { Id = 2 },
            new PalavraProibida("burro") { Id = 3 }
            // adicionar mais palavras proibidas conforme necessário
    );
        }
    }
}
