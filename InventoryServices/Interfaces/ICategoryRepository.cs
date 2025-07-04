using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ICategoryRepository
    {
        Task<int> Save(CategoryDtos categoryDtos);
        Task<CategoryDtos> FindCategoryDtos(int id);
        Task<IEnumerable<CategoryDtos>> GetAll(string key = "");
        Task<bool> Delete(int id);
    }
}
