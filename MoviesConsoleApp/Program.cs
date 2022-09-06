using Microsoft.EntityFrameworkCore;
using Persistencia.Entidades;
using Persistencia.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoviesConsoleApp
{
    class Program
    {
        static void Main(String[] args)
        {
            MovieContext _db = new MovieContext();

            #region # Exercício 01
            Console.WriteLine();
            Console.WriteLine("1. Listar o nome de todos personagens desempenhados por um determinado ator, incluindo a informação de qual o título do filme e o diretor");
            
            var questao1 = from c in _db.Characters
                                        .Include(a=> a.Actor)
                                        .Include(m => m.Movie)
                                        .ThenInclude(g => g.Genre)
                           where c.Actor.Name == "David Prowse"
                           select new
                           {
                               c.Actor.Name,
                               c.Character,
                               c.Movie.Title,
                               c.Movie.Director
                           };
            
            foreach (var item in questao1)
            {
                Console.WriteLine("\t Ator: {0} \t Personagem: {1} \t Título: {2} \t Diretor: {3}", item.Name, item.Character, item.Title, item.Director);
            }
            #endregion

            #region # Exercício 02
            Console.WriteLine();
            Console.WriteLine("2. Mostrar o nome e idade de todos atores que desempenharam um determinado personagem(por exemplo, quais os atores que já atuaram como '007' ?");
            
            var questao2 = (from c in _db.Characters
                                        .Include(a=> a.Actor)
                            where c.Character == "James Bond"
                            select new
                            {
                                c.Character,
                                c.Actor.Name,
                                Age = DateTime.Today.Year - c.Actor.DateBirth.Year
                            }).Distinct();
            
            foreach (var item in questao2)
            {
                Console.WriteLine("\t Personagem: {0} \t Ator: {1} \t Idade: {2} anos", item.Character, item.Name, item.Age);
            }
            #endregion

            #region # Exercício 03
            Console.WriteLine();
            Console.WriteLine("3. Informar qual o ator desempenhou mais vezes um determinado personagem(por exemplo: qual o ator que realizou mais filmes como o 'agente 007'");
            
            var NomePersonagem = "Darth Vader";
            
            var personagem = (from p in _db.Characters 
                            where p.Character == NomePersonagem 
                            select new {
                                p.Character 
                            }).Distinct();
            
            var questao3 = from c in _db.Characters
                           where c.Character == NomePersonagem
                           group c by c.Actor.Name into grpChar
                           orderby grpChar.Count() descending
                           select new
                           {
                               Ator = grpChar.Key,
                               Quantidade = grpChar.Count()
                           };
            
            Console.WriteLine("\t Personagem: {0}", personagem.First().Character);
            
            foreach (var item in questao3)
            {
                Console.WriteLine("\t Ator: {0}", item.Ator);
                Console.WriteLine("\t Participação: {0}", item.Quantidade);
            }
            #endregion

            #region # Exercício 04
            Console.WriteLine();
            Console.WriteLine("4. Mostrar o nome e a data de nascimento do ator mais idoso");
            
            var questao4 = from c in _db.Actors
                           orderby DateTime.Today.Year - c.DateBirth.Year descending
                           select new
                           {
                               c.Name,
                               c.DateBirth,
                               Age = DateTime.Today.Year - c.DateBirth.Year
                           };
            
            Console.WriteLine("\t Nome do Ator: {0} \t Data de Nascimento: {1} \t Idade: {2} anos", questao4.First().Name, questao4.First().DateBirth, questao4.First().Age);
            #endregion

            #region # Exercício 05
            Console.WriteLine();
            Console.WriteLine("5. Mostrar o nome e a data de nascimento do ator mais novo a atuar em um determinado gênero");
            
            var NomeGenero = "Romance";
            
            var questao5 = from c in _db.Characters
                                        .Include(a => a.Actor)
                                        .Include(m => m.Movie)
                                        .ThenInclude(g => g.Genre)
                           orderby DateTime.Today.Year - c.Actor.DateBirth.Year
                           where c.Movie.Genre.Name == NomeGenero
                           select new
                           {
                               c.Actor.Name,
                               c.Actor.DateBirth,
                               Idade = (DateTime.Today.Year - c.Actor.DateBirth.Year),
                               Genero = c.Movie.Genre.Name
                           };
            
            Console.WriteLine("\t Genero: {0}", NomeGenero);
            
            foreach (var item in questao5)
            {
                Console.WriteLine("\t Nome do Ator: {0} \t Data de Nascimento: {1} \t Idade: {2} anos \t Genero: {3}", item.Name, item.DateBirth, questao5.First().Age, item.Genero);
            }
            #endregion

            #region # Exercício 06
            Console.WriteLine();
            Console.WriteLine("6. Mostrar o valor médio das avaliações dos filmes de um determinado diretor");
            
            var director = "Martin Campbell";
            
            var DirectorName = (    
                                from m in _db.Movies 
                                where m.Director == director 
                                select new { 
                                    m.Director 
                                }).Distinct();
            
            var questao6 = from f in _db.Movies
                     where f.Director == director
                     group f by f.Title into grpMov
                     select new
                     {
                         Filme = grpMov.Key,
                         Avaliacao = grpMov.Average(e => e.Rating)
                     };
           
            Console.WriteLine("\t Diretor: {0}", DirectorName.First().Director);
            
            foreach (var item in questao6)
            {
                Console.WriteLine("\t Filme: {0}", item.Filme);
                Console.WriteLine("\t Avaliação média: {0}", item.Avaliacao);
            }

            #endregion

            #region # Exercício 07
            Console.WriteLine();
            Console.WriteLine("7. Qual o elenco do filme melhor avaliado ?");
            
            var MaxRating = _db.Movies.Max(r => r.Rating);
            
            var MovieMaxRating = (
                                from m in _db.Movies 
                                where m.Rating == MaxRating 
                                select new { 
                                    m.Title
                                }).Distinct();
            
            var questao7 = from m in _db.Characters.Include(m => m.Movie)
                                                    .Include(a => a.Actor)
                      where m.Movie.Rating == MaxRating
                      select new
                      {
                          m.Actor.Name
                      };
            
            Console.WriteLine("\t Filme: {0}", MovieMaxRating.First().Title);
            
            foreach (var item in questao7)
            {
                Console.WriteLine("\t Nome do Ator/Atriz: {0} ", item.Name);
            }
            #endregion

            #region # Exercício 08
            Console.WriteLine();
            Console.WriteLine("8. Qual o elenco do filme com o maior faturamento?");
            
            var MaxGross = _db.Movies.Max(g => g.Gross);
            var MovieMaxGross = (from m in _db.Movies where m.Gross == MaxGross select new { m.Title }).Distinct();
            
            var questao8 = from m in _db.Characters
                                    .Include(m => m.Movie)
                                    .Include(a => a.Actor)
                           where m.Movie.Gross == MaxGross
                           select new
                           {
                               m.Actor.Name
                           };
            
            Console.WriteLine("\t Filme: {0}", MovieMaxGross.First().Title);
            
            foreach (var res in questao8)
            {
                Console.WriteLine("\t Nome do Ator/Atriz: {0} ", res.Name);
            }
            #endregion

            #region # Exercício 09
            Console.WriteLine();
            Console.WriteLine("9. Gerar um relatório de aniversariantes, agrupando os atores pelo mês de aniverário.");

            Console.WriteLine("9 - Query Fail :( ");

            #endregion

            Console.WriteLine("- - -   feito!  - - - ");
            Console.WriteLine();
        }

        static void Main_presencial(String[] args)
        {
            MovieContext _db = new MovieContext();

            #region # LINQ - consultas

            Console.WriteLine();
            Console.WriteLine("1. Todos os filmes de acao");

            Console.WriteLine("1a. Modelo tradicional");
            List<Movie> filmes1a = new List<Movie>();
            foreach (Movie f in _db.Movies.Include("Genre"))
            {
                if (f.Genre.Name == "Action")
                    filmes1a.Add(f);
            }

            foreach (Movie filme in filmes1a)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.ReleaseDate.Year);
            }

            Console.WriteLine("\n1b. Usando linq - query syntax");
            var filmes1b = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f;
            foreach (Movie filme in filmes1b)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.Director);
            }

            Console.WriteLine("\n1c. Usando linq - method syntax");
            var filmes1c = _db.Movies.Where(m => m.Genre.Name == "Action");
            foreach (Movie filme in filmes1c)
            {
                Console.WriteLine("\t{0}", filme.Title);
            }

 
            Console.WriteLine();
            Console.WriteLine("2. Todos os diretores de filmes do genero 'Action', com projecao");
            var filmes2 = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f.Director;

            foreach (var nome in filmes2)
            {
                Console.WriteLine("\t{0}", nome);
            }

            Console.WriteLine();
            Console.WriteLine("3a. Todos os filmes de cada genero (query syntax):");
            var generosFilmes3a = from g in _db.Genres.Include(gen => gen.Movies)
                                select g;
            foreach (var gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0} - {1}", f.Title, f.Rating);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("3b. Todos os filmes de cada genero (method syntax):");

            var generosFilmes3b = _db.Genres.Include(gen => gen.Movies).ToList();

            foreach (Genre gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0}", f.Title);
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("4. Titulo e ano dos filmes do diretor Quentin Tarantino, com projcao em uma class anonima:");
            var tarantino = from f in _db.Movies
                            where f.Director == "Quentin Tarantino"
                             select new
                             {
                                 Ano = f.ReleaseDate.Year,
                                 f.Title
                             };

            foreach (var item in tarantino)
            {
                Console.WriteLine("{0} - {1}", item.Ano, item.Title);
            }

            Console.WriteLine();
            Console.WriteLine("5. Todos os gêneros ordenados pelo nome:");
            var q5 = _db.Genres.OrderByDescending(g => g.Name);
            foreach (var genero in q5)
            {
                Console.WriteLine("{0, 20}\t {1}", genero.Name, genero.Description.Substring(0, 30));
            }

            Console.WriteLine();
            Console.WriteLine("6. Numero de filmes agrupados pelo anos de lançamento:");
            var q6 = from f in _db.Movies
                     group f by f.ReleaseDate.Year into grupo
                     select new
                     {
                         Chave = grupo.Key,
                         NroFilmes = grupo.Count()
                     };

            foreach (var ano in q6.OrderByDescending(g => g.NroFilmes))
            {
                Console.WriteLine("Ano: {0}  Numero de filmes: {1}", ano.Chave, ano.NroFilmes);

            }

            Console.WriteLine();
            Console.WriteLine("7. Projeção do faturamento total, quantidade de filmes e avaliação média agrupadas por gênero:");
            var q7 = from f in _db.Movies
                     group f by f.Genre.Name into grpGen
                     select new
                     {
                         Categoria = grpGen.Key,
                         Faturamento = grpGen.Sum(e => e.Gross),
                         Avaliacao = grpGen.Average(e => e.Rating),
                         Quantidade = grpGen.Count()
                     };

            foreach (var genero in q7)
            {
                Console.WriteLine("Genero: {0}", genero.Categoria);
                Console.WriteLine("\tFaturamento total: {0}\n\t Avaliação média: {1}\n\tNumero de filmes: {2}",
                                genero.Faturamento, genero.Avaliacao, genero.Quantidade);
            }
            #endregion



        }

        static void Main_CRUd(string[] args)
        {
            Console.WriteLine("Hello World!");

            MovieContext _context = new MovieContext();

            Genre g1 = new Genre()
            {
                Name = "Comedia",
                Description = "Filmes de comedia"
            };

            Genre g2 = new Genre()
            {
                Name = "Ficcao",
                Description = "Filmes de ficcao"
            };

            _context.Genres.Add(g1);
            _context.Genres.Add(g2);

            _context.SaveChanges();

            List<Genre> genres = _context.Genres.ToList();

            foreach (Genre g in genres)
            {
                Console.WriteLine(String.Format("{0,2} {1,-10} {2}",
                                    g.GenreId, g.Name, g.Description));
            }

        }
    }
}
