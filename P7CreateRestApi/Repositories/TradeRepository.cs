using Microsoft.EntityFrameworkCore;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly LocalDbContext _context;
        public TradeRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TradeDto>> GetAllAsync()
        {
            return await _context.Trades
                .Select(t => new TradeDto
                {
                    TradeId = t.TradeId,
                    Account = t.Account,
                    AccountType = t.AccountType,
                    BuyQuantity = t.BuyQuantity,
                    SellQuantity = t.SellQuantity,
                    BuyPrice = t.BuyPrice,
                    SellPrice = t.SellPrice,
                    TradeDate = t.TradeDate,
                    TradeSecurity = t.TradeSecurity,
                    TradeStatus = t.TradeStatus,
                    Trader = t.Trader,
                    Benchmark = t.Benchmark,
                    Book = t.Book,
                    CreationName = t.CreationName,
                    CreationDate = t.CreationDate,
                    RevisionName = t.RevisionName,
                    RevisionDate = t.RevisionDate,
                    DealName = t.DealName,
                    DealType = t.DealType,
                    SourceListId = t.SourceListId,
                    Side = t.Side
                })
                .ToListAsync();
        }

        public async Task<TradeDto?> GetByIdAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
                return null;

            return new TradeDto
            {
                TradeId = trade.TradeId,
                Account = trade.Account,
                AccountType = trade.AccountType,
                BuyQuantity = trade.BuyQuantity,
                SellQuantity = trade.SellQuantity,
                BuyPrice = trade.BuyPrice,
                SellPrice = trade.SellPrice,
                TradeDate = trade.TradeDate,
                TradeSecurity = trade.TradeSecurity,
                TradeStatus = trade.TradeStatus,
                Trader = trade.Trader,
                Benchmark = trade.Benchmark,
                Book = trade.Book,
                CreationName = trade.CreationName,
                CreationDate = trade.CreationDate,
                RevisionName = trade.RevisionName,
                RevisionDate = trade.RevisionDate,
                DealName = trade.DealName,
                DealType = trade.DealType,
                SourceListId = trade.SourceListId,
                Side = trade.Side
            };
        }

        public async Task<TradeDto> AddAsync(TradeDto dto)
        {
            var trade = new Trade
            {
                Account = dto.Account,
                AccountType = dto.AccountType,
                BuyQuantity = dto.BuyQuantity,
                SellQuantity = dto.SellQuantity,
                BuyPrice = dto.BuyPrice,
                SellPrice = dto.SellPrice,
                TradeDate = dto.TradeDate,
                TradeSecurity = dto.TradeSecurity,
                TradeStatus = dto.TradeStatus,
                Trader = dto.Trader,
                Benchmark = dto.Benchmark,
                Book = dto.Book,
                CreationName = dto.CreationName,
                CreationDate = dto.CreationDate,
                RevisionName = dto.RevisionName,
                RevisionDate = dto.RevisionDate,
                DealName = dto.DealName,
                DealType = dto.DealType,
                SourceListId = dto.SourceListId,
                Side = dto.Side
            };

            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();

            dto.TradeId = trade.TradeId;

            return dto;
        }
        public async Task<bool> UpdateAsync(int id, TradeDto dto)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
                return false;

            trade.Account = dto.Account;
            trade.AccountType = dto.AccountType;
            trade.BuyQuantity = dto.BuyQuantity;
            trade.SellQuantity = dto.SellQuantity;
            trade.BuyPrice = dto.BuyPrice;
            trade.SellPrice = dto.SellPrice;
            trade.TradeDate = dto.TradeDate;
            trade.TradeSecurity = dto.TradeSecurity;
            trade.TradeStatus = dto.TradeStatus;
            trade.Trader = dto.Trader;
            trade.Benchmark = dto.Benchmark;
            trade.Book = dto.Book;
            trade.CreationName = dto.CreationName;
            trade.CreationDate = dto.CreationDate;
            trade.RevisionName = dto.RevisionName;
            trade.RevisionDate = dto.RevisionDate;
            trade.DealName = dto.DealName;
            trade.DealType = dto.DealType;
            trade.SourceListId = dto.SourceListId;
            trade.Side = dto.Side;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
                return false;

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
