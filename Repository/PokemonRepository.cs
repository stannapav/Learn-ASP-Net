using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context) 
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };

            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.Add(pokemonCategory);

            _context.Add(pokemon);

            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            var pokemonOwnerToDelete = _context.PokemonOwners.Where(po => po.Pokemon == pokemon).ToList();
            var pokemonCategoryToDelete = _context.PokemonCategories.Where(pc => pc.Pokemon == pokemon).ToList();

            _context.RemoveRange(pokemonOwnerToDelete);
            _context.RemoveRange(pokemonCategoryToDelete);
            _context.Remove(pokemon);

            return Save();
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var reviews = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);

            if (reviews.Count() <= 0)
                return 0;

            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p =>  p.Id).ToList();
        }

        public bool PokemonExist(int pokeId)
        {
            return _context.Pokemons.Any(p => p.Id == pokeId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var oldOwner = _context.PokemonOwners.Where(po => po.Pokemon == pokemon).FirstOrDefault();
            if (oldOwner.OwnerId != ownerId)
            {
                _context.Remove(oldOwner);

                var newOwner = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
                var pokemonOwner = new PokemonOwner
                {
                    Owner = newOwner,
                    Pokemon = pokemon
                };

                _context.Add(pokemonOwner);
            }

            var oldCategory = _context.PokemonCategories.Where(pc => pc.Pokemon == pokemon).FirstOrDefault();
            if (oldCategory.CategoryId != categoryId)
            {
                _context.Remove(oldCategory);

                var newCategory = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

                var pokemonCategory = new PokemonCategory
                {
                    Category = newCategory,
                    Pokemon = pokemon
                };

                _context.Add(pokemonCategory);
            }

            _context.Update(pokemon);

            return Save();
        }
    }
}