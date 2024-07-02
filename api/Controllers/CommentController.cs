using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(
            ICommentRepository commentRepository,
            IStockRepository stockRepository,
            UserManager<AppUser> userManager
        )
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllAsync();
            var commentDtos = comments.Select(x => x.ToCommentDto());
            return Ok(commentDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(comment.ToCommentDto());
        }

        [Authorize]
        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create(
            [FromRoute] int stockId,
            [FromBody] CreateCommentDto commentDto
        )
        {
            if (!await _stockRepository.StockExists(stockId))
            {
                return BadRequest("Stock does not exists");
            }

            var username = User.GetUsername();
            if (username == null)
            {
                return StatusCode(500, "Could not find username");
            }
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return StatusCode(500, "Could not find appUser from username");
            }
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            commentModel.AppUserId = appUser.Id;
            
            await _commentRepository.CreateAsync(commentModel);
            return CreatedAtAction(
                nameof(GetById),
                new { id = commentModel.Id },
                commentModel.ToCommentDto()
            );
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateCommentDto commentDto
        )
        {
            var commentModel = await _commentRepository.UpdateAsync(
                id,
                commentDto.ToCommentFromUpdate()
            );
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(commentModel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentModel = await _commentRepository.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(commentModel);
        }
    }
}
