using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment5.Data;
using Assignment5.Models;

namespace Assignment5.Controllers
{
    public class SongsController : Controller
    {
        private readonly Assignment5Context _context;

        public SongsController(Assignment5Context context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
              return _context.Song != null ? 
                          View(await _context.Song.ToListAsync()) :
                          Problem("Entity set 'Assignment5Context.Song'  is null.");
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,Title,Artist,Genre,Price")] Song song)
        {
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SongId,Title,Artist,Genre,Price")] Song song)
        {
            if (id != song.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.SongId))
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
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Song == null)
            {
                return Problem("Entity set 'Assignment5Context.Song'  is null.");
            }
            var song = await _context.Song.FindAsync(id);
            if (song != null)
            {
                _context.Song.Remove(song);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
          return (_context.Song?.Any(e => e.SongId == id)).GetValueOrDefault();
        }

        //Browse
        public async Task<IActionResult> Browse(string SongGenre, string SongArtist)
        {
            ViewBag.ShowSongs = false;
            ViewBag.ShowArtists = false;

            //Get genres to add to dropdown.
            IQueryable<string> genreQuery = from m in _context.Song
                                            orderby m.Genre
                                            select m.Genre;

            //Get aritsts to add to dropdown
            IQueryable<string> artistQuery;

            if (SongGenre == "All")
            {
                artistQuery = from m in _context.Song
                              orderby m.Artist
                              select m.Artist;
            }
            else
            {
                artistQuery = from m in _context.Song
                              where m.Genre == SongGenre
                              orderby m.Artist
                              select m.Artist;
            }

            var songs = from m in _context.Song
                         select m;

            if (!string.IsNullOrEmpty(SongGenre))
            {
                ViewBag.ShowArtists = true;
                if (SongGenre != "All") //only filter if genre isn't "All"
                {
                    songs = songs.Where(x => x.Genre == SongGenre);
                }
                if (!string.IsNullOrEmpty(SongArtist))
                {
                    if (SongArtist != "All") //only filter if artist isn't "All"
                    {
                        songs = songs.Where(x => x.Artist == SongArtist);
                    }
                    ViewBag.ShowSongs = true;
                }
            }

            

            var browseSongVM = new BrowseSongViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Artists = new SelectList(await artistQuery.Distinct().ToListAsync()),
                Songs = await songs.ToListAsync()
            };

            return View(browseSongVM);

        }
        //AddToCart
        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null || _context.Song == null)
            {
                return NotFound();
            }
            var song = await _context.Song
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (song == null)
            {
                return NotFound();
            }
            if (Cart.Songs == null)
            {
                Cart.Songs = new List<Song>();
            }
            Cart.Songs.Add(song);
            return RedirectToAction(nameof(Browse));
        }

        //ShoppingCart
        public IActionResult ShoppingCart()
        {
            if (Cart.Songs == null)
            {
                Cart.Songs = new List<Song>();
            }
            decimal total = 0;
            foreach (var song in Cart.Songs)
            {
                total += song.Price;
            }
            ViewBag.Total = total.ToString("c");
            return View();
        }

        //Remove from Cart
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            foreach (Song song in Cart.Songs)
            {
                if (song.SongId == id) {
                    Cart.Songs.Remove(song);
                    break;
                }
            }
            return RedirectToAction(nameof(ShoppingCart));
        }
    }
}
