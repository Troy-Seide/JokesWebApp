using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JokesWebAppTester.Data;
using Microsoft.AspNetCore.Authorization;

namespace JokesWebAppTester.Models
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JokesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
              return _context.Joke != null ? 
                          View(await _context.Joke.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Joke'  is null.");
        }

        // GET: Jokes/SearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            //This would mean go find the form with this particular name
            //return View("ShowSearchForm");

            //Since the method name "ShowSearchForm" is already the title we can just
            // have nothing in the View item, meaning this is an optional parameter
            return View();
        }

        // Post: Jokes/ShowSearchResults
        //The SearchPhrase value comes from our form
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            //return "You entered " + SearchPhrase;

            //The await command says we're gonna do some form of asynchronous programming and
            // then the view is saying we're gonna show something called searchResults
            //return View(await _context.Joke.ToListAsync());

            //Here we put in an optional parameter Index to tell it we're looking for the index view
            // and the following data to go with it. However, this only returned the data that was in the
            // view, we are trying to filter the data based on a user's search
            //return View("Index", await _context.Joke.ToListAsync());

            //We modify the code again to use the Where funtion which is like a SQL function
            // where you can specify a condition on how you want to select items. We then use
            // an annonymous/arrow function to filter the list based on our SearchPhrase
            return View("Index", await _context.Joke.Where(j => j.JokeQuestion.Contains(SearchPhrase)).ToListAsync());
        }

        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // GET: Jokes/Create

        //Here we add this decorator by using the square brackets and immported
        // the using Microsoft.AspNetCore.Authorization. This help to authorize the
        // create jokes functionality. So we are not able to do any creating of jokes unless
        // we're logged in.
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        // We also add authorization here because this is going to actually put the data in the db.
        // Which he says is more important for the security then the entry form
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,JokeQuestion,JokeAnswer")] Joke joke)
        {
            if (ModelState.IsValid)
            {
                _context.Add(joke);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Jokes/Edit/5

        //We also add authorize to here so a person cannot edit a joke unless they have been authorized
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }
            return View(joke);
        }

        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //We add authorize here too since this is a processing function for the edit method
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,JokeQuestion,JokeAnswer")] Joke joke)
        {
            if (id != joke.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(joke);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JokeExists(joke.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Jokes/Delete/5
        //We also add authorize on Delete so users cannot delete a joke unless authorized
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Joke == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.id == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // POST: Jokes/Delete/5

        //We also add authorize here on DeleteConfirmed
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Joke == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Joke'  is null.");
            }
            var joke = await _context.Joke.FindAsync(id);
            if (joke != null)
            {
                _context.Joke.Remove(joke);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
          return (_context.Joke?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
