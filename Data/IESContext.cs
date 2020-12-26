using IES.Models.Infra;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modelo.Cadastros;
using Modelo.Discente;
using Modelo.Docente;

namespace IES.Data
{
    public class IESContext : IdentityDbContext<UsuarioDaAplicacao>
    {
        public IESContext(DbContextOptions<IESContext> options) : base(options)
        {

        }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Academico> Academicos { get; set; }
        public DbSet<Professor> Professores { get; set; }

        public DbSet<Modelo.Docente.CursoProfessor> CursoProfessor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CursoProfessor>()
                .HasKey(cd => new { cd.CursoID, cd.ProfessorID });
            modelBuilder.Entity<CursoProfessor>()
                .HasOne(c => c.Curso)
                .WithMany(cd => cd.CursosProfessores)
                .HasForeignKey(c => c.CursoID);

            modelBuilder.Entity<CursoProfessor>()
                .HasOne(d => d.Professor)
                .WithMany(cd => cd.CursoProfessores)
                .HasForeignKey(d => d.ProfessorID);


            modelBuilder.Entity<CursoDisciplina>()
                .HasKey(cd => new { cd.CursoID, cd.DisciplinaID });

            modelBuilder.Entity<CursoDisciplina>()
                .HasOne(c => c.Curso)
                .WithMany(cd => cd.CursoDisciplinas)
                .HasForeignKey(d => d.DisciplinaID);

            modelBuilder.Entity<CursoDisciplina>()
                .HasOne(d => d.Disciplina)
                .WithMany(cd => cd.CursosDisciplinas)
                .HasForeignKey(d => d.DisciplinaID);

        }

    }
}
