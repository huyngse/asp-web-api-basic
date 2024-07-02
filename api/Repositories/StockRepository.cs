using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModel == null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stocks.Include(x => x.Comments).ThenInclude(x => x.AppUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending
                        ? stocks.OrderByDescending(s => s.Symbol)
                        : stocks.OrderBy(s => s.Symbol);
                }
            }
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context
                .Stocks
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context
                .Stocks
                .FirstOrDefaultAsync(x => x.Symbol == symbol);
        }

        public async Task<bool> StockExists(int id)
        {
            return await _context.Stocks.AnyAsync(x => x.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, Stock stockModel)
        {
            var existingModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (existingModel == null)
            {
                return null;
            }
            existingModel.Symbol = stockModel.Symbol;
            existingModel.CompanyName = stockModel.CompanyName;
            existingModel.Purchase = stockModel.Purchase;
            existingModel.LastDiv = stockModel.LastDiv;
            existingModel.Industry = stockModel.Industry;
            existingModel.MarketCap = stockModel.MarketCap;
            await _context.SaveChangesAsync();
            return existingModel;
        }
    }
}
