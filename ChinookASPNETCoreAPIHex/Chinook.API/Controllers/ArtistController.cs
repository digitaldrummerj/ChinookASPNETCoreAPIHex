﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chinook.Domain.Repositories;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using Chinook.API.Supervisor;
using Chinook.Domain.ViewModels;

namespace Chinook.API.Controllers
{
    [Route("api/[controller]")]
    public class ArtistController : Controller
    {
        private readonly IChinookSupervisor _chinookSupervisor;

        public ArtistController(IChinookSupervisor chinookSupervisor)
        {
            _chinookSupervisor = chinookSupervisor;
        }

        [HttpGet]
        [Produces(typeof(List<ArtistViewModel>))]
        public async Task<IActionResult> Get(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                return new ObjectResult(await _chinookSupervisor.GetAllArtistAsync(ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{id}")]
        [Produces(typeof(ArtistViewModel))]
        public async Task<IActionResult> Get(int id, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (await _chinookSupervisor.GetArtistByIdAsync(id, ct) == null)
                {
                    return NotFound();
                }
                return Ok(await _chinookSupervisor.GetArtistByIdAsync(id, ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Produces(typeof(ArtistViewModel))]
        public async Task<IActionResult> Post([FromBody]ArtistViewModel input, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (input == null)
                    return BadRequest();
                
                return Ok(await _chinookSupervisor.AddArtistAsync(input, ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("{id}")]
        [Produces(typeof(void))]
        public async Task<IActionResult> Put(int id, [FromBody]ArtistViewModel input, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (input == null)
                    return BadRequest();
                if (await _chinookSupervisor.GetArtistByIdAsync(id, ct) == null)
                {
                    return NotFound();
                }
                var errors = JsonConvert.SerializeObject(ModelState.Values
                .SelectMany(state => state.Errors)
                .Select(error => error.ErrorMessage));
                Debug.WriteLine(errors);

                if (await _chinookSupervisor.UpdateArtistAsync(input, ct))
                {
                    return Ok(input);
                }

                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (await _chinookSupervisor.GetAlbumByIdAsync(id, ct) == null)
                {
                    return NotFound();
                }

                if (await _chinookSupervisor.DeleteAlbumAsync(id, ct))
                {
                    return Ok();
                }

                return StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
