using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using InventoryServices.Models;

namespace InventoryServices.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public async Task<int> Save(CategoryDtos categoryDtos)
        {
            var dbContext = new InventoryDbContext();

            var category = categoryDtos.AsCategory();

            var queryCategory = await FindCategory(category.Id, dbContext);

            if (queryCategory == null) dbContext.Categories.Add(category);
            else
            {
                queryCategory.Name = category.Name;

                queryCategory.MinimumStock = category.MinimumStock;

                dbContext.Entry(queryCategory).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return category.Id;
        }

        public async Task<CategoryDtos> FindCategoryDtos(int id)
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                var queryCategory = await FindCategory(id, dbContext);

                if (queryCategory == null) return null;

                return queryCategory.AsCategoryDtos();
            }
        }

        public async Task<IEnumerable<CategoryDtos>> GetAll(string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {

                var queryCategoryList = await dbContext.Categories
                    .AsNoTracking()
                    //.Include(cat => cat.Items)
                    //.Include(cat => cat.Items.Select(item => item.Category))
                    //.Include(cat => cat.Items.Select(item => item.ItemPriceList))
                    .Where(cat => cat.Name.Contains(key))
                    .OrderBy(cat => cat.Name)
                    .ToListAsync();

                return queryCategoryList.AsParallel().Select(item => item.AsCategoryDtos()).ToList();
            }
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindCategory(id, dbContext);

            if (query != null)
            {

                var itemList = await GetItemsByCategory(id, dbContext);

                if (itemList.Count < 1) return false;

                dbContext.Categories.Remove(query);
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        #region Helper Method
        private async Task<Category> FindCategory(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Categories
                //.Include(cat => cat.Items)
                //.Include(cat => cat.Items.Select(item => item.Category))
                //.Include(cat => cat.Items.Select(item => item.ItemPriceList))
                .FirstOrDefaultAsync(cat => cat.Id == id);
        }

        private async Task<List<Item>> GetItemsByCategory(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Where(item => item.CategoryId == id)
                .ToListAsync();
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion
    }
}
