using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Controllers
{
    public class CategoryController
    {
        private ICategoryRepository repository = new CategoryRepository();

        public async Task<int> Save(CategoryDtos categoryDtos)
        {
            return await repository.Save(categoryDtos);
        }

        public async Task<CategoryDtos> Find(int id)
        {
            return await repository.FindCategoryDtos(id);
        }

        public async Task<IEnumerable<CategoryDtos>> GetAll(string key = "")
        {
            return await repository.GetAll(key);
        }

        public async Task<bool> Delete(int id)
        {
            return await repository.Delete(id);
        }

        public async Task<bool> IsValidName(string oldName, string newName)
        {
            var queryDtosList = await repository.GetAll();

            var valid = true;

            if (queryDtosList.Count() > 0)
            {
                if (!string.IsNullOrWhiteSpace(oldName))
                    queryDtosList = queryDtosList.Where(cat => cat.Name != oldName);

                valid = !queryDtosList.Any(cat => cat.Name == newName);
            }

            return valid;
        }
    }
}
