using FinalProject_Data;
using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Services
{
    public interface IProductServices
    {
        Product Create(Product product);
        void Delete(int id);
        Product Get(int id);
        Product Edit(Product product);
        List<Product> Search(ProductSearching productSearching);
        Cart AddToCart(int UserID, int ProductID, int ProductQuantity);
        Cart RemoveFromCart(int UserID, int ProductID);
        Receipt MakeReceipt(int UserID);
    }
    public class ProductServices : IProductServices
    {
        private readonly DatabaseContext _context;
        public ProductServices(DatabaseContext context)
        {
            _context = context;
        }
        public Product Create(Product product)
        {
            _context.Add(product);
            _context.SaveChanges();
            return product;
        }

        public void Delete(int id)
        {
            var product = _context.Products.Where(x => x.ID == id).Include(x=>x.Carts).FirstOrDefault();
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public Product Edit(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return product;
        }

        public Product Get(int id)
        {
            var product = _context.Products.Where(product => product.ID == id).FirstOrDefault();
            return product;
        }

        public List<Product> Search(ProductSearching productSearching)
        {
            List<Product> searchResult = new List<Product>();
            searchResult = _context.Products.Where(x => x.Name.Contains(productSearching.SearchString) 
                                                     && x.Price >= productSearching.LowerPrice 
                                                     && x.Price <= productSearching.UpperPrice
                                                     && x.Category == productSearching.Category)
                                            .Skip((productSearching.PageNumber - 1) * productSearching.PageSize)
                                            .Take(productSearching.PageSize)
                                            .ToList();
            return searchResult;
        }
        public Cart AddToCart(int UserID, int ProductID, int ProductQuantity)
        {
            var user = _context.Accounts.Find(UserID);
            var product = _context.Products.Find(ProductID);
            var cart = new Cart();
            if(user != null && product != null)
            {
                if (_context.Carts.Where(x => x.AccountID == UserID && x.ProductID == ProductID).FirstOrDefault() == null)
                {
                    cart = new Cart()
                    {
                        AccountID = UserID,
                        ProductID = ProductID,
                        ProductQuantity = ProductQuantity,
                        TotalPrice = product.Price * ProductQuantity,
                    };
                    _context.Carts.Add(cart);
                }
                else
                {
                    cart = _context.Carts.Where(x => x.AccountID == UserID && x.ProductID == ProductID).FirstOrDefault();
                    cart.ProductQuantity += ProductQuantity;
                    cart.TotalPrice = product.Price * cart.ProductQuantity;
                    _context.Entry(cart).State = EntityState.Modified;
                }

                _context.SaveChanges();
                return cart;
            }
            else
            {
                throw new ArgumentException("User or product not found") ;
            }

        }
        public Cart RemoveFromCart(int UserID, int ProductID)
        {
            var user = _context.Accounts.Find(UserID);
            var product = _context.Products.Find(ProductID);
            if (user != null && product != null)
            {
                try
                {
                    var cart = _context.Carts.Where(x => x.AccountID == UserID && x.ProductID == ProductID).FirstOrDefault();
                    _context.Carts.Remove(cart);
                    _context.SaveChanges();
                    return cart;
                }
                catch
                {
                    throw new ArgumentException("Cart error");
                }
            }
            else
            {
                throw new ArgumentException("User or product not found");
            }
        }

        public Receipt MakeReceipt(int UserID)
        {
            var user = _context.Accounts.Find(UserID);
            var carts = user.Carts;
            var totalSum = carts.Sum(cart => cart.TotalPrice);
            var receipt = new Receipt()
            {
                AccountID = UserID,
                Account = user,
                Date = DateTime.Today,
                TotalSum = totalSum
            };
            _context.Receipts.Add(receipt);
            _context.SaveChanges();

            foreach (var cart in carts){
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            }

            return receipt;
        }
    }
}
