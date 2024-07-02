using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (commentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include(x => x.AppUser).ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context
                .Comments
                .Include(x => x.AppUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            var existingModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (existingModel == null)
            {
                return null;
            }
            existingModel.Title = commentModel.Title;
            existingModel.Content = commentModel.Content;
            await _context.SaveChangesAsync();
            return existingModel;
        }
    }
}
