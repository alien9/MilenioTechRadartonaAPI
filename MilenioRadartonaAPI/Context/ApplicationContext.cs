﻿using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using MilenioRadartonaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilenioRadartonaAPI.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }


        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RequisicaoInfos> Requisicoes { get; set; }
        public DbSet<Caminhao> Caminhoes { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasKey(k => k.UsuarioId);
            modelBuilder.Entity<RequisicaoInfos>().HasKey(k => k.RequisicaoInfosId);
            modelBuilder.Entity<Mensagem>().HasKey(k => k.MensagemId);


            /*    RELAÇÕES    */
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Usuario>().HasOne(u => u.Chave).WithOne(c => c.Usuario);
            modelBuilder.Entity<Usuario>().HasMany(u => u.ReqInfos).WithOne(c => c.Usuario);


            modelBuilder.Entity<Mensagem>().HasOne(pf => pf.Caminhao);
            modelBuilder.Entity<Mensagem>().HasOne(pf => pf.Usuario);

        }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }


    }
}
