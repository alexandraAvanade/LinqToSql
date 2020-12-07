using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace linqSqlDemo
{
    public class Esercizi
    {
   
        const string conn = @"Persist Security Info=False; Integrated Security=true;Initial Catalog = CinemaDb; Server = DESKTOP-JJCBS1S\SQLEXPRESS";

        //Selezionare i film
        public static void SelectMovies()
        {
            CinemaDataContext db = new CinemaDataContext(conn);


            foreach(var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2}",movie.ID, movie.Titolo,movie.Genere);
            }


        }

        //Filtrare i film
        public static void FilterMovieByGenere()
        {

            CinemaDataContext db = new CinemaDataContext(conn);

            foreach (var movie in db.Movies)
            {
                Console.WriteLine("{0} - {1} - {2}", movie.ID, movie.Titolo, movie.Genere);
            }

            //Query 
            Console.WriteLine("Genere :");
            string Genere;
            Genere = Console.ReadLine();

            IQueryable<Movy> moviesFiltered =
                from m in db.Movies
                where m.Genere == Genere
                select m;

            foreach (var film in moviesFiltered)
            {
                Console.WriteLine("{0} - {1} - {2}", film.ID, film.Titolo, film.Genere);
            }

            Console.ReadKey();
            


        }

        //Inserire record
        public static void InsertMovie()
        {
            CinemaDataContext db = new CinemaDataContext(conn);

            SelectMovies();

            var movieToInsert = new Movy();
            movieToInsert.Titolo = "BBBBBBBBBBB";
            movieToInsert.Genere = "Romantico";
            movieToInsert.Durata = 120;

            db.Movies.InsertOnSubmit(movieToInsert);
            var deleteMovie = db.Movies.SingleOrDefault(m => m.ID == 2 );

            if (deleteMovie != null)
            {
                db.Movies.DeleteOnSubmit(deleteMovie);
            }

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            SelectMovies();

            Console.ReadKey();
        }

        //Update Movie
        public static void UpdateMovieByTitolo()
        {
            CinemaDataContext db = new CinemaDataContext(conn);

            Console.WriteLine("Dimmi il titolo del film da aggiornare: ");
            string titolo = Console.ReadLine();

            IQueryable<Movy> filmByTitolo =
                from film in db.Movies
                where film.Titolo == titolo
                select film;

            Console.WriteLine("I film trovati sono: {0}", filmByTitolo.Count());

            if (filmByTitolo.Count() == 0)
            {
                return;
            }

            if (filmByTitolo.Count() > 1)
            {
                return;
            }


            SelectMovies();

            Console.WriteLine("Scrivere is valori aggiornati ");
            Console.WriteLine("Titolo: ");
            string titolo1 = Console.ReadLine();

            Console.WriteLine("Genere: ");
            string genere = Console.ReadLine();

            Console.WriteLine("Durata: ");
            int durata = Convert.ToInt32(Console.ReadLine());


            foreach(var f in filmByTitolo)
            {
                f.Titolo = titolo1;
                f.Genere = genere;
                f.Durata = durata;
            };

            try
            {
                Console.WriteLine("Premi un tasto per mandare modifiche al db");
                Console.ReadKey();
                db.SubmitChanges(ConflictMode.FailOnFirstConflict);

            }
            catch (ChangeConflictException e)
            {

                Console.WriteLine("Concurrency error");
                Console.WriteLine(e.Message);
                Console.ReadKey();

                //OverwriteCurrentValues->ignora le mie modifiche(aggiorna il mio obj model)
                //KeepCurrentValues->ignora le modifiche altrui(sovrascrive il db con il mio obj model)
                //KeepChanges->cerca di tenere entrambe le modifiche 
                db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues);

                db.SubmitChanges();
            }


        }

    }
}
